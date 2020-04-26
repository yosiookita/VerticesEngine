float4x4 MatrixTransform;


// The inverse View Projection
float4x4 InverseViewProjection;


// The Camera's View Projection
float4x4 ViewProjection;


float2 Radius = float2(0.02, 0.5);

float Bias = 0.00001;
float RangeCutOff = 1;
float Intensity = 1.25;


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


texture RandomMap;
sampler2D randomSampler = sampler_state
{
	Texture = <RandomMap>;
	MipFilter = NONE;
	MagFilter = POINT;
	MinFilter = POINT;
	AddressU = Wrap;
	AddressV = Wrap;
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

// Returns the depth, given a UV texture coordinate
float GetDepth(float2 texCoord)
{
	return tex2Dlod(DepthSampler, float4(texCoord.xy, 0, 0)).r;
	//return tex2D(DepthSampler, texCoord).r;
}

float3 GetUVFromPosition(float3 position)
{
	// Convert Position into View Space
	float4 UVpos = mul(float4(position, 1.0f), ViewProjection);

	// Now convert the UVpos
	UVpos.xy = float2(0.5f, 0.5f) + float2(0.5f, -0.5f) * UVpos.xy / UVpos.w;

	// return the UV pos with the depth value at that location
	return float3(UVpos.xy, UVpos.z / UVpos.w);
}

half3 DecodeNormal (half4 enc)
{
	float kScale = 1.7777;
	float3 nn = enc.xyz*float3(2*kScale,2*kScale,0) + float3(-kScale,-kScale,1);
	float g = 2.0 / dot(nn.xyz,nn.xyz);
	float3 n;
	n.xy = g*nn.xy;
	n.z = g-1;
	return n;
}

void SpriteVertexShader(inout float4 vColor : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : POSITION0)
{
	position = mul(position, MatrixTransform);
}

#define SAMPLE_COUNT 16
float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{

	    float3 RAND_SAMPLES[SAMPLE_COUNT] = 
{
      float3( 0.5381, 0.1856,-0.4319), 
	  float3( 0.1379, 0.2486, 0.4430),
      float3( 0.3371, 0.5679,-0.0057), 
	  float3(-0.6999,-0.0451,-0.0019),
      float3( 0.0689,-0.1598,-0.8547), 
	  float3( 0.0560, 0.0069,-0.1843),
      float3(-0.0146, 0.1402, 0.0762), 
	  float3( 0.0100,-0.1924,-0.0344),
      float3(-0.3577,-0.5301,-0.4358), 
	  float3(-0.3169, 0.1063, 0.0158),
      float3( 0.0103,-0.5869, 0.0046), 
	  float3(-0.0897,-0.4940, 0.3287),
      float3( 0.7119,-0.0154,-0.0918), 
	  float3(-0.0533, 0.0596,-0.5411),
      float3( 0.0352,-0.0631, 0.5460), 
	  float3(-0.4776, 0.2847,-0.0271)
  };

		//Get initial Depth and 3d position
	float depth = GetDepth(texCoord);

	// get the world position
	float3 pos = GetWorldPosition(texCoord, depth);

	float2 newTexCoord = GetUVFromPosition(pos);
	
	//return float4(pos + float3( 1.1379, 0.2486, 0.4430),  1);
	// total occlusion
	float totalOcclusion = 0;

	
	//prevent near 0 divisions
	float scale = min(Radius.y,Radius.x / max(1,depth));


	half3 normal = DecodeNormal(tex2D(NormalSampler, texCoord));
	normal.y = -normal.y;

	//this will be used to avoid self-shadowing		  
	half3 normalScaled = normal * 0.25f;
	//pick a random normal, to add some "noise" to the output
	half3 randNormal = 1-(tex2D(randomSampler, texCoord * 100).rgb * 0.5);

	float3 finalPos = pos;
	float newDepth = 1;
	for (int i = 0; i < SAMPLE_COUNT; i++)
	{
		// get the offset from the Sample Kernel

		half3 randomDirection = reflect(RAND_SAMPLES[i], randNormal) ;
		//float2 offset = RAND_SAMPLES[i].xy * randNormal.xy * scale;
		randomDirection *= sign( dot(normal , randomDirection) );

		//randomDirection += normal;
		
		//we use a modified depth in the tests
		half modifiedDepth = depth -(randomDirection.z * Radius.x);

		// now check in the vicinity of this 
		newDepth = GetDepth(texCoord + randomDirection.xy * scale);

		//we only care about samples in front of our original-modifies 
		float deltaDepth = saturate(depth - newDepth);

		//ignore negative deltas
		float rangeCheck= abs(depth - newDepth) < RangeCutOff ? 1.0 : 0.0;
		totalOcclusion += (1-deltaDepth) * (deltaDepth > Bias) * rangeCheck;
	}
	//return newDepth;
	totalOcclusion /= SAMPLE_COUNT;

    return 1 - totalOcclusion * Intensity;
}

technique Technique1
{
    pass Pass0
    {
		VertexShader = compile vs_3_0 SpriteVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
