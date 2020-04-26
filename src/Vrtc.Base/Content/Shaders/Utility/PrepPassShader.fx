﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//matrix WorldViewProjection;

// Main Properties
//*********************************************************************************************
float4x4 World;

// The World * View * Projection matrix. This is pre computed on the CPU to allow for quicker Vertex Shader Speed.
float4x4 wvp;

float3 CameraPos;

// Texture UV Coordinate Offset
float2 UVOffset = float2(0, 0);

// UV Factor to keep apparent texture position the same when plane scaling (i.e. water scaling outwards)
float2 UVFactor = float2(1, 1);

// The strength of the Emissivity Channel Value
float GlowIntensity = 1;

// Texture UV Coordinate Offset
float2 DistUVOffset = float2(0, 0);
// UV Factor to keep apparent texture position the same when plane scaling (i.e. water scaling outwards)
float2 DistUVFactor = float2(1, 1);

// The Primary Colour for the model for debugging
float4 PrimaryColour = float4(0.1, 0.1, 0.1, 1);

// The light direction is shared between the Lambert and Toon lighting techniques.
//*********************************************************************************************
float3 LightDirection = normalize(float3(1, 1, 1));

// Ambient Light Colour
float4 AmbientLight = float4(0.5, 0.5, 0.5, 1);

// Selection Colour 
float4 SelectionColour = float4(0, 0, 0, 0);

// Specular Intensity Factor
float SpecularIntensity = 1;

// Specular Power Factor
float SpecularPower = 5;

bool DoSSRefection = false;

// not all items should output normal data.
bool DoNormalMapping = true;
bool DoDepthMapping = true;
bool DoDistortionMapping = true;
bool DoEmissiveMapping = true;
bool DoReflections = false;

float ReflectionIntensity = 1;



	float fFresnelBias = 0.025f;
	float fFresnelPower  = 6.0f;

	

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

// Texture Samplers Variables
//*********************************************************************************************

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



