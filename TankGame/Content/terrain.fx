float4x4 World;
float4x4 View;
float4x4 Projection;

texture terrainTexture1;

sampler2D textureSampler = sampler_state {
	Texture = (terrainTexture1);
	AddressU = Wrap;
	AddressV = Wrap;
};struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// TODO: add your pixel shader code here.
	return tex2D(textureSampler, input.TextureCoordinate);
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