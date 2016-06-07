//////////////////////
// Constant Buffers //
//////////////////////
cbuffer globalBuffer : register(b0)
{
	matrix worldViewProjection;           // WorldViewProjection Matrix
	float4 color1;                          // Color
	float4 color2;                          // Color
	float4 tileSize;
};

////////////////
// Structures //
////////////////
struct VS_INPUT
{
	float3 inPosition           : POSITION;
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
	output.position = mul(float4(input.inPosition.xy, 1.0f, 1.0f), worldViewProjection);


	return output;
}

//////////////////
// Pixel Shader //
//////////////////
float4 PS(float4 screenPosition : SV_Position) : SV_TARGET
{
	int x = trunc((16000.0 + tileSize.z + screenPosition.x) / tileSize.x);
	int y = trunc((16000.0 + tileSize.w + screenPosition.y) / tileSize.y);
	if((x+y)%2==0)
		return color1;
	else
		return color2;
}