Shader "Custom/TextureDistortion"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_CheckerBoardColor("CheckerBoard Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
		_TextureDistortionColor("Texture Distortion Color", Color) = (1,1,1,1)
		_FireTex("Fire Texture", 2D) = "white" {}
		_DissolveTexture("Disolve Texture", 2D) = "white" {}
		_CheckerBox("CheckerBox Texture", 2D) = "white" {}
		[NoScaleOffset] _FlowMap("Flow (RG) A Noise", 2D) = "black" {}

		_SpecialLightingTexture("Lighting Texture", 2D) = "white" {}
		_SecondFlowMapForLighting("Second Flow Map for Lighting", 2D) = "black" {}

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_UJump("U jump per phase", Range(-0.25, 0.25)) = 0.25
		_VJump("V jump per phase", Range(-0.25, 0.25)) = 0.25
		_LightLerp("Light Lerp", Range(0,1)) = 0.5
		_DisolveAmount("Dissolve Threshold", Range(0,1)) = 0.25
		_CheckerBoardThreshold("Checkerboard Threshold", Range(0,1)) = 0.5
		_NeonLerp("Neon Lerp", Range(0,1)) = 0.5
		_NeonSpeed("NeonSpeed", Float) = 0.5

		_DissolveVal("Dissolve Value", Range(-0.2, 1.2)) = 1.2
		_LineWidth("Line Width", Range(0.0, 0.2)) = 0.1
		_LineColor("Line Color", Color) = (1.0, 1.0, 1.0, 1.0)

		_DistortUV("Distort UV", Range(-5,5)) = 1

		[PowerSlider(4)] _FresnelExponent("Fresnel Exponent", Range(0.25, 4)) = 1
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)

		[HDR] _Emission("Emission", color) = (0,0,0)

	}
		SubShader
		{
			Tags {"Render" = "Opaque" "Queue" = "Geometry"}
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			// #include "Flow.cginc"

			sampler2D _MainTex;
			sampler2D _FireTex; 
			sampler2D _FlowMap;
			sampler2D _DissolveTexture;
			sampler2D _CheckerBox;
			sampler2D _SecondFlowMapForLighting;
			sampler2D _SpecialLightingTexture;
			float _UJump, _VJump;
			float _LightLerp;
			float _CheckerBoardThreshold; 
			float _NeonLerp; 
			float _NeonSpeed; 

			float4 _LineColor;
			float _DissolveVal;
			float _LineWidth;
			float _DistortUV;


			struct Input
			{
				float2 uv_MainTex;
				float2 uv_FireTex; 
				float2 uv_DissolveTexture;
				float2 uv_CheckerBox;
				float2 uv_SecondFlowMapForLighting;
				float3 worldSpaceViewDir;
				INTERNAL_DATA
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			fixed4 _CheckerBoardColor;
			fixed4 _fresnelColor;
			fixed3 _Emission;
			fixed4 _TextureDistortionColor; 

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			// #if !defined(FLOW_INCLUDED)
			// #define FLOW_INCLUDED
		
			float3 FlowUVW(float2 uv, float2 flowVector, float time, float2 jump, bool flowB) {
				float phaseOffset = flowB ? 0.5 : 0; 
				float progress = frac(time + phaseOffset);
				// can be substitutded with a lerp or smoothstep 


				float3 uvw;
				uvw.xy = uv - flowVector * progress + phaseOffset; // xy value
				uvw.xy += (time - progress) * jump;
				uvw.z = 1 - abs(1 - 2 * progress);

				return uvw;
			}

		
			// #endif 

			void surf (Input IN, inout SurfaceOutputStandard o)
			{
				float2 flowVector = tex2D(_FlowMap, IN.uv_FireTex).rg * 2 - 1;
				float2 secondFlowVectorLighting = tex2D(_SecondFlowMapForLighting, IN.uv_SecondFlowMapForLighting).rg * 2 - 1; 
				float noise = tex2D(_FlowMap, IN.uv_FireTex).a;

				// half dissolve_value = tex2D(_DissolveTexture, IN.uv_DissolveTexture).r; //Get how much we have to dissolve based on our dissolve texture
				// clip(dissolve_value - _DisolveAmount);
				half4 dissolve = tex2D(_DissolveTexture, IN.uv_DissolveTexture * _DistortUV);

				half4 clear = half4(0,0,0,0);

				int isClear = int(dissolve.r - (_DissolveVal + _LineWidth) + 0.99);
				int isAtLeastLine = int(dissolve.r - (_DissolveVal)+0.99);

				half4 altCol = lerp(_LineColor, clear, isClear);
			

				float time = _Time.y + noise; 
				float2 jump = float2(_UJump, _VJump);

				float3 uvwA = FlowUVW(IN.uv_MainTex, flowVector, time, jump, false);
				float3 uvwB = FlowUVW(IN.uv_MainTex, flowVector, time, jump, true);

				fixed4 texA = tex2D(_FireTex, uvwA.xy) * uvwA.z;
				fixed4 texB = tex2D(_FireTex, uvwB.xy) * uvwB.z; 
			
				float3 uvLightingA = FlowUVW(IN.uv_SecondFlowMapForLighting, secondFlowVectorLighting, time, jump, false);
				float3 uvLightingB = FlowUVW(IN.uv_SecondFlowMapForLighting, secondFlowVectorLighting, time, jump, true);


				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);			
				fixed4 checkerBoardTexture = tex2D(_CheckerBox, IN.uv_CheckerBox + _Time.y * _NeonSpeed) * _CheckerBoardColor;

				// half dissolve_value_checkers = checkerBoardTexture.b; //Get how much we have to dissolve based on our dissolve texture
				// clip(dissolve_value_checkers - _DisolveAmount);

				fixed4 lerpedTexture = lerp((texA + texB), checkerBoardTexture, _LightLerp) * _TextureDistortionColor;
				fixed3 combinedLighting = uvLightingA + uvLightingB;
				// checkerBoardTexture = clamp(tex2D(_DissolveTexture, IN.uv_DissolveTexture), 1, checkerBoardTexture);
				// fixed4 lerpedCheckerBoardTexture = lerp(lerpedTexture, checkerBoardTexture, saturate(sign(_CheckerBoardThreshold - tex2D(_DissolveTexture, IN.uv_MainTex))));

				//lerpedCheckerBoardTexture = lerp(lerpedTexture, lerpedCheckerBoardTexture, _NeonLerp);
						
				fixed4 lerpedCheckerBoardTexture = checkerBoardTexture + mainTex;
				fixed4 c = (lerpedCheckerBoardTexture);

				o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
				// o.Albedo = altCol; 
				o.Albedo = lerp(o.Albedo, altCol, isAtLeastLine) * mainTex * _Color + step(altCol,0) * (lerpedTexture);
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
