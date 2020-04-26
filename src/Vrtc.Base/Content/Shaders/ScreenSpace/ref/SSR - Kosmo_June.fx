
//Screen Space Reflection shader TheKosmonaut 2016

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  VARIABLES
////////////////////////////////////////////////////////////////////////////////////////////////////////////

//#include "helper.fx"

float4x4 Projection;
float4x4 InverseProjection;

float3 FrustumCorners[4]; //In Viewspace!

float FarClip;

float Time = 0;

const int Samples = 3;
const int SecondarySamples = 3;

const float MinimumThickness = 70;

const float border = 0.1f;
float2 resolution = float2(1280, 800);

//Texture2D DepthMap;
//Texture2D TargetMap;
//Texture2D NormalMap;
//Texture2D NoiseMap;
//
//SamplerState texSampler
//{
//	Texture = (AlbedoMap);
//	AddressU = CLAMP;
//	AddressV = CLAMP;
//	MagFilter = LINEAR;
//	MinFilter = LINEAR;
//	Mipfilter = POINT;
//};


// and/or sketch filter are being applied over the top of.
texture ColorMap;
sampler ColorSampler : register(s0) = sampler_state
{
	Texture = (ColorMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
};



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

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  STRUCTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct VertexShaderInput
{
	float3 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 ViewRay : TEXCOORD1;
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  FUNCTIONS
////////////////////////////////////////////////////////////////////////////////////////////////////////////

float3 GetFrustumRay(float2 texCoord)
{
	float index = texCoord.x + (texCoord.y * 2);
	return FrustumCorners[index];
}

float3 encode(float3 n)
{
	return   0.5f * (n + 1.0f);
}

float3  decode(float3 n)
{
	return 2.0f * n.xyz - 1.0f;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  VERTEX SHADER
////////////////////////////////////////////////////////////////////////////////////////////////////////////



VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = float4(input.Position, 1);
	output.TexCoord = input.TexCoord;

	output.ViewRay = GetFrustumRay(input.TexCoord);
	return output;

}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  PIXEL SHADER
////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  HELPER FUNCTIONS
////////////////////////////////////////////////////////////////////////////////////////////////////////////

float3 randomNormal(float2 tex)
{
	tex = frac(tex * Time);
	float noiseX = (frac(sin(dot(tex, float2(15.8989f, 76.132f) * 1.0f)) * 46336.23745f));
	float noiseY = (frac(sin(dot(tex, float2(11.9899f, 62.223f) * 2.0f)) * 34748.34744f));
	float noiseZ = (frac(sin(dot(tex, float2(13.3238f, 63.122f) * 3.0f)) * 59998.47362f));
	return normalize(float3(noiseX, noiseY, noiseZ));
}

float3 GetFrustumRay2(float2 texCoord)
{
	float3 x1 = lerp(FrustumCorners[0], FrustumCorners[1], texCoord.x);
	float3 x2 = lerp(FrustumCorners[2], FrustumCorners[3], texCoord.x);
	float3 outV = lerp(x1, x2, texCoord.y);
	return outV;
}

float TransformDepth(float depth, matrix trafoMatrix)
{
	return (depth*trafoMatrix._33 + trafoMatrix._43) / (depth * trafoMatrix._34 + trafoMatrix._44);
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Main Raymarching algorithm
////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Basic
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	const float border2 = 1 - border;
const float bordermulti = 1 / border;
float samples = abs(Samples);

float4 output = 0;

//Just a bit shorter
float2 texCoord = float2(input.TexCoord);

//Get our current Position in viewspace
float linearDepth = tex2Dlod(DepthSampler, float4(texCoord, 0, 0)).r;// DepthMap.Sample(texSampler, texCoord).r;
float3 positionVS = input.ViewRay * linearDepth; //GetFrustumRay2(texCoord) * linearDepth;

												 //Sample the normal map
float4 normalData = tex2D(NormalSampler, texCoord);// NormalMap.Sample(texSampler, texCoord);
//tranform normal back into [-1,1] range
float3 normal = decode(normalData.xyz);
float roughness = normalData.a;

//Exit if material is not reflective
//[branch]
if (normalData.x + normalData.y <= 0.001f || roughness > 0.8f) //Out of range
{
	return float4(0, 0, 0, 0);
}

//Where does our ray start, where does it go?
float3 incident = normalize(positionVS);
float3 reflectVector = reflect(incident, normal);

//Transform to WVP to get the uv's for our ray
float4 reflectVectorVPS = mul(float4(positionVS + reflectVector, 1), Projection);
reflectVectorVPS.xyz /= reflectVectorVPS.w;

//transform to UV coordinates
float2 reflectVectorUV = 0.5f * (float2(reflectVectorVPS.x, -reflectVectorVPS.y) + float2(1, 1));

// raymarch, transform depth to z/w depth so we march equal distances on screen (no perspective distortion)
float3 rayOrigin = float3(texCoord, TransformDepth(positionVS.z, Projection));
float3 rayStep = float3(reflectVectorUV - texCoord, reflectVectorVPS.z - rayOrigin.z);

//extend the ray so it crosses the whole screen once
float xMultiplier = (rayStep.x > 0 ? (1 - texCoord.x) : -texCoord.x) / rayStep.x;
float yMultiplier = (rayStep.y > 0 ? (1 - texCoord.y) : -texCoord.y) / rayStep.y;
float multiplier = min(xMultiplier, yMultiplier) / samples;
rayStep *= multiplier;

/*//uniform raystep distance
if (Samples < 0)
{
float length2 = length(rayStep.xy);
rayStep /= length2 * samples;
}*/

//Some variables we need later when precising the hit point
float startingDepth = rayOrigin.z;
float3 hitPosition;
float2 hitTexCoord = 0;

float temporalComponent = 0;

//[branch]
if (Time > 0)
{
	temporalComponent = frac(Time * 10000 * rayStep.x / rayOrigin.y / rayStep.z); // frac(sin(Time * 3.2157) * 46336.23745f);
}

//Add some noise
float noise = 0;// NoiseMap.Sample(texSampler, frac(((texCoord)* resolution) / 64) + temporalComponent).r; // + frac(input.TexCoord* Projection)).r;

																									  //Raymarching the depth buffer
//[loop]
for (int i = 1; i <= samples; i++)
{
	//March a step
	float3 rayPosition = rayOrigin + (i - 0.5f + noise)*rayStep;

	//We don't consider rays coming out of the screeen
	if (rayPosition.z < 0 || rayPosition.z>1) break;

	//Get the depth at our new position
	int3 texCoordInt = int3(rayPosition.xy * resolution, 0);

	float linearDepth = tex2Dlod(DepthSampler, float4(rayPosition.xy * resolution, 0, 0)).r * -FarClip;// DepthMap.Load(texCoordInt).r * -FarClip;
	float sampleDepth = TransformDepth(linearDepth, Projection);

	float depthDifference = sampleDepth - rayPosition.z;

	//needs normal looking to it!

	//Coming towards us, let's go back to linear depth!
	//[branch]
	if (rayStep.z < 0 && depthDifference < 0)
	{
		//Where are we currently in linDepth, note - in VS is + in VPS
		float depthMinusThickness = TransformDepth(linearDepth + MinimumThickness, Projection);

		if (depthMinusThickness < rayPosition.z)
			continue;
	}

	//March backwards, idea -> binary searcH?
	//[branch]
	if (depthDifference <= 0 && sampleDepth >= startingDepth - rayStep.z*0.5f) //sample < rayPosition.z
	{
		hitPosition = rayPosition;

		//Less samples when already going far ... is this good?
		int samples2 = SecondarySamples;//samples + 1 - i;

		bool hit = false;

		float sampleDepthFirstHit = sampleDepth;
		float3 rayPositionFirstHit = rayPosition;

		//March backwards until we are outside again
		//[loop]
		for (int j = 1; j <= samples2; j++)
		{
			rayPosition = hitPosition - rayStep * j / (samples2);

			texCoordInt = int3(rayPosition.xy * resolution, 0);

			sampleDepth = TransformDepth(tex2Dlod(DepthSampler, float4(rayPosition.xy * resolution, 0, 0)).r * -FarClip - 50.0f, Projection);

			//Looks like we don't hit anything any more?
			//[branch]
			if (sampleDepth >= rayPosition.z)
			{
				//only z is relevant

				float origin = rayPositionFirstHit.z;

				//should be smaller
				float r = rayPosition.z - origin;

				//y = r * x + c, c = 0
				//y = (b-a)*x + a
				float a = sampleDepthFirstHit - origin;

				float b = sampleDepth - origin;

				float x = (a) / (r - b + a);

				float sampleDepthLerped = lerp(sampleDepth, sampleDepthFirstHit, x);

				hit = true;

				hitTexCoord = lerp(rayPosition.xy, rayPositionFirstHit.xy, x);

				hitTexCoord = rayPosition.xy;

				////In front
				//if (sampleDepthFirstHit <= rayPositionFirstHit.z - rayStep.z*j/samples2)
				//{
				//	hit = false;
				//}

				break;
			}

			sampleDepthFirstHit = sampleDepth;
			rayPositionFirstHit = rayPosition;
		}

		//We haven't hit anything we can travel further
		if (!hit)
			continue;

		int3 hitCoordInt = int3(hitTexCoord.xy * resolution, 0);

		//float4 albedoColor = TargetMap.Load(hitCoordInt);
		output.rgb = tex2D(ColorSampler, hitTexCoord.xy * resolution);
		output.a = 1;

		//Fade out to the edges
		//[branch]
		if (rayPosition.y > border2)
		{
			output.a = lerp(1, 0, (hitTexCoord.y - border2) * bordermulti);
		}
		else if (rayPosition.y < border)
		{
			output.a = lerp(0, 1, hitTexCoord.y * bordermulti);
		}
		//[branch]
		if (rayPosition.x > border2)
		{
			output.a *= lerp(1, 0, (hitTexCoord.x - border2) * bordermulti);
		}
		else if (rayPosition.x < border)
		{
			output.a *= lerp(0, 1, hitTexCoord.x * bordermulti);
		}

		//Fade out to the front

		float fade = saturate(1 - reflectVector.z);
		output.a *= (1 - roughness) * fade;
		//output.rgb *= output.a;

		break;
	}
	startingDepth = rayPosition.z;
}

return output;
}

technique Default
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}