
//////////////////////
// Constant Buffers //
//////////////////////
cbuffer globalBuffer : register(b0)
{
	matrix worldViewProjection;           // WorldViewProjection Matrix
	float4 color1;                          // Color
	float4 color2;                          // Color
	float4 workingRect;
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
	float2 work					: TEXCOORD0;
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
	output.work = float2(input.inPosition.z, input.inPosition.w);


	return output;
}

//////////////////
// Pixel Shader //
//////////////////
float4 PS(VS_OUTPUT input) : SV_TARGET
{
	// Left Side
	if (input.work.y > 0.0)
	{
		if (input.position.y > workingRect.y && input.position.y < workingRect.x)
			return color1;
		else
			return color2;
	}
// Top Side
	else if (input.work.x > 0.0) {
		if (input.position.x<workingRect.w && input.position.x > workingRect.z)
			return color1;
		else
			return color2;
	}
	return color1;
}