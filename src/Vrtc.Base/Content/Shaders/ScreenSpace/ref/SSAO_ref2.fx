
#define SAMPLE_COUNT 16
float2 Radius;
float RandomTile;
float Bias;
float FarClip;
float4x4 uProjectionMat;
float3 vViewRay;

float4x4 MatrixTransform;
/*
#define SAMPLE_COUNT 16
float3 RAND_SAMPLES[SAMPLE_COUNT] =
{
	float3(0.5381, 0.1856,-0.4319),
	float3(0.1379, 0.2486, 0.4430),
	float3(0.3371, 0.5679,-0.0057),
	float3(-0.6999,-0.0451,-0.0019),
	float3(0.0689,-0.1598,-0.8547),
	float3(0.0560, 0.0069,-0.1843),
	float3(-0.0146, 0.1402, 0.0762),
	float3(0.0100,-0.1924,-0.0344),
	float3(-0.3577,-0.5301,-0.4358),
	float3(-0.3169, 0.1063, 0.0158),
	float3(0.0103,-0.5869, 0.0046),
	float3(-0.0897,-0.4940, 0.3287),
	float3(0.7119,-0.0154,-0.0918),
	float3(-0.0533, 0.0596,-0.5411),
	float3(0.0352,-0.0631, 0.5460),
	float3(-0.4776, 0.2847,-0.0271)
};
*/

// This texture contains the main scene image, which the edge detection
// and/or sketch filter are being applied over the top of.
texture SceneTexture;
sampler SceneSampler = sampler_state
{
	Texture = (SceneTexture);

	MinFilter = Linear;
	MagFilter = Linear;

	AddressU = Clamp;
	AddressV = Clamp;
};


// This texture contains the Depth Value.
texture DepthBuffer;
sampler depthSampler = sampler_state
{
	Texture = (DepthBuffer);

	MinFilter = Point;
	MagFilter = Point;

	AddressU = Clamp;
	AddressV = Clamp;
};


texture NormalBuffer;
sampler2D normalSampler = sampler_state
{
	Texture = <NormalBuffer>;
	MipFilter = NONE;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
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


void SpriteVertexShader(inout float4 vColor : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : POSITION0)
{
	position = mul(position, MatrixTransform);
}

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
    

    // get view ray
    float3 origin = vViewRay * texture(depthSampler, texCoord).r;

    float3 normal = texture(normalSampler, texCoord).xyz * 2.0 - 1.0;
    normal = normalize(normal);

    float3 rvec = texture(randomSampler, texCoord * 1).xyz * 2.0 - 1.0;
   float3 tangent = normalize(rvec - normal * dot(rvec, normal));
   float3 bitangent = cross(normal, tangent);
   float3x3 tbn = mat3(tangent, bitangent, normal);

    for (int i = 0; i < 5; ++i) {
    // get sample position:
    float3 kernelsample = tbn * uSampleKernel[i];
    smple = smple * Radius + origin;
  
    // project sample position:
    float4 offset = float4(smple, 1.0);
    offset = uProjectionMat * offset;
    offset.xy /= offset.w;
    offset.xy = offset.xy * 0.5 + 0.5;
  
    // get sample depth:
    float sampleDepth = texture(depthSampler, offset.xy).r;
  
    // range check & accumulate:
    float rangeCheck= abs(origin.z - sampleDepth) < Radius ? 1.0 : 0.0;
    occlusion += (sampleDepth <= smple.z ? 1.0 : 0.0) * rangeCheck;
    }
	 occlusion = 1.0 - (occlusion / 5);
	return float4(occlusion, occlusion, occlusion, 1);
}

technique Technique1
{
    pass Pass0
    {
		VertexShader = compile vs_3_0 SpriteVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
