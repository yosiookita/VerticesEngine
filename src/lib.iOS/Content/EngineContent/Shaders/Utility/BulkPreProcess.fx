

//			Main Properties
//*********************************************************************************************
float4x4 World;
//float4x4 View;
//float4x4 Projection;

// The World * View * Projection matrix. This is pre computed on the CPU to allow for quicker 
// Vertex Shader Speed.
float4x4 wvp;

// Texture UV Coordinate Offset
float2 UVOffset = float2(0, 0);

// UV Factor to keep apparent texture position the same when plane scaling (i.e. water scaling outwards)
float2 UVFactor = float2(1, 1);


// Texture UV Coordinate Offset
float2 DistUVOffset = float2(0, 0);
// UV Factor to keep apparent texture position the same when plane scaling (i.e. water scaling outwards)
float2 DistUVFactor = float2(1, 1);

// The Primary Colour for the model for debugging
float4 PrimaryColour = float4(0.1, 0.1, 0.1, 1);

// The light direction is shared between the Lambert and Toon lighting techniques.
//*********************************************************************************************
float3 LightDirection = normalize(float3(1, 1, 1));
float4 AmbientLight = float4(0.5, 0.5, 0.5, 1);
float4 SelectionColour = float4(0, 0, 0, 0);
float SpecularIntensity = 1;
float SpecularPower = 5;

// Texture Samplers Variables
//*********************************************************************************************

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
bool ShadowDebug = false;

// The Shadow Transforms for each Split
float4x4 ShadowTransform[NumSplits];

// The Shadow Map Size
float ShadowMapSize = 512;

// The Tile Bounds for each Split Viewport
float4 TileBounds[NumSplits];

// The split colours 
float4 SplitColors[NumSplits + 1];


// The Number of Samples for Blending
int numSamples = 4;

// The Poisson Kernel Arrays
float2 PoissonKernel[12];
float  PoissonKernelScale[NumSplits + 1] = { 1.1f, 1.10f, 1.2f, 1.3f, 0.0f };

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

//texture3D RandomTexture3D;
//sampler RandomSampler3D = sampler_state
//{
//	texture = <RandomTexture3D>;
//	magfilter = POINT;
//	minfilter = POINT;
//	mipfilter = POINT;
//	AddressU = wrap;
//	AddressV = wrap;
//	AddressW = wrap;
//};

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



struct ShadowSplitInfo
{
	float2 TexCoords;
	float  LightSpaceDepth;
	int    SplitIndex;
};

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
	float2 randomValues = tex2D(RandomSampler2D, shadowData.WorldPosition.xy*50).rg;
	float2 rotation = randomValues * 2 - 1;

	//float l = saturate(smoothstep(-0.2, 0.2, ndotl));
	float l = saturate(smoothstep(0, 0.2, ndotl));
	float t = smoothstep(randomValues.x * 0.5, 1.0f, l);

	ShadowSplitInfo splitInfo = GetSplitInfo(shadowData);

	//return lerp(ShadowBrightness, 1.0, (splitInfo.LightSpaceDepth < tex2Dlod(ShadowMapSampler, float4(splitInfo.TexCoords, 0, 0)).r));
	//return lerp(ShadowBrightness, 1.0, splitInfo.LightSpaceDepth <  tex2D(ShadowMapSampler, splitInfo.TexCoords).r);

	float result = 1;

	for (int s = 0; s<numSamples; ++s)
	{
		float2 poissonOffset = float2(
			rotation.x * PoissonKernel[s].x - rotation.y * PoissonKernel[s].y,
			rotation.y * PoissonKernel[s].x + rotation.x * PoissonKernel[s].y
			);

		const float4 randomizedTexCoords = float4(splitInfo.TexCoords + poissonOffset * PoissonKernelScale[splitInfo.SplitIndex] * 0.5f, 0, 0);
		
		// 
		result += splitInfo.LightSpaceDepth <  tex2Dlod(ShadowMapSampler, randomizedTexCoords).r;
	}

	float shadowFactor = result / numSamples * t;
	return lerp(ShadowBrightness, 1.0, shadowFactor);

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
	Calculations, Mask for God Rays. It performs all of 
	this in one pass rendering to multiple render targets 
	at once.
*/

// Vertex shader input structure.
struct PrepVSInput
{
    float4 Position : POSITION0;
    float3 Normal 	: NORMAL0;
    float2 TexCoord	: TEXCOORD0;
    float3 Binormal	: BINORMAL0;
    float3 Tangent	: TANGENT0;
};

struct PrepVSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float3x3 tangentToWorld : TEXCOORD2;
	ShadowData Shadow : TEXCOORD5;
};

struct PrepPSOutput
{
    half4 SurfaceDetail : COLOR0;
	half4 Normal : COLOR1;
    half4 Depth : COLOR2;
	half4 Distortion : COLOR3;
};





