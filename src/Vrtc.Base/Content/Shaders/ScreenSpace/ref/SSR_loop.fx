



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

	position.x = (UV.x * 2.0f - 1.0f);
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

// A struct for data from an intersection run.
struct RayTraceOutput
{
	bool Hit;
	float2 UV;
	//float4 Colour;
};

RayTraceOutput TraceRay(float2 texCoord, float scale = 1)
{
	// Setup Results Output
	RayTraceOutput output;
	output.Hit = false;
	output.UV = 0;

	//Get initial Depth
	float InitDepth = GetDepth(texCoord);

	// Now get the position
	float3 texelPosition = GetPosition(texCoord, InitDepth);

	// Get the Normal Data
	float4 normalData = tex2D(NormalSampler, texCoord);

	//tranform normal back into [-1,1] range
	float3 texelNormal = 2.0f * normalData.xyz - 1.0f;

	// First, Get the View Direction
	float3 viewDir = normalize(texelPosition - CameraPosition);
	float3 reflectDir = normalize(reflect(viewDir, texelNormal));


	//float4 DiffCol = tex2D(ColorSampler, texCoord);



	float3 currentRay = 0;

	float3 nuv = 0;
	float L = 1;
	float2 finalUV = 0;
	float4 OutputColour = 0;

	int loops = 8 * scale;

	float samples = 4;


	for (int i = 0; i < loops; i++)
	{
		if (output.Hit == false)
		{
			// Update the Current Position of the Ray
			currentRay = texelPosition + reflectDir * L;

			// Get the UV Coordinates of the current Ray
			nuv = GetUV(currentRay);

			// The Depth of the Current Pixel
			float n = GetDepth(nuv.xy);
			//float n2 = GetDepth(nuv.xy + HalfPixel * 2);
			//float n3 = GetDepth(nuv.xy - HalfPixel * 2);

			for (int j = 0; j < samples; j++)
			{
				// We've found the correct intersection point once the depth of the currnet pixel, 'n', is 
				// less than the depth of our projected pixel, nuv. That is to say, when nuv.z > n, then we've
				// hit an intersection point
				if (n < nuv.z)
				{
					//if (n > InitDepth &&  abs(nuv.z - n) < 0.00001f)
					if (abs(nuv.z - n) < 0.0001f && n > 0.00001f)
					{
						i = loops + 1;

						output.Hit = true;

						// Set Output UV
						output.UV = nuv.xy;
						break;
					}
				}
				else
				{
					if (j == 0)
						n = GetDepth(nuv.xy + HalfPixel * 2);
					else if (j == 1)
						n = GetDepth(nuv.xy - HalfPixel * 2);
					else if (j == 2)
						n = GetDepth(nuv.xy + float2(HalfPixel.x * 2, -HalfPixel.y * 2));
					else if (j == 3)
						n = GetDepth(nuv.xy + float2(-HalfPixel.x * 2, HalfPixel.y * 2));
				}
			}

			if (output.Hit == true)
				break;

			float3 newPosition = GetPosition(nuv.xy, n);
			L = length(texelPosition - newPosition);
		}
	}
	return output;
}

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	//return lerp(DiffCol, tex2D(ColorSampler, finalUV), amount);

	float amount = tex2D(RefecltionMapSampler, texCoord).r;

float2 resolution = float2(1000, 600);

float4 DiffCol = tex2D(ColorSampler, texCoord);

float4 OutputColour = 0;


RayTraceOutput ray = TraceRay(texCoord);

float cutoff = 0.05f;
if (ray.Hit == true)
{
	OutputColour = tex2D(ColorSampler, ray.UV);
	if (ray.UV.y < cutoff)
		OutputColour *= (ray.UV.y / cutoff);
	/*if(ray.UV.x < cutoff)
	OutputColour *= (ray.UV.x / cutoff);*/
	/*if (1-ray.UV.x > cutoff)
	OutputColour *= (1-ray.UV.x);*/
}
//else
//{
//	RayTraceOutput ray2 = TraceRay(texCoord);
//	OutputColour = tex2D(ColorSampler, ray2.UV);
//}
//OutputColour = float4(finalUV, 0, 1);

return OutputColour * amount;

//return lerp(DiffCol, OutputColour, amount * OutputColour.r*2);
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