// Distortion Properties
//*********************************************************************************************
texture2D DistortionMap;
sampler2D DisplacementMapSampler = sampler_state
{
	texture = <DistortionMap>;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

float DistortionScale = 0.25f;






//-----------------------------------------------------------------------------
// Shadow Mapping Code
//*********************************************************************************************

// Virtices Engine
// Collection of Common Cascade Shadow Mapping Code for use in 
// both the CascadeShadowShader.fx as well as in any other *.fx
// files which use it.
//
// It is adapted from theomader's source code here: http://dev.theomader.com/cascaded-shadow-mapping-2/
// It is released under the MIT License (https://opensource.org/licenses/mit-license.php)
//
// It has been modified for the Virtices Engine.
//-----------------------------------------------------------------------------
#define NumSplits 4

bool DoShadow = true;

// The Shadow Transforms for each Split
float4x4 ShadowTransform[NumSplits];

// The Shadow Map Size
float ShadowMapSize = 512;

// The Tile Bounds for each Split Viewport
float4 TileBounds[NumSplits];

// The split colours 
float4 SplitColors[NumSplits + 1];

int ShadowBlurStart = 0;

// The Number of Samples for Blending
int numSamples = 4;

// The Poisson Kernel Arrays
//float2 PoissonKernel[12];
float  PoissonKernelScale[NumSplits + 1] = { 1.1f, 1.10f, 1.2f, 1.3f, 0.0f };
float2 pk[6];

// The amount of shade to preform during the cascade shadow mapping.
float ShadowBrightness = 0.25f;



// The Shadow Map 
texture2D ShadowMap;
sampler ShadowMapSampler = sampler_state
{
	texture = <ShadowMap>;
	magfilter = POINT;
	minfilter = POINT;
	mipfilter = POINT;
	AddressU = clamp;
	AddressV = clamp;
};

// The Random Data texture for blending of the shadow edges
texture2D RandomTexture2D;
sampler RandomSampler2D = sampler_state
{
	texture = <RandomTexture2D>;
	magfilter = POINT;
	minfilter = POINT;
	mipfilter = POINT;
	AddressU = wrap;
	AddressV = wrap;
};

struct ShadowSplitInfo
{
	float2 TexCoords;
	float  LightSpaceDepth;
	int    SplitIndex;
};

struct ShadowData
{
	// The Texture Coordinates for the 1st and 2nd cascade Viewports
	float4 TexCoords_0_1;

	// The Texture Coordinates for the 3rd and 4th cascade Viewports
	float4 TexCoords_2_3;

	// An array of holding the depth value for each cascade viewport
	float4 LightSpaceDepth;

	// The world position
	float3 WorldPosition;
};


ShadowData GetShadowData(float4 worldPosition, float4 clipPosition)
{
	ShadowData result;

	// The UV Coordinates for each split.
	float4 texCoords[NumSplits];
	
	// The Light Space Depth for each split
	float lightSpaceDepth[NumSplits];

	// For each split, get the Light Space Depth.
	for (int i = 0; i<NumSplits; ++i)
	{
		float4 lightSpacePosition = mul(worldPosition, ShadowTransform[i]);
		texCoords[i] = lightSpacePosition / lightSpacePosition.w;
		lightSpaceDepth[i] = texCoords[i].z;
	}

	// Now return the UV Coordinates and the Light Space depth for each split
	result.TexCoords_0_1 = float4(texCoords[0].xy, texCoords[1].xy);
	result.TexCoords_2_3 = float4(texCoords[2].xy, texCoords[3].xy);
	result.LightSpaceDepth = float4(lightSpaceDepth[0], lightSpaceDepth[1], lightSpaceDepth[2], lightSpaceDepth[3]);

	// Get the World Position
	result.WorldPosition = worldPosition;
	
	return result;
}




// Returns which Split Index the Pixel is in.
ShadowSplitInfo GetSplitInfo(ShadowData shadowData)
{

	float2 shadowTexCoords[NumSplits] =
	{
		shadowData.TexCoords_0_1.xy,
		shadowData.TexCoords_0_1.zw,
		shadowData.TexCoords_2_3.xy,
		shadowData.TexCoords_2_3.zw
	};

	float lightSpaceDepth[NumSplits] =
	{
		shadowData.LightSpaceDepth.x,
		shadowData.LightSpaceDepth.y,
		shadowData.LightSpaceDepth.z,
		shadowData.LightSpaceDepth.w,
	};

	for (int splitIndex = 0; splitIndex < NumSplits; splitIndex++)
	{
		if (shadowTexCoords[splitIndex].x >= TileBounds[splitIndex].x && shadowTexCoords[splitIndex].x <= TileBounds[splitIndex].y &&
			shadowTexCoords[splitIndex].y >= TileBounds[splitIndex].z && shadowTexCoords[splitIndex].y <= TileBounds[splitIndex].w)
		{
			ShadowSplitInfo result;
			result.TexCoords = shadowTexCoords[splitIndex];
			result.LightSpaceDepth = lightSpaceDepth[splitIndex];
			result.SplitIndex = splitIndex;

			return result;
		}
	}

	ShadowSplitInfo result = { float2(0,0), 0, NumSplits };
	return result;
}


// Returns the Split Colour for that index.
float4 GetSplitIndexColor(ShadowData shadowData)
{
	ShadowSplitInfo splitInfo = GetSplitInfo(shadowData);
	return SplitColors[splitInfo.SplitIndex];
}



// Gets the Shadow Factor Finally
float GetShadowFactor(ShadowData shadowData, float ndotl)
{
	ShadowSplitInfo splitInfo = GetSplitInfo(shadowData);

// Only blend if it's greater than a certain value
/*
	if(splitInfo.SplitIndex > ShadowBlurStart)
	{
		return lerp(ShadowBrightness, 1.0, (splitInfo.LightSpaceDepth < tex2Dlod(ShadowMapSampler, float4(splitInfo.TexCoords, 0, 0)).r));
	}*/
	float2 randomValues = tex2D(RandomSampler2D, shadowData.WorldPosition.xy * 100 * shadowData.WorldPosition.z).rg;
	float2 rotation = randomValues * 2 - 1;

	//float l = saturate(smoothstep(-0.2, 0.2, ndotl));
	float l = saturate(smoothstep(0, 0.2, ndotl));
	float t = smoothstep(randomValues.x * 0.5, 1.0f, l);

	// The Shadow Result
	float result = 1;

	float sampleCount = 6;
	
	for(int i = 0; i < sampleCount; i++)
	{
		//float2 pOffset = float2(rotation.x * pk[i].x - rotation.y * pk[i].y, rotation.y * pk[i].x + rotation.x * pk[i].y)/750;
		float4 randomizedTexCoords = float4(splitInfo.TexCoords + float2(rotation.x * pk[i].x - rotation.y * pk[i].y, rotation.y * pk[i].x + rotation.x * pk[i].y)/750 * PoissonKernelScale[splitInfo.SplitIndex], 0, 0);
		
		// Average the Poissin Rotated Disk Value
		result += splitInfo.LightSpaceDepth <  tex2Dlod(ShadowMapSampler, randomizedTexCoords).r;
	}

	//shdwfactor = result / 4 * t;

	return lerp(ShadowBrightness, 1.0, result / 6.0 * t);
}

float GetShadowFactor(ShadowData shadowData)
{
	return GetShadowFactor(shadowData, 1);
}




//**************************************************
//					Preperation Shader
//**************************************************

/*
	This Technique draws the rendertargets which are used
	in other techniques later on, such as Normal and Depth
	Calculations, as well as Surface data like shadow factors
	and specular factors and distortion. 
	It performs all of this in one pass rendering to multiple 
	render targets at once.
*/

struct PrepVertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal 	: NORMAL0;
    float2 TexCoord	: TEXCOORD0;
    float3 Binormal	: BINORMAL0;
    float3 Tangent	: TANGENT0;
};

