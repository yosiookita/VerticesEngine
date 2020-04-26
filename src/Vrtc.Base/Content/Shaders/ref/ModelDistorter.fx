


//			Main Properties
//*********************************************************************************************
float4x4 World;
float4x4 View;
float4x4 Projection;
float Alpha = 1;

// common distortion parameters
float4x4 WorldViewProjection;
float4x4 WorldView;
float DistortionScale;
float Time;

bool UseTwoDisplacementMaps;

// Texture UV Coordinate Offset
float2 TextureUVOffset = float2(0, 0);

float2 DistortionMapUVOffset = float2(0, 0);
float2 DistortionMapUVOffset2 = float2(0, 0);

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




texture2D DepthMap;
sampler DepthMapSampler : register(s2) = sampler_state
{
	texture = <DepthMap>;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};


//-----------------------------------------------------------------------------
//
// Displacement Mapping
//
//-----------------------------------------------------------------------------

struct PositionTextured
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD;
	float2 Depth : TEXCOORD1;
};

PositionTextured TransformAndTexture_VertexShader(PositionTextured input)
{
	PositionTextured output;

	output.Position = mul(input.Position, WorldViewProjection);
	output.TexCoord = input.TexCoord;
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

texture2D DisplacementMap;
sampler2D DisplacementMapSampler = sampler_state
{
	texture = <DisplacementMap>;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture2D SecondaryDisplacementMap;
sampler2D SecondaryDisplacementMapSampler = sampler_state
{
	texture = <SecondaryDisplacementMap>;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 Textured_PixelShader(PositionTextured input) : COLOR
{
	float4 color = tex2D(DisplacementMapSampler, input.TexCoord + DistortionMapUVOffset);

	float4 final = color;

	if (UseTwoDisplacementMaps == true)
	{
		float4 color2 = tex2D(SecondaryDisplacementMapSampler, input.TexCoord + DistortionMapUVOffset2);

		final = lerp(color, color2, 0.5);
	}
	float Depth = input.Depth.x / input.Depth.y;


	// Use the blue channel as Depth
	return float4(final.rg, 0, final.a) * DistortionScale;
}

technique DisplacementMapped
{
	pass
	{
		VertexShader = compile vs_3_0 TransformAndTexture_VertexShader();
		PixelShader = compile ps_3_0 Textured_PixelShader();
	}
}


//-----------------------------------------------------------------------------
//
// Heat-Haze Displacement
//
//----------------------------------------------------------------------------- 

struct PositionPosition
{
	float4 Position : POSITION;

	// the pixel shader does not have direct access to the position of the pixel
	// being shaded, so we must pass this information through from the vertex shader
	float4 PositionAsTexCoord : TEXCOORD;
};

PositionPosition TransformAndCopyPosition_VertexShader(float4 position : POSITION)
{
	PositionPosition output;

	output.Position = mul(position, WorldViewProjection);
	output.PositionAsTexCoord = output.Position;

	return output;
}

float4 HeatHaze_PixelShader(float4 position : TEXCOORD) : COLOR
{
	float2 displacement;
displacement.x = sin(position.x / 60 + Time * 1.5) * sin(position.x / 10) *
cos(position.x / 50);
displacement.y = sin(position.y / 50 - Time * 2.75);
displacement *= DistortionScale;
displacement = (displacement + float2(1, 1)) / 2;

return float4(displacement, 0, 1);
}

technique HeatHaze
{
	pass
	{
		VertexShader = compile vs_3_0 TransformAndCopyPosition_VertexShader();
		PixelShader = compile ps_3_0 HeatHaze_PixelShader();
	}
}


//-----------------------------------------------------------------------------
//
// Pull-In Displacement
//
//-----------------------------------------------------------------------------

struct PositionNormal
{
	float4 Position : POSITION;
	float3 Normal : NORMAL;
};

struct PositionDisplacement
{
	float4 Position : POSITION;
	float2 Displacement : TEXCOORD;
};

PositionDisplacement PullIn_VertexShader(PositionNormal input)
{
	PositionDisplacement output;

	output.Position = mul(input.Position, WorldViewProjection);
	float3 normalWV = mul(input.Normal, WorldView);
	normalWV.y = -normalWV.y;

	float amount = dot(normalWV, float3(0, 0, 1)) * DistortionScale;
	output.Displacement = float2(.5, .5) + float2(amount * normalWV.xy);

	return output;
}

float4 DisplacementPassthrough_PixelShader(float2 displacement : TEXCOORD) : COLOR
{
	return float4(displacement, 0, 1);
}

technique PullIn
{
	pass
	{
		VertexShader = compile vs_3_0 PullIn_VertexShader();
		PixelShader = compile ps_3_0 DisplacementPassthrough_PixelShader();
	}
}


//-----------------------------------------------------------------------------
//
// Zero Displacement (provided for reference)
//
//-----------------------------------------------------------------------------


float4 TransformOnly_VertexShader(float4 position : POSITION) : POSITION
{
	return mul(position, WorldViewProjection);
}

float4 ZeroDisplacement_PixelShader() : COLOR
{
	return float4(.5, .5, 0, 0);
}

technique ZeroDisplacement
{
	pass
	{
		VertexShader = compile vs_3_0 TransformOnly_VertexShader();
		PixelShader = compile ps_3_0 ZeroDisplacement_PixelShader();
	}
}


//-----------------------------------------------------------------------------
//
// Test
//
//-----------------------------------------------------------------------------


float4 TESTVS(float4 position : POSITION) : POSITION
{
	return mul(position, WorldViewProjection);
}

float4 TESTPS() : COLOR
{
	return float4(0.5, 0.5, 0, 0);
}

technique test
{
	pass
	{
		VertexShader = compile vs_3_0 TESTVS();
		PixelShader = compile ps_3_0 TESTPS();
	}
}




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
	float2 TexCoord	: TEXCOORD0;
};

struct MainVSOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};


MainVSOutput MainVSFunction(MainVSInput input)
{
	MainVSOutput output;


	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord + TextureUVOffset;

	return output;
}



// Pixel shader applies a cartoon shading algorithm.
float4 MainPSFunction(MainVSOutput input) : COLOR0
{
	//First, Get the Diffuse Colour of from the Texture
	//*********************************************************************************************

	float4 col1 = tex2D(diffuseSampler, input.TexCoord);
	float4 col2 = tex2D(diffuseSampler, float2(input.TexCoord.x*0.5, 1-input.TexCoord.y));
	return (col1 + col2/4) * Alpha;

}

technique Technique_Main
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MainVSFunction();
		PixelShader = compile ps_3_0 MainPSFunction();
	}
}