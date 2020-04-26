float4x4 InvertViewProjection;
float4x4 ViewProjection;
float4x4 Projection;
float4x4 View;

float2 InverseResolution = (1.0f / 1000.0f, 1.0f / 600.0f);

float3 cameraPosition;
float3 cameraDir;

float cb_nearPlaneZ = -1;
float cb_farPlaneZ = 300;

float2 cb_depthBufferSize = { 1000, 600 };
float cb_zThickness = 0.000001f;
float cb_strideZCutoff = 200;
float cb_maxDistance = 299;
float cb_maxSteps = 40;
float cb_stride = 3;



// and/or sketch filter are being applied over the top of.
texture ColorMap;
sampler ColorSampler : register(s0) = sampler_state
{
	Texture = (ColorMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
};



// This texture contains normals (in the color channels) and depth (in alpha)
// for the main scene image. Differences in the normal and depth data are used
// to detect where the edges of the model are.
texture NormalMap;

sampler NormalSampler : register(s1) = sampler_state
{
	Texture = (NormalMap);

	MinFilter = Linear;
	MagFilter = Linear;

	AddressU = Clamp;
	AddressV = Clamp;
};


// This texture contains the Depth Value.
texture DepthMap;

sampler DepthSampler : register(s2) = sampler_state
{
	Texture = (DepthMap);

	MinFilter = Point;
	MagFilter = Point;

	AddressU = Clamp;
	AddressV = Clamp;
};

// This texture contains the main scene image, which the edge detection
// and/or sketch filter are being applied over the top of.
texture ReflectionMap;
sampler RefecltionMapSampler : register(s3) = sampler_state
{
	Texture = (ReflectionMap);

	MinFilter = Linear;
	MagFilter = Linear;

	AddressU = Clamp;
	AddressV = Clamp;
};


float4x4 MatrixTransform;
float2 HalfPixel;




////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  STRUCT DEFINITIONS

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 posH : SV_Position;
	float3 viewRay : VIEWRAY;
	float2 tex : TEXCOORD0;
};




////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  FUNCTION DEFINITIONS

//  DEFAULT LIGHT SHADER FOR MODELS
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.posH = float4(input.Position);
	output.viewRay = float3(input.Position.xy / input.Position.z, 1.0f);
	output.tex = input.TexCoord;
	return output;
}



float linearizeDepth(float depthSample)
{
	//depthSample = 2.0 * depthSample - 1.0;
	//float zLinear = 2.0 * cb_nearPlaneZ * cb_farPlaneZ / (cb_farPlaneZ + cb_nearPlaneZ - depthSample * (cb_farPlaneZ - cb_nearPlaneZ));

	float zLinear = depthSample * (cb_farPlaneZ - cb_nearPlaneZ) + cb_nearPlaneZ;
	return zLinear;
	//float zw = depthSample;
	//float linearZ = Projection._43 / (zw - Projection._33);
	//return linearZ;
}

float distanceSquared(float2 a, float2 b)
{
	a -= b;
	return dot(a, a);
}

bool intersectsDepthBuffer(float z, float minZ, float maxZ)
{
	/*
	* Based on how far away from the camera the depth is,
	* adding a bit of extra thickness can help improve some
	* artifacts. Driving this value up too high can cause
	* artifacts of its own.
	*/
	float depthScale = min(1.0f, z * cb_strideZCutoff);
	z += cb_zThickness + lerp(0.0f, 2.0f, depthScale);
	return (maxZ >= z) && (minZ - cb_zThickness <= z);
}

void swap(inout float a, inout float b)
{
	float t = a;
	a = b;
	b = t;
}

float linearDepthTexelFetch(int2 hitPixel)
{
	// Load returns 0 for any value accessed out of bounds
	return linearizeDepth(1 - tex2D(DepthSampler, hitPixel).r);
}