struct PrepVertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float3x3 tangentToWorld : TEXCOORD2;
	ShadowData Shadow : TEXCOORD5;
};

struct PrepPSOutput
{
    float4 SurfaceDetail : COLOR0;
	float4 Normal : COLOR1;
    float4 Depth : COLOR2;
};

PrepVertexShaderOutput PrepPassMainVS(in PrepVertexShaderInput input)
{
	PrepVertexShaderOutput output = (PrepVertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz,1), World);

	// calculate the W * V * P in one pass.
	output.Position = mul(float4(input.Position.xyz, 1), wvp);

	// Set UV Coordinates
	output.TexCoord = input.TexCoord * UVFactor + UVOffset;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(input.Tangent, World);
    output.tangentToWorld[1] = mul(input.Binormal, World);
    output.tangentToWorld[2] = mul(input.Normal, World);

	//output.ViewDirection =  normalize(CameraPos - worldPosition);
	// Depth
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

	// Now Get the Shadow Data
	if (DoShadow==true)
	{
		output.Shadow = GetShadowData(worldPosition, output.Position);
	}

	return output;
}

PrepPSOutput PrepPassMainPS(PrepVertexShaderOutput input)
{
    PrepPSOutput output = (PrepPSOutput)0;
	
	//Next get the Specular Attribute from the Specular Map.
	//*********************************************************************************************
	// R: Specular Power
	// G: Specular Intensity
	// B: Reflection Map Factor
	// A: Emissivity (i.e. controls whether shadows are cast on this object. Handy for lights and glowing objects)
	float4 surfaceAttributes = tex2D(surfaceMapSampler, input.TexCoord);
	
	// Get the emissivity glow factor
	float glowFactor = DoEmissiveMapping ? (1-surfaceAttributes.a) * GlowIntensity : 0;

	// Place different deferred rendering factors as different values in the SurfaceDetail variable
	//	R: Cascade Shadow Mapping Factor
	//	G: Emissivity Map
	//	B: Specular Power
	//	A: Specular Intensity
	//*********************************************************************************************    
	// The dot product between a surface normal and the light direction, if it's negative, then the surface should be shaded.
	//float dotl = dot(LightDirection, input.tangentToWorld[2]);
	float shadow = DoShadow ? GetShadowFactor(input.Shadow, dot(LightDirection, input.tangentToWorld[2])) : 1;

	// factor the shadow 
	shadow =  (glowFactor > 0) ? lerp(shadow, 1, glowFactor) : shadow;

	output.SurfaceDetail = float4(shadow, glowFactor, surfaceAttributes.r * SpecularPower, surfaceAttributes.g * SpecularIntensity);

	
	//output.Normal = float4(input.tangentToWorld[2], 0);
	float3 normalFromMap = DoNormalMapping ? mul(2.0f * tex2D(normalSampler, input.TexCoord) - 1.0f, input.tangentToWorld) : input.tangentToWorld[2];

    //normalize the result and output the normal, in [0,1] space
    output.Normal = float4(0.5f * (normalize(normalFromMap) + 1.0f), 0);

	float reflectionData = 0;
	
	//Next, Set the Depth Value
	//*********************************************************************************************
	output.Depth = input.Depth.x / input.Depth.y;
	/*
	if(DoReflections == true)
	{
		reflectionData = surfaceAttributes.b;//tex2D(ReflectionMapSampler, input.TexCoord).r;

		//Calculate View Direction
		//float3 ViewDirection = normalize(CameraPos - input.Shadow.WorldPosition);
		float fFacing = 1.0 - max(dot(input.ViewDirection, normalFromMap), 0);
		float fFresnel = fFresnelBias + (1.0 - fFresnelBias) * pow(abs(fFacing), fFresnelPower);
		reflectionData *= (fFresnel * ReflectionIntensity);
		
		// Get the Reflection Colour
		float3 Reflection = reflect(-input.ViewDirection, normalize(normalFromMap));
		output.ReflClr = lerp(texCUBE(ReflectionSampler, normalize(Reflection)), 0, 1-reflectionData);
		output.ReflClr.a *= reflectionData;
		output.Normal.a = DoSSRefection ? reflectionData : 0;// specularAttributes.a * SpecularPower;
	}
*/



    return output;
}

