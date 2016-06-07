
//////////////////////
// Constant Buffers //
//////////////////////
cbuffer globalBuffer : register(b0)
{
	matrix worldViewProjection;           // WorldViewProjection Matrix
	float4 color;                          // Color
	float4 texPos;
};

////////////////
// Structures //
////////////////
struct VS_INPUT
{
	float4 inPosition           : POSITION;
};

struct VS_OUTPUT
{
	float4 position             : SV_POSITION;
	float2 tex					: TEXCOORD0;
};

/////////////
// Texture //
/////////////
Texture2D Texture;
SamplerState SampleType;



///////////////////
// Vertex Shader //
///////////////////
VS_OUTPUT VS(VS_INPUT input)
{
	VS_OUTPUT output;

	output.position = mul(float4(input.inPosition.xy, 1.0, 1.0), worldViewProjection);
	output.tex = float2(texPos.x+input.inPosition.z*texPos.z, texPos.y+ input.inPosition.w*texPos.w);


	return output;
}

//////////////////
// Pixel Shader //
//////////////////
float4 PS(VS_OUTPUT input) : SV_TARGET
{
	float4 texColor = Texture.Sample(SampleType, input.tex);
	//return texColor;
	return float4(color.xyz, (1.0-texColor.x)*color.w);
}