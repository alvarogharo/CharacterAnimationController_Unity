Shader "FGC/Volund Morph3D-Variants/Std Character (Specular, Surface)" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_AlphaTex("Alpha", 2D) = "white" {}
		
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_SpecColor("Specular", Color) = (0.2,0.2,0.2)
		_SpecGlossMap("Specular", 2D) = "white" {}

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}
		
		_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
		_ParallaxMap ("Height Map", 2D) = "black" {}
		
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}
		
		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}
		
		_DetailMask("Detail Mask", 2D) = "white" {}
		
		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0

		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
		
		// Volund properties
		[HideInInspector] _SmoothnessInAlbedo ("__smoothnessinalbedo", Float) = 0.0
		[HideInInspector] _SmoothnessTweaks ("__smoothnesstweaks", Vector) = (1,0,0,0)
		_SmoothnessTweak1("Smoothness Scale", Range(0.0, 2.0)) = 1.0
		_SmoothnessTweak2("Smoothness Bias", Range(-1.0, 1.0)) = 0.0
		_SpecularMapColorTweak("Specular Color Tweak", Color) = (1,1,1,1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
//		Tags { "RenderType"="Opaque" "Special"="Wrinkles" "PerformanceChecks"="False" }

		LOD 300
		
		Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]

		CGPROGRAM
		// - Physically based Standard lighting model, specular workflow
		// - 'fullforwardshadows' to enable shadows on all light types
		// - 'addshadow' to ensure alpha test works for depth/shadow passes
		// - 'keepalpha' to allow alpha blended output options
		// - 'interpolateview' because that's what the non-surface Standard does
		// - Custom vertex function to setup detail UVs as expected by Standard shader (and also workaround bug)
		// - Custom finalcolor function to output controlled final alpha
		// - 'nolightmap' and 'nometa' since this shader is only for dynamic objects
		// - 'exclude_path:prepass' since we have no use for this legacy path
		// - 'exclude_path:deferred' because unique shadows are currently forward only
		#pragma surface SurfSpecular StandardSpecular fullforwardshadows addshadow keepalpha interpolateview vertex:FixClothVertex finalcolor:StandardSurfaceSpecularFinal nolightmap nometa exclude_path:deferred exclude_path:prepass

		// Use shader model 3.0 target, to get nicer looking lighting (PBS toggles internally on shader model)
		#pragma target 3.0

		// This shader probably works fine for console/mobile platforms as well, but 
		// these are the ones we've actually tested.
		#pragma only_renderers d3d11 d3d9 opengl
		
		// Standard shader feature variants
		#pragma shader_feature _NORMALMAP
		#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
		#pragma shader_feature _SPECGLOSSMAP
		#pragma shader_feature _DETAIL_MULX2
		
		// Standard, but unused in this project
		//#pragma shader_feature _EMISSION
		//#pragma shader_feature _PARALLAXMAP
		
		// Volund additional variants
		#pragma shader_feature SMOOTHNESS_IN_ALBEDO
//		#pragma multi_compile _ WRINKLE_MAPS
		#pragma multi_compile _ UNIQUE_SHADOW UNIQUE_SHADOW_LIGHT_COOKIE
						
		// Volund uniforms
		uniform sampler2D	_NormalAndOcclusion;
		uniform half3 		_SpecularMapColorTweak;
		uniform half2		_SmoothnessTweaks;
		uniform sampler2D 	_AlphaTex;
		
		
		

		// Include unique shadow functions
//		#include "../UniqueShadow/UniqueShadow_ShadowSample.cginc"

		// Need screen pos if wrinkle maps are active
//#ifdef WRINKLE_MAPS
//		struct SurfaceInput {
//			float4	texcoord;
//	#ifdef _PARALLAXMAP
//			half3	viewDir;
//	#endif
//			float4	screenPos;
//		};
//		#define Input SurfaceInput
//#endif

		// Include all the Standard shader surface helpers
		#include "UnityStandardSurface.cginc"

		// The cloth component has a bug where is writes zero-length normals for
		// vertices that aren't simulated. Patch that up here until the engine fix
		// lands in an official build (expected in 5.1.x)
		void FixClothVertex (inout appdata_full v, out Input o)
		{
			// Invoke the actual vertex mod function
			StandardSurfaceVertex (v, o);
			
			// Test for valid normal
			if(dot(v.normal, v.normal) < 0.01f)
				v.normal = float3(0.f, 1.f, 0.f);
		}
		
		// Our main surface entry point
		void SurfSpecular(Input IN, inout SurfaceOutputStandardSpecular o)
		{
			StandardSurfaceSpecular(IN, o);

			// Apply specular color tweak if we sampled spec color from a texture
#ifdef _SPECGLOSSMAP
			o.Specular *= _SpecularMapColorTweak;
#endif

			o.Alpha =  tex2D(_AlphaTex, IN.texcoord.xy).r;//use our custom alpha map
	
#if defined(_ALPHATEST_ON)
	clip(o.Alpha - _Cutoff);
#endif

			// Optionally sample smoothness from albedo texture alpha channel instead of sg texture
#ifdef SMOOTHNESS_IN_ALBEDO
			o.Smoothness = tex2D(_MainTex, IN.texcoord.xy).a;
#endif
			// Apply smoothness scale and bias (always active)
			o.Smoothness = saturate(o.Smoothness * _SmoothnessTweaks.x + _SmoothnessTweaks.y);

			// Sample occlusion and normals from screen-space buffer when wrinkle maps are active
//#ifdef WRINKLE_MAPS
//			float3 normalOcclusion = tex2D(_NormalAndOcclusion, IN.screenPos.xy / IN.screenPos.w).rgb;
//			o.Occlusion = normalOcclusion.r;
//	#ifdef _NORMALMAP
//			o.Normal.xy = normalOcclusion.gb * 2.f - 1.f;
//			o.Normal.z = sqrt(saturate(1.f - dot(o.Normal.xy, o.Normal.xy)));
//	#endif
//#endif
			o.Alpha = tex2D(_AlphaTex, IN.texcoord.xy).r;


		}
		
		

		
		
		ENDCG
	}
	
	CustomEditor "VolundMultiStandardShaderGUI"
	FallBack "Diffuse"
}