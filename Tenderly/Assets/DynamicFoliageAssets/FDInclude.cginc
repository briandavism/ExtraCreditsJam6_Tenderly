// Upgrade NOTE: upgraded instancing buffer 'PerDrawSprite' to new syntax.

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef UNITY_SPRITES_INCLUDED
#define UNITY_SPRITES_INCLUDED

#include "UnityCG.cginc"

#ifdef UNITY_INSTANCING_ENABLED

    UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
        // SpriteRenderer.Color while Non-Batched/Instanced.
        fixed4 unity_SpriteRendererColorArray[UNITY_INSTANCED_ARRAY_SIZE];
        // this could be smaller but that's how bit each entry is regardless of type
        float4 unity_SpriteFlipArray[UNITY_INSTANCED_ARRAY_SIZE];
    UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

    #define _RendererColor unity_SpriteRendererColorArray[unity_InstanceID]
    #define _Flip unity_SpriteFlipArray[unity_InstanceID]

#endif // instancing

CBUFFER_START(UnityPerDrawSprite)
#ifndef UNITY_INSTANCING_ENABLED
    fixed4 _RendererColor;
    float4 _Flip;
#endif
    float _EnableExternalAlpha;
CBUFFER_END

struct appdata_t
			{
			    float4 vertex   : POSITION;
			    float4 color    : COLOR;
			    float2 texcoord : TEXCOORD0;
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			};

struct v2f
{
    float4 vertex   : SV_POSITION;
    fixed4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};

sampler2D _MainTex;
sampler2D _AlphaTex;

fixed4 SampleSpriteTexture (float2 uv)
{
    fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
    fixed4 alpha = tex2D (_AlphaTex, uv);
    color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif

    return color;
}

fixed4 SpriteFrag(v2f IN) : SV_Target
{
    fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
    c.rgb *= c.a;
    return c;
}

#endif // UNITY_SPRITES_INCLUDED
