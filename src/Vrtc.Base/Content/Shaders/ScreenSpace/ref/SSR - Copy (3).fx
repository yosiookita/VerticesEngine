
// Number of Samples to try for each pixel to check surrounding pixels as well for intersections.
float SampleTries = 3;

// Number of loops to increment by for the main loop
float Loops = 8;

// Cutoff Value for screen borders
float cutoff = 0.05f;

bool Blur = false;

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

// The Amount of Reflection Required. If the Green Channel is 0, then 
// the shader will skip checking for reflection for this pixel.
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
float3 GetWorldPosition(float2 uv, float depth)
{
	float4 pos = 1.0f;

	// Convert the UV values.
	pos.x = (uv.x * 2.0f - 1.0f);
	pos.y = -(uv.y * 2.0f - 1.0f);

	pos.z = depth;

	//Transform Position from Homogenous Space to World Space 
	pos = mul(pos, InverseViewProjection);

	pos /= pos.w;

	return pos.xyz;
}


// Converts a World Position into UV coordinates with a depth value.
float3 GetUVFromPosition(float3 position)
{
	// Convert Position into View Space
	float4 UVpos = mul(float4(position, 1.0f), ViewProjection);

	// Now convert the UVpos
	UVpos.xy = float2(0.5f, 0.5f) + float2(0.5f, -0.5f) * UVpos.xy / UVpos.w;

	// return the UV pos with the depth value at that location
	return float3(UVpos.xy, UVpos.z / UVpos.w);
}

// Returns the depth, given a UV texture coordinate
float GetDepth(float2 texCoord)
{
	return tex2Dlod(DepthSampler, float4(texCoord.xy, 0, 0)).r;
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
	float3 texelPosition = GetWorldPosition(texCoord, InitDepth);

	// Get the Normal Data
	float3 normalData = tex2D(NormalSampler, texCoord).xyz;

	//tranform normal back into [-1,1] range
	float3 texelNormal = 2.0f * normalData - 1.0f;

	// First, Get the View Direction
	float3 viewDir = normalize(texelPosition - CameraPosition);
	float3 reflectDir = normalize(reflect(viewDir, texelNormal));




	float3 currentRay = 0;

	float3 nuv = 0;
	float L = 1;
	//float2 finalUV = 0;
	float4 OutputColour = 0;

	int loops = Loops * scale;


	for (int i = 0; i < loops; i++)
	{
		if (output.Hit == false)
		{
			// Update the Current Position of the Ray
			currentRay = texelPosition + reflectDir * L;

			// Get the UV Coordinates of the current Ray
			nuv = GetUVFromPosition(currentRay);

			// The Depth of the Current Pixel
			float n = GetDepth(nuv.xy);
			float n2 = GetDepth(nuv.xy + HalfPixel * 2);
			float n3 = GetDepth(nuv.xy - HalfPixel * 2);
			float n4 = GetDepth(nuv.xy + float2(HalfPixel.x, -HalfPixel.y) * 2);
			float n5 = GetDepth(nuv.xy + float2(-HalfPixel.x, HalfPixel.y) * 2);


			// We've found the correct intersection point once the depth of the currnet pixel, 'n', is 
			// less than the depth of our projected pixel, nuv. That is to say, when nuv.z > n, then we've
			// hit an intersection point
			if (n < nuv.z)
			{
				//if (n > InitDepth &&  abs(nuv.z - n) < 0.00001f)
				if (abs(nuv.z - n) < 0.0001f)
				{
					output.Hit = true;

					// Set Output UV
					output.UV = nuv.xy;
					break;
				}
			}
			else if (n2 < nuv.z)
			{
				//if (n > InitDepth &&  abs(nuv.z - n) < 0.00001f)
				if (abs(nuv.z - n2) < 0.0001f)
				{
					output.Hit = true;

					// Set Output UV
					output.UV = nuv.xy;
					break;
				}
			}
			else if (n3 < nuv.z)
			{
				//if (n > InitDepth &&  abs(nuv.z - n) < 0.00001f)
				if (abs(nuv.z - n3) < 0.0001f)
				{
					output.Hit = true;

					// Set Output UV
					output.UV = nuv.xy;
					break;
				}
			}
			else if (n4 < nuv.z)
			{
				//if (n > InitDepth &&  abs(nuv.z - n) < 0.00001f)
				if (abs(nuv.z - n4) < 0.0001f)
				{
					output.Hit = true;

					// Set Output UV
					output.UV = nuv.xy;
					break;
				}
			}
			else if (n5 < nuv.z)
			{
				//if (n > InitDepth &&  abs(nuv.z - n) < 0.00001f)
				if (abs(nuv.z - n5) < 0.0001f)
				{
					output.Hit = true;

					// Set Output UV
					output.UV = nuv.xy;
					break;
				}
			}

			float3 newPosition = GetWorldPosition(nuv.xy, n);
			L = length(texelPosition - newPosition);
		}
	}
	return output;
}

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	// Get the Amount of
	float amount = tex2D(NormalSampler, texCoord).a;

	float2 resolution = float2(1000, 600);

	float4 DiffCol = tex2D(ColorSampler, texCoord);

	float4 OutputColour = 0;

	if (amount > 0)
	{
		RayTraceOutput ray = TraceRay(texCoord);
	
		if (ray.Hit == true)
		{
			OutputColour = tex2D(ColorSampler, ray.UV);
	
			// Cut off if it's near the edges of the screen
	
			// Top of screen cut off
			if (ray.UV.y < cutoff)
				amount *= (ray.UV.y / cutoff);

			if (ray.UV.y <= 0 || ray.UV.x <= 0 || ray.UV.y >= 1 || ray.UV.x >= 1)
				amount = 0;
	
			// Left Side of screen cut off
			if (ray.UV.x < cutoff)
				amount *= (ray.UV.x / cutoff);
	
			// Right Side of screen cut off
			if (1 - ray.UV.x < cutoff)
				amount *= ((1 - ray.UV.x) / cutoff);
	
		
			//TODO: Blurring based on roughness and distance
			if (Blur == true)
			{
				// Returns the UV coordinates in the R and G values as well as the Amount in the B value.
				//return float4(ray.UV.xy, 0, amount);
				float blur = length(texCoord - ray.UV);

				//amount *= clamp(1 - length(texCoord - ray.UV)*2, 0, 1);

				float d = HalfPixel * 25 * blur;
				// The ray has successfully hit a reflection, therefore linerarly interpolate between the
				// scene colour and the reflection colour by the calculated amount.
				float4 col1 = tex2D(ColorSampler, ray.UV + float2(d, d));
				float4 col2 = tex2D(ColorSampler, ray.UV + float2(-d, -d));
				float4 col3 = tex2D(ColorSampler, ray.UV + float2(d, -d));
				float4 col4 = tex2D(ColorSampler, ray.UV + float2(-d, d));

				OutputColour = OutputColour / 5 + col1 / 5 + col2 / 5 + col3 / 5 + col4 / 5;
			}
			
			return lerp(DiffCol, OutputColour, amount);
		}
	}

	// If it didn't hit anything, then just simply return the DiffCol
	return DiffCol;
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
