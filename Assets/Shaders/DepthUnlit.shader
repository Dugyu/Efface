Shader "Dugy/DepthUnlit"
{
    Properties
    {


	    _MainTex("Texture", 2D) = "white" {}
		_p1("Param 1", Float) = 1.0
		_p2("Param 2", Float) = 1.0
		_color("Color", Color) = (0.8, 0.45, 0.5,1.0)
		_lightp("Light Position", Vector) = (10.4, 10.6, 10.9, 1.0)

			    
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.718, 0.173, 0.341, 0.75)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.8, 0.45, 0.5, 0.75)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1
 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
		Cull Off //Back | Front | Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _p1;
			float _p2;
			float4 _color;
			float3 _lightp;

			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float _DepthMaxDistance;

			sampler2D _CameraDepthTexture;

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
				
				float4 screenPosition: TEXCOORD6;  //??? SAMPLE CAMERA TEXTURE

				float2 uv : TEXCOORD0;
            };

            //sampler2D _MainTex;
            //float4 _MainTex_ST;

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
				//f.p += f.n *_p1                       //offset point along  world normal

				//projection
                f.screen_p = mul(UNITY_MATRIX_VP, float4(f.p, 1.0));     //=UnityObjectToClipPos  screen space homogenous coordinates
				f.vp_p = f.screen_p;                                     //normalized viewport coords

				//texture coords
                f.uv =  v.uv;      
               

				//SAMPLE CAMERA TEXTURE
				f.screenPosition = ComputeScreenPos(f.screen_p);   //from homogenous to screen resolution

				return f;
            }

            float4 frag (fragmentData f) : SV_Target
            {
				//aliases of input data
				float3 op = f.obj_p;	                //pixel position,  object space
				float3 p = f.p;                         //pixel position,  world space
				float3 sp = f.vp_p.xyz / f.vp_p.w;      //pixel viewport position
				float3 on = normalize(f.obj_n);         //surface normal at pixel in object space
				float3 n = normalize(f.n);				//surface normal at pixel in world space
				float2 uv = f.uv;                       //texture mapping coordinates
				float3 eye = _WorldSpaceCameraPos;      //camera position
				float t = _Time.y;                      //unity time in seconds since start


				float depthNonLinear = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(f.screenPosition)).r;   //????
				float depthLinear = LinearEyeDepth(depthNonLinear);    //when moving further from camera, larger distances are represented by smaller values in the depth buffer.
				
				float depthDifference = depthLinear - f.screenPosition.w;
				float waterDepthDifference = saturate(depthDifference / _DepthMaxDistance);   //clamps inputs from 0.0 to 1.0
				float4 Color = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference);
				 
                return Color;
            }
            ENDCG
        }
    }
}
