Shader "Dugy/TerrainUnlit"
{
    Properties
    {  
		_MainTex ("MainTex", 2D) = "white" {}
		_SecondTex("SecondTex", 2D) = "white"{}

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

				return tex2D(_MainTex,i.uv) + tex2D(_SecondTex,i.uv);

			}

		ENDCG
		}

    }
    FallBack "Diffuse"
}
