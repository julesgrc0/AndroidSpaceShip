#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VertexShaderOutput
{
	float2 Position : TEXCOORD0;
	float4 Color : COLOR0;
};

sampler2D textureSampler;


float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(textureSampler, input.Position);
	
    float moy = ((color.g + color.b) / 2);
    if(color.r < 0.3)
    {
        color.r = moy;
    }
    else
    {
		color.r = 0.5 + moy;
    }

    return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};