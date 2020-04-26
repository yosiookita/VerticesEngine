
//			Main Properties
//*********************************************************************************************
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float Alpha = 1;

// Lighting Values
float3 LightDirection = normalize(float3(1, 1, 1));
float4 AmbientLight = float4(0.5, 0.5, 0.5, 1);
float4 EmissiveColour = float4(0, 0, 0, 0);

float4 TintColor = float4(1, 1, 1, 1);

float SpecularIntensity = 1;
float SpecularPower = 5;

// Texture UV Coordinate Offset
float2 UVOffset = float2(0, 0);

// UV Factor to keep apparent texture position the same when plane scaling (i.e. water scaling outwards)
float2 UVFactor = float2(1, 1);

float3 WaterScale = float3(1, 1, 1);

//Fog Variables

float fFresnelBias = 0.025f;
float fFresnelPower  = 6.0f;
float fHDRMultiplier  = 0.450f;

//float4 vDeepColor = { 0.0f, 0.20f, 0.4150f, 1.0f };
float4 vDeepColor = float4(0, 0.25, 0.5, 1);

//float4 vShallowColor  = { 0.35f, 0.55f, 0.55f, 1.0f };
float4 vShallowColor = float4(0, 0.5, 0.75, 1);


float fReflectionAmount = 0.5f;

float fWaterAmount= 0.5f;

// Camera Position
float3 CameraPos;

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

texture SpecularMap;
sampler specularSampler = sampler_state
{
	Texture = (SpecularMap);
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture BumpMap;
sampler BumpMapSampler = sampler_state
{
	Texture = (BumpMap);
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};


texture DepthMap;
sampler DepthMapSampler = sampler_state
{
	Texture = (DepthMap);
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture AuxDepthMap;
sampler AuxDepthMapSampler = sampler_state
{
	Texture = (AuxDepthMap);
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

//texture ReflectionTexture;
//samplerCUBE ReflectionSampler = sampler_state
//{
//	texture = <ReflectionTexture>;
//	magfilter = LINEAR;
//	minfilter = LINEAR;
//	mipfilter = LINEAR;
//	AddressU = Mirror;
//	AddressV = Mirror;
//};

texture ReflectionMap;
sampler ReflectionMapSampler = sampler_state
{
	Texture = (ReflectionMap);
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 SelectionColour;


texture ReflectionCube;
samplerCUBE ReflectionSampler = sampler_state
{
	texture = <ReflectionCube>;
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
	float2 TexCoord : TEXCOORD0;
};

struct MainVSOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Position3D	: TEXCOORD1;
	float4 ScrnPos : TEXCOORD2;
	//float3 Reflection : TEXCOORD2;
};



MainVSOutput MainVSFunction(MainVSInput input)
{
	MainVSOutput output;

	float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord * UVFactor;
	output.TexCoord += UVOffset;

	output.Position3D = worldPosition;

	output.ScrnPos = output.Position;// float2(output.Position.x / output.Position.z, output.Position.y / output.Position.z);

	return output;
}


// Pixel shader applies a water shader effect with fresnal shading
float4 MainPSFunction(MainVSOutput input) : COLOR0
{
	float3 normalVector = float3(0, 1, 0);

	//Get Bump Map
	//if (abs(length(dist)) < 5000)
	//{
		float4 bumpColorSample1 = tex2D(BumpMapSampler, input.TexCoord);
		

		float4 bumpColorSample2 = tex2D(BumpMapSampler, input.TexCoord - UVOffset/4);
		float4 bumpColor = bumpColorSample1;// (bumpColorSample1 + bumpColorSample2) / 2;
		//float2 perturbation = xWaveHeight*(bumpColor.rg - 0.5f)*2.0f;
		//perturbatedTexCoords = ProjectedTexCoords + perturbation;

		normalVector = (bumpColor.rbg - 0.5f)*2.0f;

		//return  float4(normalVector, 1);
	//}

	float3 eyeVector = normalize(CameraPos - input.Position3D);

	float fresnelTerm = dot(eyeVector, normalVector);

	// Compute the Fresnel term
	float fFacing = 1.0 - max(dot(eyeVector, normalVector), 0);
	float fFresnel = fFresnelBias + (1.0 - fFresnelBias) * pow(fFacing, fFresnelPower);

	// Compute the final water color
	float4 vWaterColor = lerp(vDeepColor * fWaterAmount, vShallowColor + EmissiveColour * 0.5, fFacing);

	float4 OutputColor = vWaterColor  + fFresnel;

	//Apply Specular
	float3 reflectionVector = -reflect(LightDirection, normalVector);
	float specular = dot(normalize(reflectionVector), normalize(eyeVector));
	specular = pow(specular, 256);


	// Calculate Reflection Angles
	float3 Normal = normalize(mul(normalVector, WorldInverseTranspose));
	float3 Reflection = reflect(normalize(eyeVector), normalize(Normal));

	// Get the Reflection Colour
	float4 refCol = texCUBE(ReflectionSampler, normalize(Reflection));

	// Now get the Relection Amount
	float refFactor = tex2D(ReflectionMapSampler, input.TexCoord).r;

	OutputColor.rgb = lerp(OutputColor.rgb, refCol, refFactor*fFacing);

	OutputColor.rgb += specular * specular;


	float2 scrnPos = float2(input.ScrnPos.x / input.ScrnPos.w / 2.0f + 0.5f, 1 - input.ScrnPos.y / input.ScrnPos.w / 2.0f + 0.5f);

	float f = 10000.0;
	float n = 0.1;
	float depth = (2 * n) / (f + n - tex2D(DepthMapSampler, scrnPos).x * (f - n));
	float auxDepth = (2 * n) / (f + n - tex2D(AuxDepthMapSampler, scrnPos).x * (f - n));


	float dif = (1 - abs(depth - auxDepth) * 250) * tex2D(AuxDepthMapSampler, scrnPos).x;

	return lerp(OutputColor,1, clamp(dif, 0, 1)) + SelectionColour;// +float4(input.ScrnPos.x / input.ScrnPos.w / 2.0f + 0.5f, input.ScrnPos.y / input.ScrnPos.w / 2.0f + 0.5f, 0, 1);
}

technique Technique_Water
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MainVSFunction();
		PixelShader = compile ps_3_0 MainPSFunction();
	}
}



//-----------------------------------------------------------------------------
//
// Displacement Mapping
//
//-----------------------------------------------------------------------------

float4x4 WorldViewProjection;
float4x4 WorldView;
float DistortionScale;
float Time;

struct PositionTextured
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD;
};

PositionTextured TransformAndTexture_VertexShader(PositionTextured input)
{
	PositionTextured output;

	output.Position = mul(input.Position, WorldViewProjection);
	output.TexCoord = input.TexCoord;
	output.TexCoord.x *= UVFactor.x;
	output.TexCoord.y *= UVFactor.y;
	output.TexCoord += UVOffset;

	return output;
}
float offset = 0;

texture2D DisplacementMap;
sampler2D DisplacementMapSampler = sampler_state
{
	texture = <DisplacementMap>;
};

float4 Textured_PixelShader(float2 texCoord : TEXCOORD) : COLOR
{
	float4 color = tex2D(DisplacementMapSampler, texCoord + float2(0,offset));

	//DistortionScale *= (1 / CameraPos - input.Position3D);

	// Ignore the blue channel    
	return float4(color.rgb, DistortionScale);
}

technique Distortion
{
	pass
	{
		VertexShader = compile vs_3_0 TransformAndTexture_VertexShader();
		PixelShader = compile ps_3_0 Textured_PixelShader();
	}
}