technique Technique_PrepPass
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL PrepPassMainVS();
		PixelShader = compile PS_SHADERMODEL PrepPassMainPS();
	}
};







//**************************************************
//					DataMask Shader
//**************************************************

/*
	This Technique draws a number of masks which hold differe
	data values such as Motion Blur Masks, Encoded Index
	values for selection and an auxilary depth map.
*/

// The Aux Depth is for testing the edge of water
bool DoAuxDepth = true;

// The Encoded Index Colour
float4 IndexEncodedColour;

// a multi channel porperties color
float4 MaskPropertiesColor;

struct DataMaskVSInput
{
    float4 Position : POSITION0;
    float3 Normal 	: NORMAL0;
    float2 TexCoord	: TEXCOORD0;
    float3 Binormal	: BINORMAL0;
    float3 Tangent	: TANGENT0;
};

struct DataMaskVSOutput
{
	float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    //float3 Normal : TEXCOORD2;
};

struct DataMaskPSOutput
{
    half4 EncodedIndex : COLOR0;
	half4 MaskMap : COLOR1;
    half4 AuxDepth : COLOR2;
    float4 Distortion : COLOR3;
    //half4 AuxDepth : COLOR3;
};

DataMaskVSOutput DataMaskMainVS(in DataMaskVSInput input)
{
	DataMaskVSOutput output = (DataMaskVSOutput)0;

	// calculate the W * V * P in one pass.
	output.Position = mul(float4(input.Position.xyz, 1), wvp);
	// Set UV Coordinates
	output.TexCoord = input.TexCoord * UVFactor + UVOffset;

	// Depth
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;
	
	// Normal	
    //output.Normal = mul(input.Normal, World);

	return output;
}

DataMaskPSOutput DataMaskMainPS(DataMaskVSOutput input)
{
    DataMaskPSOutput output = (DataMaskPSOutput)0;
	
	// The First Render Target is the 
	output.EncodedIndex = IndexEncodedColour;

	// The 2nd Render target is the Mask Properties Color
	output.MaskMap = float4(0, MaskPropertiesColor.g, 0, 0);
	
	//Next, Set the Aux Depth Value for use with items whose thickness should be know (like water edging)
	output.AuxDepth = DoAuxDepth ? input.Depth.x / input.Depth.y : 0;

	// Finally, set any distortion info.
	//*********************************************************************************************
	output.Distortion = DoDistortionMapping ? float4(tex2D(DisplacementMapSampler, input.TexCoord + DistUVOffset).rgb, DistortionScale) : 0;

	
    return output;
}

technique Technique_DataMaskPass
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL DataMaskMainVS();
		PixelShader = compile PS_SHADERMODEL DataMaskMainPS();
	}
};


//**************************************************
//				Cascade Shadow Debug Shader
//**************************************************

/*
	This Technique handles Cascade Shadow Map Debugging.
	It's not in the DebugShader since there are a number
	of functions and structs which are used by the Cascade Shadow
	Mapping, and so the debug will show the same output
	as the main shader which uses it.
*/

bool ShowShadowDebugColours = true;

struct VSShadowDebugIN
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
};

struct VSShadowDebugOutput
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;
	ShadowData Shadow : TEXCOORD1;
};

VSShadowDebugOutput ShadowVS(in VSShadowDebugIN input)
{
	VSShadowDebugOutput output = (VSShadowDebugOutput)0;

	// Apply camera matrices to the input position.
	output.Position = mul(input.Position, wvp);
	float4 worldPosition = mul(float4(input.Position.xyz,1), World);
	output.Normal = mul(input.Normal, World);
	output.Shadow = GetShadowData(worldPosition, output.Position);

	return output;
}

float4 ShadowPS(VSShadowDebugOutput input) : COLOR
{
	float shadow = GetShadowFactor(input.Shadow, dot(LightDirection, input.Normal));
	
	// Get whether to use the Debug Colours or plane white
	float4 colors = ShowShadowDebugColours ? 1 : GetSplitIndexColor(input.Shadow);
	
	// finally output the colour shaded.
	return colors * shadow;
}

technique Technique_ShadowDebug
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL ShadowVS();
		PixelShader = compile PS_SHADERMODEL ShadowPS();
	}
};