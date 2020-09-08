Shader "Sprites/Foliage-Deformation"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_Radius ("Radius", Float) = 1
		_Height ("Height", Float) = 1

		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
			"DisableBatching"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVertLocal
			#pragma fragment SpriteFrag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "FDInclude.cginc"

			fixed _Radius, _Height, _WindSpeed;
			int _AffectorObjectCount;
			Vector _FoliageAffectorObjects[512];

			// Material Color.
			fixed4 _Color;

			v2f SpriteVertLocal(appdata_t IN)
			{
			    v2f OUT;

			    UNITY_SETUP_INSTANCE_ID (IN);
			    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

			#ifdef UNITY_INSTANCING_ENABLED
			    IN.vertex.xy *= _Flip.xy;
			#endif

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
			    OUT.texcoord = IN.texcoord;
			    OUT.color = IN.color * _Color * _RendererColor;

			    //position
				fixed2 _Pos = mul(unity_ObjectToWorld, float4(0,0,0,1)).xy;

				fixed _OffsetX = 0;
				fixed _OffsetY = 0;

				for(int i = 0; i < _AffectorObjectCount; i++){

					//direction
					fixed _TempOffsetX = _Pos.x - _FoliageAffectorObjects[i].x;
					fixed _TempOffsetY = _Pos.y - _FoliageAffectorObjects[i].y;

					//distance
					fixed _Dist = abs(max(min(_Radius, abs(_TempOffsetX)), min(_Radius, abs(_TempOffsetY))) - _Radius);

					//mod
					_TempOffsetX = (_Radius / 12.0f) * ((_TempOffsetX / (pow(_TempOffsetX, 2) + 0.25f))); 
					_TempOffsetY = (_Radius / 8.0f) * ((_TempOffsetY / (pow(_TempOffsetY, 2) + 0.25f)));

					//clamp
					_OffsetX += lerp (0, _TempOffsetX, _Dist);
					_OffsetY += lerp (0, _TempOffsetY, _Dist);
				}

				//add wind
				_OffsetX += (((sin((_Time.y * _WindSpeed) + (_Pos.x / 2.5f) + (_Pos.y / 5.0f)) + 1.0f) / 2.0f) * (_Radius / 24.0f)) / max(1, (_WindSpeed / 8.0f)) ;

				//deform top vertexs
				OUT.vertex += fixed4(_OffsetX, _OffsetY, 0, 0) * IN.texcoord.y;
			    #ifdef PIXELSNAP_ON
			    OUT.vertex = UnityPixelSnap (OUT.vertex);
			    #endif

			    return OUT;
			}

		ENDCG
		}
	}
}