PrepVSOutput PrepPassVSFunction(PrepVSInput input, float4x4 worldTransform)
{
    PrepVSOutput output;

    float4 worldPosition = mul(float4(input.Position.xyz,1), worldTransform);
    //float4 viewPosition = mul(worldPosition, View);
    //output.Position = mul(viewPosition, Projection);
	// calculate the W * V * P in one pass.
	output.Position = mul(float4(input.Position.xyz, 1), wvp);

	// Set UV Coordinates
	output.TexCoord = input.TexCoord * UVFactor + UVOffset;


    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(input.Tangent, worldTransform);
    output.tangentToWorld[1] = mul(input.Binormal, worldTransform);
    output.tangentToWorld[2] = mul(input.Normal, worldTransform);

	// Now Get the Shadow Data
	if (DoShadow==true)
	{
		output.Shadow = GetShadowData(worldPosition, output.Position);
	}

    return output;
}


PrepVSOutput PrepPassVSFunctionInstancedVS( PrepVSInput input, float4x4 instanceTransform : TEXCOORD2)
{
	float4x4 worldTransform = mul( transpose(instanceTransform), World  );
	return PrepPassVSFunction(input, worldTransform); 
}

PrepVSOutput PrepPassVSFunctionNonInstVS( PrepVSInput input )
{
	return PrepPassVSFunction(input, World);
}




PrepPSOutput PrepPassPSFunction(PrepVSOutput input)
{
    PrepPSOutput output = (PrepPSOutput)0;


	//Next get the Specular Attribute from the Specular Map.
	//*********************************************************************************************
	float4 specularAttributes = tex2D(specularSampler, input.TexCoord);


	float reflectionData = tex2D(ReflectionMapSampler, input.TexCoord).r;
	
	// Place different deferred rendering factors as different values in the SurfaceDetail variable
	//	R: Cascade Shadow Mapping Factor
	//	G: Reflection Map
	//	B: Specular Power
	//	A: Specular Intensity
	//*********************************************************************************************    
	// The dot product between a surface normal and the light direction, if it's negative, then the surface should be shaded.
	//float dotl = dot(LightDirection, input.tangentToWorld[2]);
	float shadow = DoShadow ? GetShadowFactor(input.Shadow, dot(LightDirection, input.tangentToWorld[2])) : 1;
	output.SurfaceDetail = float4(shadow, reflectionData, specularAttributes.a * SpecularPower, specularAttributes.r * SpecularIntensity);

	

	//
	//Thirdly, get the Normal from both the Gemoetry and any supplied Normal Maps.
	//*********************************************************************************************
    // read the normal from the normal map
    float3 normalFromMap = tex2D(normalSampler, input.TexCoord);
    //tranform to [-1,1]
    normalFromMap = 2.0f * normalFromMap - 1.0f;
    //transform into world space
    normalFromMap = mul(normalFromMap, input.tangentToWorld);
    //normalize the result
    normalFromMap = normalize(normalFromMap);
    //output the normal, in [0,1] space
    output.Normal.rgb = 0.5f * (normalFromMap + 1.0f);
	    
	// Output the Reflection Data here as it's need in the SSR shader.
	output.Normal.a = reflectionData;// specularAttributes.a * SpecularPower;



	//Next, Set the Depth Value
	//*********************************************************************************************
	output.Depth = input.Depth.x / input.Depth.y;


	// Finally, set any distortion info.
	//*********************************************************************************************
	float3 distColor = tex2D(DisplacementMapSampler, input.TexCoord + DistUVOffset).rgb;
	//float disScale = DistortionScale *(1 - output.Depth / 10);
	output.Distortion = float4(distColor, DistortionScale);

    return output;
}

technique Technique_PrepPass
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 PrepPassVSFunctionNonInstVS();
        PixelShader = compile ps_3_0 PrepPassPSFunction();
    }
}
technique Technique_PrepPass_Instanced
{
    pass Pass0
    {
		VertexShader = compile vs_3_0 PrepPassVSFunctionInstancedVS();
        PixelShader = compile ps_3_0 PrepPassPSFunction();
    }
}







//--------------------------- TOON SHADER PROPERTIES ------------------------------
// The color to draw the lines in.  Black is a good default.
float4 LineColor = float4(0, 0, 0, 1);

// The thickness of the lines.  This may need to change, depending on the scale of
// the objects you are drawing.
float LineThickness = .03;

//--------------------------- DATA STRUCTURES ------------------------------
// The structure used to store information between the application and the
// vertex shader
struct AppToVertex
{
	float4 Position : POSITION0;            // The position of the vertex
	float3 Normal : NORMAL0;                // The vertex's normal
	float2 TextureCoordinate : TEXCOORD0;    // The texture coordinate of the vertex
};

// The structure used to store information between the vertex shader and the
// pixel shader
struct VertexToPixel
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : TEXCOORD1;
};

