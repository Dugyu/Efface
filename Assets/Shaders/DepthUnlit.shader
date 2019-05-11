Shader "Dugy/DepthUnlit"
{
    Properties
    {

		// basic
	    _MainTex("Texture", 2D) = "white" {}
		_p1("Param 1", Float) = 1.0
		_p2("Param 2", Float) = 1.0
		_color("Color", Color) = (0.8, 0.45, 0.5,1.0)
		_lightp("Light Position", Vector) = (10.4, 10.6, 10.9, 1.0)

		// depth color
		_DepthGradientShallow("Depth Gradient Shallow", Color) = (0.718, 0.173, 0.341, 0.75)
		_DepthGradientDeep("Depth Gradient Deep", Color) = (0.8, 0.45, 0.5, 0.75)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1
		
		// noise texture
		_SurfaceNoise("Surface Noise", 2D) = "white" {}
		_SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777

		// shoreline
		_FoamMaxDistance("Foam Max Distance", Float) = 5
		_FoamMinDistance("Foam Min Distance", Float) = 0.04
		_FoamColor("Foam Color", Color) = (1,1,1,1)
		// animation
		_SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)

		// distortion
		_SurfaceDistortion("Surface Distortion", 2D) = "white" {}	
		_SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.27

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 100
		Cull Off //Back | Front | Off

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
			#define SMOOTHSTEP_AA 0.01
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _p1;
			float _p2;
			float4 _color;
			float3 _lightp;

			// normal blending
			float4 alphaBlend(float4 top, float4 bottom)
			{
				float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float alpha = top.a + bottom.a * (1 - top.a);
				return float4(color, alpha);
			}






			//Data read from mesh
            struct meshVertexData
            {
                float4 p : POSITION;
				float4 n : NORMAL;
                float2 uv : TEXCOORD0;
            };
			
			// depth
			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float _DepthMaxDistance;
			sampler2D _CameraDepthTexture;

			// noise
			sampler2D _SurfaceNoise;
			float4 _SurfaceNoise_ST;
			float _SurfaceNoiseCutoff;

			// shoreline
			float _FoamMaxDistance;
			float _FoamMinDistance;
			float4 _FoamColor;

			// animation
			float2 _SurfaceNoiseScroll;

			// distortion
			sampler2D _SurfaceDistortion;
			float4 _SurfaceDistortion_ST;
			float _SurfaceDistortionAmount;

			// normalize foam
			sampler2D _CameraNormalsTexture;


            struct fragmentData
            {
			
				float2 uv : TEXCOORD0;

                float3 obj_n : TEXCOORD1;
				float3 n : TEXCOORD2;
				float3 viewNormal: TEXCOORD9;
				
				float3 obj_p : TEXCOORD3;
				float3 p : TEXCOORD4;
				
				float4 vp_p : TEXCOORD5;    // viewport coordinates
				float4 screen_p : SV_POSITION;   
				
				float4 screenPos : TEXCOORD6;  //Full scale, not normalized screen space, used to SAMPLE CAMERA TEXTURE

				float2 noiseUV : TEXCOORD7;
				float2 distortUV : TEXCOORD8;

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
				f.viewNormal = UnityObjectToViewPos(v.n);
				//distortion  [optional]
				//f.p += f.n *_p1                       //offset point along  world normal

				//projection
                f.screen_p = mul(UNITY_MATRIX_VP, float4(f.p, 1.0));     //=UnityObjectToClipPos  screen space homogenous coordinates
				f.vp_p = f.screen_p;                                     //normalized viewport coords

				//texture coords
                f.uv =  v.uv;      
				f.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
				f.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
				
				//SAMPLE CAMERA DEPTH TEXTURE
				f.screenPos = ComputeScreenPos(f.screen_p);   //from homogenous to screen resolution


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

				// depth
				float depthNonLinear = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(f.screenPos)).r;   //"r" means rgba  , also can use:  float depth = tex2D(_CameraDepthTexture, i.screenPos.xy / i.screenPos.w).r;
				float depthLinear = LinearEyeDepth(depthNonLinear);    //when moving further from camera, larger distances are represented by smaller values in the depth buffer.
				

				float depthDifference = depthLinear - f.screenPos.w;
				float waterDepthDifference = saturate(depthDifference / _DepthMaxDistance);   //clamps inputs from 0.0 to 1.0
				// color transition
				float4 Color = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference);

				// animation
				float2 distortSample = (tex2D(_SurfaceDistortion, f.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;
				float2 noiseUV = float2((f.noiseUV.x + t * _SurfaceNoiseScroll.x) + distortSample.x, (f.noiseUV.y + t * _SurfaceNoiseScroll.y) + distortSample.y);  

				// noise
				float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;
				
				//normalize shoreline
				float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(f.screenPos));
				float3 normalDot = saturate(dot(existingNormal, f.viewNormal));
				float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);				

				// shoreline
				float foamNoiseDepth = saturate(depthDifference / foamDistance);
				float surfaceNoiseCutoff = foamNoiseDepth * _SurfaceNoiseCutoff;  // the shallower, the smaller cutoff, the more light pattern
				// cutoff
				float surfaceNoise = 0.5f * smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);

				// foam Color
				float4 surfaceNoiseColor = _FoamColor;
				surfaceNoiseColor.a *= surfaceNoise;
                return alphaBlend(surfaceNoiseColor, Color);
            }
            ENDCG
        }
    }
}
