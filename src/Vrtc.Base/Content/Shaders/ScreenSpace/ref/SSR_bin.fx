
// Number of Samples to try for each pixel to check surrounding pixels as well for intersections.
float SampleTries = 3;

// Number of loops to increment by for the main loop
float Loops = 20;


// The Scene, lit with Fog, Shadows and Lighting. 
// Essentially Everything which will be reflected.

sampler SceneTextureSampler : register(s0);



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


// The Position of the Camera.
float3 CameraPosition;

// The Camera's View Projection
float4x4 ViewProjection;

// The inverse View Projection
float4x4 InverseViewProjection;


float4x4 MatrixTransform;
float2 HalfPixel;

float DepthCheckBias = 0.0001f;


#define SAMPLE_COUNT 4 


float3 RAND_SAMPLES[SAMPLE_COUNT] = 
{
      float3( 0.5381, 0.1856,-0.4319), 
	  float3( 0.1379, 0.2486, 0.4430),
      float3( 0.3371, 0.5679,-0.0057), 
	  float3(-0.6999,-0.0451,-0.0019),
      //float3( 0.0689,-0.1598,-0.8547),
      //float3( 0.0689,-0.1598,-0.8547),
      //float3( 0.0689,-0.1598,-0.8547),
      //float3( 0.0689,-0.1598,-0.8547),
  };

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
void SpriteVertexShader(
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : POSITION0)
{
	position = mul(position, MatrixTransform);
}

// A struct for data from an intersection run.
struct RayTraceOutput
{
	bool Hit;
	float3 UV;
};


float BinLoops = 0.1;
float depthCutoff = 0.1;
float LScale = 1;
float EdgeCutOff = 0.025;

// refine the location of the 'z' value
RayTraceOutput SSRBinarySearch(float3 vDir, float3 curPos3D)
{
	RayTraceOutput result;
	result.Hit = true;
	result.UV = float3(0,0,0);

	float curDepth;
	float3 curUV = 0;
	for (int i = 0; i < 12; i++)
	{
		// Get the UV Coordinates of the current Ray
		curUV = GetUVFromPosition(curPos3D);
		
		// The Depth of the Current Pixel
		curDepth = GetDepth(curUV.xy);
		
		float DepthDiff = curUV.z - curDepth;

		if (DepthDiff <= 0.0f)
			curPos3D += vDir;

		vDir *= 0.5f;
		curPos3D -= vDir;

		if((abs(DepthDiff) < depthCutoff))
		{
			result.UV = curUV;
			return result;
		}
	}
	
			// Get the UV Coordinates of the current Ray
		curUV = GetUVFromPosition(curPos3D);
		
		// The Depth of the Current Pixel
		curDepth = GetDepth(curUV.xy);
		
		float fDepthDiff = curUV.z - curDepth;
	
	result.Hit = (abs(fDepthDiff) < depthCutoff);
	result.UV = curUV;

	return result;

	//return float4(vProjectedCoord.xy, fDepth, abs(fDepthDiff) < g_fRayhitThreshold ? 1.0f : 0.0f);
}


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
	float3 reflectDir = normalize(reflect(viewDir, normalize(texelNormal)));

	float3 curPos3D = 0;
	float curDepth=0;
	float3 curUV = 0;
float L = 1;
	for (int i = 0; i < 12; i++)
	{
			// Update the Current Position of the Ray
			curPos3D = texelPosition + reflectDir * L;

			// Get the UV Coordinates of the current Ray
			curUV = GetUVFromPosition(curPos3D);
			
			// The Depth of the Current Ray Marched Pixel
			curDepth = GetDepth(curUV.xy);
			
			// Now check if the current ray point is infront of the depth buffer point.
			float DepthDiff = curUV.z - curDepth;

			// perform a Binary search if it's greater than zero.
			if (DepthDiff > 0.0f)
				return SSRBinarySearch(reflectDir, curPos3D);

			//reflectDir *= 2.5;
			
			float3 newPosition = GetWorldPosition(curUV.xy, curDepth);
			L = length(texelPosition - newPosition) * LScale;
	}
	return output;
}

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	// Get the Amount of reflection. Only calculate reflection on 
	// surfaces with reflection data.
	float amount = tex2D(NormalSampler, texCoord - HalfPixel * 2).a;

	if (amount > 0)
	{
		RayTraceOutput ray = TraceRay(texCoord - HalfPixel * 2);
	
		if (ray.Hit == true)
		{
			if (ray.UV.y < EdgeCutOff * 2)
				amount *= (ray.UV.y / EdgeCutOff / 2);

			//if(ray.depth < depthCutoff)
				//return float4(ray.depth, 0, 0, amount);

			return float4(tex2D(SceneTextureSampler, ray.UV).rgb * amount, amount);
		}
	}

	// If it didn't hit anything, then just simply return nothing
	return 0;
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
