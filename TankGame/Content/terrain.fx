float4x4 World;
float4x4 View;
float4x4 Projection;
texture terrainTexture1;

float3 lightDirection;
float4 lightColor;
float lightBrightness;

sampler2D textureSampler = sampler_state {
	Texture = (terrainTexture1);
	AddressU = Wrap;
	AddressV = Wrap;
};struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float4 LightingColor : COLOR0;
};VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.TextureCoordinate = input.TextureCoordinate;
	float4 normal = normalize(mul(input.Normal, World));
	float lightLevel = dot(normal, lightDirection);
	output.LightingColor = saturate(lightColor * lightBrightness * lightLevel);
	return output;
}float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 pixelColor = tex2D(
	textureSampler, input.TextureCoordinate);
	pixelColor *= input.LightingColor;
	pixelColor.a = 1.0;
	return pixelColor;
}
technique Technique1
{
	pass Pass1
	{
		// TODO: set renderstates here.
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}