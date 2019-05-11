Shader "Dugy/TerrainUnlit"
{
    Properties
    {  
		_MainTex ("Albedo", 2D) = "white" {}
		_SecondTex("Albedo", 2D) = "white"{}
        _Color ("Color", Color) = (0,0,0,0)
		_Player("Player Postion", vector) = (0,0,0,0)
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Cull Off //Back | Front | Off


		Pass
		{
			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct VertInput{
				float4 pos: POSITION;
				float2 uv: TEXCOORD0;
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SecondTex;
			float4 _SecondTex_ST;
			half4 _Color;

			struct VertOutput{
				float4 pos: SV_POSITION;
				float2 uv: TEXCOORD0;
			};

			VertOutput vert(VertInput i){
				VertOutput o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				return o;
			}


			half4 frag(VertOutput i): COLOR{

				return _Color +  tex2D(_MainTex,i.uv) + tex2D(_SecondTex,i.uv);

			}

		ENDCG
		}

    }
    FallBack "Diffuse"
}
