﻿//////////////////////
// Constant Buffers //
//////////////////////
cbuffer globalBuffer : register(b0)
{
	matrix worldViewProjection;           // WorldViewProjection Matrix
	float4 color;                          // Color
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
	output.position = mul(worldViewProjection, float4(input.inPosition.xy, 1.0f, 1.0f));


	return output;
}

//////////////////
// Pixel Shader //
//////////////////
float4 PS() : SV_TARGET
{
	return color;
}