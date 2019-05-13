Shader "Dugy/BasicColorUnlit"
{
       Properties
    {
	    _MainTex("MainTex", 2D) = "white" {}
		_SecondTex("SecondTex", 2D) = "white" {}
		_p1("Param 1", Float) = 1.0
		_p2("Param 2", Float) = 1.0
		_p3("Transparency", Range(0, 1)) = 0.5
		_color("Color", Color) = (0.4, 0.6, 0.9, 1.0)
		_lightp("Light Position", Vector) = (10.4, 10.6, 10.9, 1.0)
 
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
		//Cull Off //Back | Front | Off

		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SecondTex;
			float4 _SecondTex_ST;
			float _p1;
			float _p2;
			float _p3;
			float4 _color;
			float3 _lightp;

			//Data read from mesh
            struct meshVertexData
            {
                float4 p : POSITION;
				float4 n : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct fragmentData
            {
                float3 obj_n : TEXCOORD1;
				float3 n : TEXCOORD2;
				
				float3 obj_p : TEXCOORD3;
				float3 p : TEXCOORD4;
				
				float4 vp_p : TEXCOORD5;    // viewport coordinates
				float4 screen_p : SV_POSITION;   

				float2 uv : TEXCOORD0;
            };



            fragmentData vert (meshVertexData v)
            {
                fragmentData f;

				//vertex position
				f.obj_p = v.p;                          //object space
				f.p = mul(unity_ObjectToWorld, v.p);    //world space

				//vertex normal
				f.obj_n = v.n;                           //object space
				f.n = mul(unity_ObjectToWorld, v.n);    //world space

				//distortion  [optional]
				f.p += sin(f.p*_p1+ _Time.y)*_p2;                  //offset point along  world normal

				//projection
                f.screen_p = mul(UNITY_MATRIX_VP, float4(f.p, 1.0));     //screen space
				f.vp_p = f.screen_p;                                     //normalized viewport coords

				//texture coords
                f.uv =  v.uv;      
                return f;
            }

            fixed4 frag (fragmentData f) : SV_Target
            {
				//aliases of input data
				float3 op = f.obj_p;	                //pixel position,  object space
				float3 p = f.p;                         //pixel position,  world space
				float3 sp = f.vp_p.xyz / f.vp_p.w;      //pixel viewport position
				float3 on = normalize(f.obj_n);         //surface normal at pixel in object space
				float3 n = normalize(f.n);				//surface normal at pixel in world space
				//float2 uv = f.uv;                     //texture mapping coordinates
				float2 uvTransparency = f.uv;            // Transparency
				float2 uv = float2(0.5+p.x / 1024, 0.5 - p.z/1024);
				
				float3 eye = _WorldSpaceCameraPos;      //camera position
				float t = _Time.y;                      //unity time in seconds since start

				float d=length(eye-p);
				float4 color = _color;
				//color.rgb *=d*0.1f;
				color = float4(0.0, 0.0, 0.0, 1.0);
				color += tex2D(_MainTex, uv) + tex2D(_SecondTex,uv) * 0.5 + d * 0.0005f;
				color.a = uvTransparency.x;
                return color;
            }
            ENDCG
        }
    }
}
