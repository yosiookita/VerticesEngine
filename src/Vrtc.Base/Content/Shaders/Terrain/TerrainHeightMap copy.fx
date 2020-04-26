float4x4 world;
float4x4 view;
float4x4 proj;

float4x4 wvp;

float maxHeight = 92;
float3 CameraPos;

// Texture UV Scale
float TxtrUVScale = 16;

bool IsEditMode = false;


float textureSize = 256.0f;
float2 CursorPosition;
float CursorScale = 1;

float4 CursorColour = float4(0, 0.25, 1, 1);

float4 SelectionColour;

texture displacementMap;
sampler displacementSampler = sampler_state
{
    Texture   = <displacementMap>;
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

texture textureWeightMap;
sampler textureWeightSampler = sampler_state
{
	Texture = <textureWeightMap>;
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture CursorMap;
sampler CursorMapSampler = sampler_state
{
	Texture = <CursorMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};




texture Texture01;
sampler sandSampler = sampler_state
{
	Texture = <Texture01>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture Texture02;
sampler grassSampler = sampler_state
{
	Texture = <Texture02>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture Texture03;
sampler rockSampler = sampler_state
{
	Texture = <Texture03>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture Texture04;
sampler snowSampler = sampler_state
{
	Texture = <Texture04>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};





struct VS_INPUT {
    float4 position	: POSITION;
    float4 uv : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 position  : POSITION;
    float4 uv : TEXCOORD0;
    float4 worldPos : TEXCOORD1;
    float4 textureWeights : TEXCOORD2;
};





//float4 SelectionColour = float4(0, 0, 0, 1);

//--------------------------- TOON SHADER PROPERTIES ------------------------------
// The color to draw the lines in.  Black is a good default.
float4 LineColor = float4(0, 0, 0, 1);

// The thickness of the lines.  This may need to change, depending on the scale of
// the objects you are drawing.
float LineThickness = .3;

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
	float4 original = mul(input.Position, wvp);

	// Calculates the normal of the vertex like it ought to be.
	float4 normal = mul(input.Normal, wvp);

	float thk = length(input.Position.xyz - CameraPos) / 400;

	// Take the correct "original" location and translate the vertex a little
	// bit in the direction of the normal to draw a slightly expanded object.
	// Later, we will draw over most of this with the right color, except the expanded
	// part, which will leave the outline that we want.
	output.Position = original + (mul(thk, normal));

	return output;
}

// The pixel shader for the outline.  It is pretty simple:  draw everything with the
// correct line color.
float4 OutlinePixelShader(VertexToPixel input) : COLOR0
{
	return SelectionColour;// float4(SelectionColour.rgb, 1);
}





 
VS_OUTPUT TerrainVS(VS_INPUT In)
{
	//initialize the output structure
	VS_OUTPUT Out = (VS_OUTPUT)0;  

	// Calculate World Position
	float4 worldPosition = mul(float4(In.position.xyz, 1), world);
										
	Out.position = mul(float4(In.position.xyz, 1), wvp);
	
	// with the newly read height, we compute the new value of the Y coordinate
	// we multiply the height, which is in the (0,1) range by a value representing the Maximum Height of the Terrain
	//In.position.y = height * maxHeight;

	//Pass the world position the the Pixel Shader
	Out.worldPos = worldPosition;// mul(In.position, world);

	//Compute the final projected position by multiplying with the world, view and projection matrices                                                      
	//Out.position = mul(In.position, worldViewProj);
	Out.uv = In.uv;


	// this instruction reads from the heightmap, the value at the corresponding texture coordinate
	// Note: we selected level 0 for the mipmap parameter of tex2Dlod, since we want to read data exactly as it appears in the heightmap
	float height = 1;// tex2Dlod(displacementSampler, float4(In.uv.xy, 0, 0));
	height = Out.worldPos.y / maxHeight;

	//height = tex2Dlod(textureWeightSampler, float4(In.uv.xy, 0, 0)).r;
	//height = tex2D(textureWeightSampler, In.uv.xy).r;

	float4 TexWeights = 0;

	TexWeights.x = saturate(1.0f - abs(height - 0) / 0.25f);
	TexWeights.y = saturate(1.0f - abs(height - 0.3) / 0.25f);
	TexWeights.z = saturate(1.0f - abs(height - 0.6) / 0.25f);
	TexWeights.w = min(1, saturate(1.0f - abs(height - 0.9) / 0.25f));
	float totalWeight = TexWeights.x + TexWeights.y + TexWeights.z + TexWeights.w;
	TexWeights /= totalWeight;
	Out.textureWeights = TexWeights;
	
	return Out;
}

float4 TerrainPS(VS_OUTPUT input) : COLOR0
{ 
	
	float4 color = 1;

	float cursorFactor;
	//if (IsEditMode==true)
	//{
		cursorFactor = tex2D(CursorMapSampler, (input.uv.xy * textureSize - (CursorPosition - float2(CursorScale / 2, CursorScale / 2))) / CursorScale).a;

		color = lerp(color, CursorColour, cursorFactor);
	//}

	float4 weights = input.textureWeights;

	// Get the UV Scale
	float2 uv = input.uv.xy * TxtrUVScale * 4;

	float4 sand = weights.x > 0 ? tex2D(sandSampler,uv) : 0;
	float4 grass = weights.y > 0 ? tex2D(grassSampler,uv) : 0;
	float4 rock = weights.z > 0 ? tex2D(rockSampler,uv) : 0;
	float4 snow = weights.w > 0 ? tex2D(snowSampler,uv) : 0;

	color = sand * weights.x + grass * weights.y + rock * weights.z + snow * weights.w;
	color.a = 1;

	return  lerp(color, CursorColour, cursorFactor);
	
}

technique Terrain
{

    pass P0
    {
        VertexShader = compile vs_3_0 TerrainVS();
        PixelShader  = compile ps_3_0 TerrainPS();
    }
}
