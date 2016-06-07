//////////////////////
// Constant Buffers //
//////////////////////
cbuffer globalBuffer : register(b0)
{
	matrix worldViewProjection;           // WorldViewProjection Matrix
	float4 color;                          // Color
	float4 metrics;
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
};

///////////////////
// Vertex Shader //
///////////////////
VS_OUTPUT VS(VS_INPUT input)
{
	VS_OUTPUT output;

	// Compute position in projection space
	float4 pos = float4(input.inPosition.x*metrics.x + input.inPosition.z*metrics.z,
		input.inPosition.y*metrics.y + input.inPosition.w*metrics.w, 1.0f, 1.0f);

	output.position = mul(pos, worldViewProjection);

	return output;
}

//////////////////
// Pixel Shader //
//////////////////
float4 PS() : SV_TARGET
{
	return color;
}