// The vertex shader that does the outlines
VertexToPixel OutlineVertexShader(AppToVertex input)
{
	VertexToPixel output = (VertexToPixel)0;

	// Calculate where the vertex ought to be.  This line is equivalent
	// to the transformations in the CelVertexShader.
	//float4 original = mul(mul(mul(input.Position, World), View), Projection);
	float4 original = mul(input.Position, wvp);

	// Calculates the normal of the vertex like it ought to be.
	float4 normal = mul(input.Normal, wvp);

	// Take the correct "original" location and translate the vertex a little
	// bit in the direction of the normal to draw a slightly expanded object.
	// Later, we will draw over most of this with the right color, except the expanded
	// part, which will leave the outline that we want.
	output.Position = original + (mul(LineThickness, normal));

	return output;
}

// The pixel shader for the outline.  It is pretty simple:  draw everything with the
// correct line color.
float4 OutlinePixelShader(VertexToPixel input) : COLOR0
{
	return SelectionColour;
}

technique Technique_Outline
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 OutlineVertexShader();
		PixelShader = compile ps_3_0 OutlinePixelShader();
	}
}







bool DoTexture = false;
bool DoDebugWireFrame = false;
float3 DiffuseLight = 0.5;
float4 WireColour = float4(0.1, 0.1, 0.1, 1);

struct DebugWireVSInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct DebugWireVSOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float LightAmount : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
};

DebugWireVSOutput DebugWireVSFunction(DebugWireVSInput input)
{
	DebugWireVSOutput output;


	// Apply camera matrices to the input position.
	output.Position = mul(input.Position, wvp);

	// Copy across the input texture coordinate.
	output.TextureCoordinate = input.TextureCoordinate + UVOffset;

	// Compute the overall lighting brightness.
	float3 worldNormal = mul(input.Normal, World);

	output.LightAmount = dot(worldNormal, LightDirection);
	output.WorldNormal = worldNormal;// input.Normal;


	if (DoDebugWireFrame == true)
	{
		// Calculate where the vertex ought to be.  This line is equivalent
		// to the transformations in the CelVertexShader.
		float4 original = mul(input.Position, wvp);

		// Calculates the normal of the vertex like it ought to be.
		float4 normal = mul(input.Normal, wvp);

		// Take the correct "original" location and translate the vertex a little
		// bit in the direction of the normal to draw a slightly expanded object.
		// Later, we will draw over most of this with the right color, except the expanded
		// part, which will leave the outline that we want.
		output.Position = original + (mul(.05, normal));
	}

	return output;
}

float4 DebugWirePSFunction(DebugWireVSOutput input) : COLOR0
{
	if(DoDebugWireFrame==true)
		return WireColour * 0.5;

	if (DoTexture==true)
		return tex2D(diffuseSampler, input.TextureCoordinate);
	
	float4 color = DoDebugWireFrame ? WireColour : float4(0.1,0.1,0.1,1);
	color = DoTexture ? tex2D(diffuseSampler, input.TextureCoordinate) + PrimaryColour : color;
	color.rgb *= saturate(input.LightAmount) * DiffuseLight + AmbientLight;
	return color;
}

technique Technique_DebugWire
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 DebugWireVSFunction();
		PixelShader = compile ps_3_0 DebugWirePSFunction();
	}
}



bool DoOnlyShadow = false;
bool DoBlockTexture = false;

struct DebugCascadeShadowMapVSInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
};

struct DebugCascadeShadowMapVSOutput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float LightAmount : TEXCOORD0;
	ShadowData Shadow : TEXCOORD1;
};

DebugCascadeShadowMapVSOutput DebugCascadeShadowMapVSFunction(DebugCascadeShadowMapVSInput input)
{
	DebugCascadeShadowMapVSOutput output;


	// Apply camera matrices to the input position.
	//output.Position = mul(mul(mul(input.Position, World), View), Projection);

	float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
	//float4 viewPosition = mul(worldPosition, View);
	//output.Position = mul(viewPosition, Projection);

	output.Position = mul(input.Position, wvp);

	output.LightAmount = dot(mul(input.Normal, World), LightDirection);
	
	output.Normal = input.Normal;

	output.Shadow = GetShadowData(worldPosition, output.Position);

	return output;
}

float4 DebugCascadeShadowMapPSFunction(DebugCascadeShadowMapVSOutput input) : COLOR0
{
	float4 Color = 0;
	//float dotl = dot(LightDirection, input.Normal);
	float shadow = GetShadowFactor(input.Shadow, 1);

	if (DoOnlyShadow == true)
	{
		Color = float4(0.65,0.9,0.9,1) * (0.25 + (input.LightAmount));
		Color.rgb *= shadow;
	}
	else
	{
		Color = GetSplitIndexColor(input.Shadow) * shadow;
	}
	return Color;
}

technique Technique_DebugCascadeShadowMap
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 DebugCascadeShadowMapVSFunction();
		PixelShader = compile ps_3_0 DebugCascadeShadowMapPSFunction();
	}
}



