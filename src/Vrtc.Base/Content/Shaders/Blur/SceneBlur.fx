//-----------------------------------------------------------------------------
// SceneBlur.fx
//
//-----------------------------------------------------------------------------

sampler SceneTexture : register(s0);



#define SAMPLE_COUNT 15

float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];


float2 HalfPixel;

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 uv : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 uv : TEXCOORD0;

};

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    output.uv = input.uv + HalfPixel;
    return output;
}


float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float4 c = 0;
    
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        c += tex2D(SceneTexture, input.uv + SampleOffsets[i]/2) * SampleWeights[i];
    }
    
    return c;
}

technique SceneBlur
{
    pass
    {
		VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();
    }
}