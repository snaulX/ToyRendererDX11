#include "Common.hlsli"

struct VSInput
{
	float3 Position : ATTRIBUTE0;
	float4 Color : ATTRIBUTE1;
	float2 TexCoord : ATTRIBUTE2;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TexCoord : TEXCOORD0;
};

struct DrawData
{
	float4x4 world;
};

struct CameraData
{
	float4x4 view;
	float4x4 projection;
	float4x4 viewProjection;
};

ConstantBuffer<CameraData> camera : register(b0);
ConstantBuffer<DrawData> draw : register(b1);

#if defined(BINDLESS) && TODO
struct PushConstants {
	uint dataIndex;
};
PUSH_CONSTANT(PushConstants, constants);
#else
Texture2D DiffuseTexture : register(t0);
#endif

VSOutput VSMain(VSInput input)
{
	float4x4 worldViewProjection = mul(camera.viewProjection, draw.world);
	VSOutput output = (VSOutput)0;
	output.Position = mul(worldViewProjection, float4(input.Position.xyz, 1.0f));
	output.Color = input.Color;
	output.TexCoord = input.TexCoord;
	return output;
}

float4 PSMain(VSOutput input) : SV_TARGET
{
#if defined(BINDLESS) && TODO
	Texture2D DiffuseTexture = Texture2DTable[constants.dataIndex];
#endif
	return DiffuseTexture.Sample(LinearSampler, input.TexCoord) * input.Color;
}