bool traceScreenSpaceRay(
	// Camera-space ray origin, which must be within the view volume
	float3 csOrig,
	// Unit length camera-space ray direction
	float3 csDir,
	// Number between 0 and 1 for how far to bump the ray in stride units
	// to conceal banding artifacts. Not needed if stride == 1.
	float jitter,
	// Pixel coordinates of the first intersection with the scene
	out float2 hitPixel,
	// Camera space location of the ray hit
	out float3 hitPoint)
{
	// Clip to the near plane
	float rayLength = ((csOrig.z + csDir.z * cb_maxDistance) < cb_nearPlaneZ) ?
		(cb_nearPlaneZ - csOrig.z) / csDir.z : cb_maxDistance;
	float3 csEndPoint = csOrig + csDir * rayLength;

	// Project into homogeneous clip space
	float4 H0 = mul(float4(csOrig, 1.0f), MatrixTransform);
	H0.xy *= cb_depthBufferSize;
	float4 H1 = mul(float4(csEndPoint, 1.0f), MatrixTransform);
	H1.xy *= cb_depthBufferSize;
	float k0 = 1.0f / H0.w;
	float k1 = 1.0f / H1.w;

	// The interpolated homogeneous version of the camera-space points
	float3 Q0 = csOrig * k0;
	float3 Q1 = csEndPoint * k1;

	// Screen-space endpoints
	float2 P0 = H0.xy * k0;
	float2 P1 = H1.xy * k1;

	// If the line is degenerate, make it cover at least one pixel
	// to avoid handling zero-pixel extent as a special case later
	P1 += (distanceSquared(P0, P1) < 0.0001f) ? float2(0.01f, 0.01f) : 0.0f;
	float2 delta = P1 - P0;

	// Permute so that the primary iteration is in x to collapse
	// all quadrant-specific DDA cases later
	bool permute = false;
	if (abs(delta.x) < abs(delta.y))
	{
		// This is a more-vertical line
		permute = true;
		delta = delta.yx;
		P0 = P0.yx;
		P1 = P1.yx;
	}

	float stepDir = sign(delta.x);
	float invdx = stepDir / delta.x;

	// Track the derivatives of Q and k
	float3 dQ = (Q1 - Q0) * invdx;
	float dk = (k1 - k0) * invdx;
	float2 dP = float2(stepDir, delta.y * invdx);

	// Scale derivatives by the desired pixel stride and then
	// offset the starting values by the jitter fraction
	float strideScale = 1.0f - min(1.0f, csOrig.z * cb_strideZCutoff);
	float stride = 1.0f + strideScale * cb_stride;
	dP *= stride;
	dQ *= stride;
	dk *= stride;

	P0 += dP * jitter;
	Q0 += dQ * jitter;
	k0 += dk * jitter;

	// Slide P from P0 to P1, (now-homogeneous) Q from Q0 to Q1, k from k0 to k1
	float4 PQk = float4(P0, Q0.z, k0);
	float4 dPQk = float4(dP, dQ.z, dk);
	float3 Q = Q0;

	// Adjust end condition for iteration direction
	float end = P1.x * stepDir;

	float stepCount = 0.0f;
	float prevZMaxEstimate = csOrig.z;
	float rayZMin = prevZMaxEstimate;
	float rayZMax = prevZMaxEstimate;
	float sceneZMax = rayZMax + 100.0f;
	for (;
		((PQk.x * stepDir) <= end) && (stepCount < cb_maxSteps) &&
		!intersectsDepthBuffer(sceneZMax, rayZMin, rayZMax) &&
		(sceneZMax != 0.0f);
		++stepCount)
	{
		rayZMin = prevZMaxEstimate;
		rayZMax = (dPQk.z * 0.5f + PQk.z) / (dPQk.w * 0.5f + PQk.w);
		prevZMaxEstimate = rayZMax;
		if (rayZMin > rayZMax)
		{
			swap(rayZMin, rayZMax);
		}

		hitPixel = permute ? PQk.yx : PQk.xy;
		// You may need hitPixel.y = depthBufferSize.y - hitPixel.y; here if your vertical axis
		// is different than ours in screen space
		sceneZMax = linearDepthTexelFetch(int2(hitPixel));

		PQk += dPQk;
	}

	// Advance Q based on the number of steps
	Q.xy += dQ.xy * stepCount;
	hitPoint = Q * (1.0f / PQk.w);
	return intersectsDepthBuffer(sceneZMax, rayZMin, rayZMax);
}


float4 PixelShaderFunction(VertexShaderOutput pIn) : COLOR0
{
	float amount = tex2D(RefecltionMapSampler,  pIn.tex).r;

	// First get the Depth and Normal values for this pixel
	float depth = 1-tex2D(DepthSampler, pIn.tex).r;
	float3 rayOriginVS = float4(pIn.viewRay, 1) * linearizeDepth(depth);

	//float4 normalData = tex2D(NormalSampler, pIn.tex);

	float3 normalVS = tex2D(NormalSampler, pIn.tex).xyz * 2 - 1;

	normalVS = mul(float4(normalVS, 0), ViewProjection).xyz;

	/*
	* Since position is reconstructed in view space, just normalize it to get the
	* vector from the eye to the position and then reflect that around the normal to
	* get the ray direction to trace.
	*/
	float3 toPositionVS = normalize(rayOriginVS);

	toPositionVS = mul(float4(cameraDir, 1), ViewProjection);

	float3 rayDirectionVS = normalize(reflect(toPositionVS, normalVS));

	// output rDotV to the alpha channel for use in determining how much to fade the ray
	float rDotV = dot(rayDirectionVS, toPositionVS);

	// out parameters
	float2 hitPixel = float2(0.0f, 0.0f);
	float3 hitPoint = float3(0.0f, 0.0f, 0.0f);

	float jitter = 1.123;// cb_stride > 1.0f ? float(int(pIn.posH.x + pIn.posH.y) & 1) * 0.5f : 0.0f;

	// perform ray tracing - true if hit found, false otherwise
	bool intersection = traceScreenSpaceRay(rayOriginVS, rayDirectionVS, jitter, hitPixel, hitPoint);

	depth = 1 - tex2D(DepthSampler, pIn.tex).r;

	// move hit pixel from pixel position to UVs
	hitPixel *= InverseResolution; //float2(texelWidth, texelHeight);
	if (hitPixel.x > 1.0f || hitPixel.x < 0.0f || hitPixel.y > 1.0f || hitPixel.y < 0.0f)
	{
		intersection = false;
	}

	return tex2D(ColorSampler, hitPixel);
}

technique Technique1
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
