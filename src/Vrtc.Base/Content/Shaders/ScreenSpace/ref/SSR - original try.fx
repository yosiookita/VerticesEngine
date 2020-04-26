



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


// Gets the 3D Position based on the UV and Depth Coorrdinates
float3 GetPosition(float2 UV, float depth)
{
	float4 position = 1.0f;

	position.x = UV.x * 2.0f - 1.0f;
	position.y = -(UV.y * 2.0f - 1.0f);

	position.z = depth;

	//Transform Position from Homogenous Space to World Space 
	position = mul(position, InverseViewProjection);

	position /= position.w;

	return position.xyz;
}

float3 GetUV(float3 position)
{
	float4 pVP = mul(float4(position, 1.0f), ViewProjection);
	pVP.xy = float2(0.5f, 0.5f) + float2(0.5f, -0.5f) * pVP.xy / pVP.w;
	return float3(pVP.xy, pVP.z / pVP.w);
}

float GetDepth(float2 UV)
{
	return tex2Dlod(DepthSampler, float4(UV.xy, 0, 0)).r;
	//return tex2D(DepthSampler, UV.xy).r;
}


// Generic Vertex Shader
void SpriteVertexShader(inout float4 vColor : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : POSITION0)
{
	position = mul(position, MatrixTransform);
}



float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float amount = tex2D(RefecltionMapSampler, texCoord).r;

float2 resolution = float2(1000, 600);
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

// Now perform the Ray march


float3 currentRay = 0;

float3 nuv = 0;
float L = 1;
float2 finalUV = 0;

int end = 10;
int finalI = 0;

for (int i = 0; i < end; i++)
{
	currentRay = texelPosition + reflectDir * L;

	//if (length(currentRay) > 0.5)

	nuv = GetUV(currentRay);

	if (currentRay.z > 1.0f || currentRay.z < 0.0f)
		break;

	// The Depth of the Current Pixel
	float n = GetDepth(nuv.xy);


	// We've found the correct intersection point once the depth of the currnet pixel, 'n', is 
	// less than the depth of our projected pixel, nuv. That is to say, when nuv.z > n, then we've
	// hit an intersection point
	if (n < nuv.z)
	{
		finalI = i;
		i = end + 1;
		finalUV = nuv.xy;
		break;
	}

	float3 newPosition = GetPosition(nuv.xy, n);
	L = length(texelPosition - newPosition);
	//L++;
}

//if()


return lerp(DiffCol, tex2D(ColorSampler, finalUV), amount);

//return float4(reflectDir, 1);// float4(1, 0, 0, 1);
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
