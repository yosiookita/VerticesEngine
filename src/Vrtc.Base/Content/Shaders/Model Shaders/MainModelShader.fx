
//			Main Properties
//*********************************************************************************************
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float Alpha = 1;


// Lighting Values
float DiffuseIntensity = 1;
float3 LightDirection = normalize(float3(1, 1, 1));

float4 AmbientLight = float4(0.5, 0.5, 0.5, 1);
float AmbientIntensity = 0.1;

float4 EmissiveColour = float4(0, 0, 0, 0);
float EmissiveIntensity = 1;

bool IsTextureEnabled = true;
float4 TintColor = float4(1, 1, 1, 1);

float SpecularIntensity = 1;
float SpecularPower = 5;

float4 SelectionColour = 0;

float ReflectionIntensity = 1;

// Texture UV Coordinate Offset
float2 UVOffset = float2(0, 0);

bool HasNormalMap = false;
bool HasReflectionMap = false;

//Fog Variables
float FogNear;
float FogFar;
float4 FogColor = { 1, 1, 1, 1 };
bool DoFog;
float3 CameraPos;


	float fFresnelBias = 0.025f;
	float fFresnelPower  = 6.0f;

//			Shadow Properties
//*********************************************************************************************
bool DoShadow = true;
bool ShadowDebug = false;

texture Texture;
sampler diffuseSampler = sampler_state
{
    Texture = (Texture);
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture SurfaceMap;
sampler surfaceMapSampler = sampler_state
{
    Texture = (SurfaceMap);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
    Texture = (NormalMap);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture ReflectionTexture;
samplerCUBE ReflectionSampler = sampler_state
{
	texture = (ReflectionTexture);
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};



//**************************************************
//					Main Shader
//**************************************************

/*
This Technique draws the rendertargets which are used
in other techniques later on, such as Normal and Depth
Calculations, Mask for God Rays. It performs all of
this in one pass rendering to multiple render targets
at once.
*/

// Vertex shader input structure.
struct MainVSInput
{
	float4 Position : POSITION0;
	float3 Normal 	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
	float3 Binormal	: BINORMAL0;
	float3 Tangent	: TANGENT0;
};

struct MainVSOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 LightAmount : TEXCOORD1;
	float3x3 tangentToWorld : TEXCOORD2;
	float3 Reflection : TEXCOORD5;
};

//Computes the Fog Factor
float ComputeFogFactor(float d)
{
	return clamp((d - FogNear) / (FogFar - FogNear), 0, 1);
}

MainVSOutput MainVSFunction(MainVSInput input, float4x4 worldTransform)
{
	MainVSOutput output;

	float4 worldPosition = mul(float4(input.Position.xyz, 1), worldTransform);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord.xy = input.TexCoord + UVOffset;

	// calculate tangent space to world space matrix using the world space tangent,
	// binormal, and normal as basis vectors
	output.tangentToWorld[0] = mul(input.Tangent, worldTransform);
	output.tangentToWorld[1] = mul(input.Binormal, worldTransform);
	output.tangentToWorld[2] = mul(input.Normal, worldTransform);



	//Calculate View Direction
	float3 ViewDirection = normalize(CameraPos - worldPosition);
	
	// The LightAmount is the 'x' component
	output.LightAmount.x = saturate(dot(output.tangentToWorld[2], LightDirection));


	float fFacing = 1.0 - max(dot(ViewDirection, output.tangentToWorld[2]), 0);
	float fFresnel = fFresnelBias + (1.0 - fFresnelBias) * pow(fFacing, fFresnelPower);
	// Place holder until I find 
	output.LightAmount.y = fFresnel;
	
	//Compute Fog Factor for the 'z' component for the Light Amount
	output.LightAmount.z = DoFog ? ComputeFogFactor(length(ViewDirection)) : 0;
	

	// Calculate Reflection Angles
	float3 Normal = normalize(mul(input.Normal, worldTransform));
	output.Reflection = reflect(-ViewDirection, normalize(Normal));

	return output;
}


MainVSOutput MainVSFunctionInstancedVS(MainVSInput input, float4x4 instanceTransform : TEXCOORD2)
{
	float4x4 worldTransform = mul(transpose(instanceTransform), World);
	return MainVSFunction(input, worldTransform);
}

MainVSOutput MainVSFunctionNonInstVS(MainVSInput input)
{
	return MainVSFunction(input, World);
}


// Pixel shader applies a cartoon shading algorithm.
float4 MainPSFunction(MainVSOutput input) : COLOR0
{
	//First, Get the Diffuse Colour of from the Texture
	//*********************************************************************************************
	float4 DiffuseColor = IsTextureEnabled ? tex2D(diffuseSampler, input.TexCoord) : float4(0.75,0.75,0.75,1);

	float4 SurfaceMap = tex2D(surfaceMapSampler, input.TexCoord.xy);

	float3 normalFromMap = 1 - tex2D(normalSampler, input.TexCoord).rgb * 2.0;
    normalFromMap = mul(normalFromMap, input.tangentToWorld);
	//float3 normalMap = 2.0 *(tex2D(normalSampler, input.TexCoord)) - 1.0;
    //normalMap = normalize(mul(normalMap, input.WorldToTangentSpace));
    //float4 normal = float4(normalMap,1.0);

	//float lightAmount = saturate(dot(-LightDirection, normalFromMap));
	//float4 Color = diffusecolor * (input.LightAmount.x) * DiffuseIntensity;// * input.LightAmount;// diffusecolor * saturate(dot(LightDirection, input.tangentToWorld[2]));
	//float4 Color = diffusecolor * (lightAmount + AmbientIntensity) * DiffuseIntensity;

    float4 diffuse = HasNormalMap ? saturate(dot(-LightDirection,normalFromMap)) : input.LightAmount.x;

	float4 Color = DiffuseColor * AmbientLight * AmbientIntensity + 
            DiffuseColor * DiffuseIntensity * (0.5 + diffuse * 0.5);

	// Get the Reflection Colour
	float4 refCol = texCUBE(ReflectionSampler, normalize(input.Reflection));

	// Now get the Relection Amount
	float refFactor = HasReflectionMap ? SurfaceMap.b * ReflectionIntensity : 0;

	Color = lerp(Color, refCol, refFactor * input.LightAmount.y);
	Color = lerp(Color, FogColor, input.LightAmount.z);

	return Color + float4(0, 0, 0, Alpha) + EmissiveColour * EmissiveIntensity + SelectionColour;
}

technique Technique_Main
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MainVSFunctionNonInstVS();
		PixelShader = compile ps_3_0 MainPSFunction();
	}
}

technique Technique_Main_Instanced
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MainVSFunctionInstancedVS();
		PixelShader = compile ps_3_0 MainPSFunction();
	}
}