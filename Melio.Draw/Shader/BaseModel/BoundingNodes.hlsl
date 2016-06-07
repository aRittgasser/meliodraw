//////////////////////
// Constant Buffers //
//////////////////////
cbuffer globalBuffer : register(b0)
{
	matrix worldView;           // WorldViewProjection Matrix
	matrix projection;           // WorldViewProjection Matrix
	float4 color;                          // Color
	float4 metrics;
	float4 rect;
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

	float4 pos = float4(rect.x + input.inPosition.z*metrics.z,
						rect.y + input.inPosition.w*metrics.w, 1.0f, 1.0f);

	pos = mul(worldView, pos);
	pos.x += input.inPosition.x*metrics.x;
	pos.y += input.inPosition.y*metrics.y;

	output.position = mul(projection, pos);
	// Compute position in projection space

	return output;
}

//////////////////
// Pixel Shader //
//////////////////
float4 PS() : SV_TARGET
{
	return color;
}