// Pixel shader extracts the brighter areas of an image.
// This is the first step in applying a bloom postprocess.

sampler TextureSampler : register(s0);

float BloomThreshold;

bool DoFullSceneBloom = true;

texture lightMap;
sampler lightSampler = sampler_state
{
    Texture = (lightMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture EmissiveMapTexture;
sampler EmissiveMapSampler = sampler_state//: register(s1) = sampler_state
{
	Texture = (EmissiveMapTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the original image color.
    float4 c = tex2D(TextureSampler, texCoord);

    if(DoFullSceneBloom == false)
    {
        	// Now get the light map colour
        float4 light = tex2D(lightSampler, texCoord);

        float3 diffuseLight = light.rgb;
        float specularLight = light.a;

        float e = tex2D(EmissiveMapSampler, texCoord).g + diffuseLight + specularLight;
        c *= e;
    }

    // Adjust it to keep only values brighter than the specified threshold.
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
