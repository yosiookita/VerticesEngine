
float4x4 Projection;



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


// The Position of the Camera.
float3 CameraPosition;

// The Camera's View Projection
float4x4 ViewProjection;

// The inverse View Projection
float4x4 InverseViewProjection;


float4x4 MatrixTransform;
float2 HalfPixel;





// Generic Vertex Shader
void SpriteVertexShader(inout float4 vColor : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : POSITION0)
{
	position = mul(position, MatrixTransform);
}






float distanceSquared(float2 a, float2 b)
{
	a -= b;
	return dot(a, a);
}

bool traceScreenSpaceRay(float3 csOrig, float3 csDir, float4x4 proj,
	float2 csZBufferSize, float zThickness,
	float nearPlaneZ,
	float stride, float jitter, const float maxSteps, float maxDistance,
	out float2 hitPixel, out float3 csHitPoint) {

	// Clip to the near plane
	float rayLength = ((csOrig.z + csDir.z * maxDistance) > nearPlaneZ) ?
		(nearPlaneZ - csOrig.z) / csDir.z : maxDistance;
	float3 csEndPoint = csOrig + csDir * rayLength;
	hitPixel = float2(-1, -1);

	// Project into screen space
	float4 H0 = mul(proj, float4(csOrig, 1.0)), H1 = mul(proj, float4(csEndPoint, 1.0));
	float k0 = 1.0 / H0.w, k1 = 1.0 / H1.w;
	float3 Q0 = csOrig * k0, Q1 = csEndPoint * k1;

	// Screen-space endpoints
	float2 P0 = H0.xy * k0, P1 = H1.xy * k1;

	// [ Optionally clip here using listing 4 ]

	P1 += (distanceSquared(P0, P1) < 0.0001) ? 0.01 : 0.0;
	float2 delta = P1 - P0;

	bool permute = false;
	if (abs(delta.x) < abs(delta.y)) {
		permute = true;
		delta = delta.yx; P0 = P0.yx; P1 = P1.yx;
	}

	float stepDir = sign(delta.x), invdx = stepDir / delta.x;

	// Track the derivatives of Q and k.
	float3 dQ = (Q1 - Q0) * invdx;
	float dk = (k1 - k0) * invdx;
	float2 dP = float2(stepDir, delta.y * invdx);

	dP *= stride; dQ *= stride; dk *= stride;
	P0 += dP * jitter; Q0 += dQ * jitter; k0 += dk * jitter;
	float prevZMaxEstimate = csOrig.z;
	// Slide P from P0 to P1, (now-homogeneous) Q from Q0 to Q1, k from k0 to k1
	float3 Q = Q0; float k = k0, stepCount = 0.0, end = P1.x * stepDir;

#ifndef GLSL
	[loop]
#endif
	for (float2 P = P0;
		((P.x * stepDir) <= end) && (stepCount < maxSteps);
		P += dP, Q.z += dQ.z, k += dk, stepCount += 1.0) {

		// Project back from homogeneous to camera space
		hitPixel = permute ? P.yx : P;

		// The depth range that the ray covers within this loop iteration.
		// Assume that the ray is moving in increasing z and swap if backwards.
		float rayZMin = prevZMaxEstimate;
		// Compute the value at 1/2 pixel into the future
		float rayZMax = (dQ.z * 0.5 + Q.z) / (dk * 0.5 + k);
		prevZMaxEstimate = rayZMax;
		if (rayZMin > rayZMax) {
			float t = rayZMin;
			rayZMin = rayZMax;
			rayZMax = t;
		}

		float2 uv = hitPixel / csZBufferSize;
		if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
			return false;

		// Camera-space z of the background at each layer (there can be up to 4)
		float sceneZMax = -unpackDepthLod0(DepthSampler, uv);
		float sceneZMin = sceneZMax - zThickness;

		if (((rayZMax >= sceneZMin) && (rayZMin <= sceneZMax)) ||
			(sceneZMax == 0)) {
			return true; // Breaks out of both loops, since the inner loop is a macro
		}
	} // for each pixel on ray

	  // Advance Q based on the number of steps
	Q.xy += dQ.xy * stepCount; csHitPoint = Q * (1.0 / k);
	return false;

	return dot(abs(hitPixel - (csZBufferSize * 0.5)) <= csZBufferSize * 0.5, 1) > 1.5;
}


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float amount = tex2D(RefecltionMapSampler, texCoord).r;

	// First get the Depth and Normal values for this pixel
	float depth = tex2D(DepthSampler, texCoord).r;
	float3 texelPosition = GetPosition(texCoord, depth);

	float4 normalData = tex2D(NormalSampler, texCoord);
	//tranform normal back into [-1,1] range
	float3 texelNormal = 2.0f * normalData.xyz - 1.0f;

	// First, Get the View Direction
	float3 viewDir = normalize(texelPosition - CameraPosition);
	float3 reflectDir = normalize(reflect(viewDir, texelNormal));


	float4 DiffCol = tex2D(ColorSampler, texCoord);

	float zThickness = 1000.f;
	float nearPlaneZ = -0.5f;
	float stride = 20;
	float jitter = 1.123;
	float maxSteps = 40;
	float maxDistance = 400;

	float2 hitPixel = 0;
	float3 csHitPoint = 0;

	float2 resolution = float2(1000, 600);

	float4x4 vpxf =
		float4x4
		(
			0.5f * resolution.x, 0, 0, 0.5f * resolution.x,
			0, -0.5f * resolution.y, 0, 0.5f * resolution.y,
			0, 0, 1, 0,
			0, 0, 0, 1
			);

	float4x4 proj = mul(vpxf, (Projection));

	bool r = traceScreenSpaceRay(csOrig, csDir, proj, resolution, zThickness, nearPlaneZ, stride, jitter, maxSteps, maxDistance, hitPixel, csHitPoint);

	float fade = r;
	float2 uv = hitPixel / resolution;


	return lerp(DiffCol, tex2D(ColorSampler, uv).rgb, fade);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 SpriteVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
