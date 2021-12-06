// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Epitome.Shader/WaterSurface" {
	Properties {
		_ReflectionCubeMap("Reflection CubeMap",Cube) = ""{}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_FoamTex ("Foam Texture ", 2D) = "black" {}
		_FoamGradientTex("Foam Gradient Texture ", 2D) = "white" {}
		_MainColor("Main Color", Color) = (0.3, 0.4, 0.7, 1.0)
		_ReflectionColor("Reflection Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecularIntensity("Specular Intensity", Range (0, 2)) = 1
		_SpecularSharp("Specular Sharp", Float) = 96
		_WaveIntensity("Wave Intensity", Range(0, 1)) = 1.0
		_FoamIntensity("Foam Intensity", Range (0, 1.0)) = 0.75
		_FoamSpeed("Foam Speed", Range (0, 1.0)) = 0.25
		_FoamFadeDepth("Foam Fade Depth", Range (0, 1.0)) = 0.4
		_FoamBrightness("Foam Brightness", Range (0, 2.0)) = 0
		_Force("Wave Speed&Direction", Vector) = (0.5, 0.5, -0.5, -0.5)
	}

	SubShader {
		Tags 
		{ 
			"Queue" = "Geometry+100"
			"IgnoreProjector" = "True"
		}

		Pass 
		{ 
			Lighting Off
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
 
			#include "UnityCG.cginc"
			#pragma multi_compile_fog
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#pragma shader_feature _FOAM_ON
			#pragma shader_feature _RUNTIME_REFLECTIVE
			#pragma shader_feature _RUNTIME_REFRACTIVE
			#pragma multi_compile __ _FANCY_STUFF
 
			struct appdata_water 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
 
			struct v2f 
			{
				float4 pos : POSITION;
				fixed4 color : COLOR;
				float2 uv0 : TEXCOORD0;
				UNITY_FOG_COORDS(1)
#if _FANCY_STUFF
				float2 uvNoise : TEXCOORD2;
				float3 posWorld : TEXCOORD3;
				half3 normal : TEXCOORD4;
	#if _FOAM_ON
				float2 uvFoam : TEXCOORD5;	
	#endif		
	#if _RUNTIME_REFLECTIVE || _RUNTIME_REFRACTIVE
				float4 uvProjector : TEXCOORD6;
	#endif
#endif
			};
		
			uniform fixed4 _MainColor;
#if _FANCY_STUFF
			uniform fixed4 _Force;
			uniform sampler2D _BumpMap;
			float4 _BumpMap_ST;
			uniform fixed _WaveIntensity;
			uniform fixed4 _ReflectionColor;
			uniform fixed _SpecularIntensity;
			uniform half _SpecularSharp;
			half4 _GlobalMainLightDir;
			fixed4 _GlobalMainLightColor;
 
	#if _FOAM_ON
			uniform sampler2D _FoamTex;
			uniform sampler2D _FoamGradientTex;
			float4 _FoamTex_ST;
			uniform fixed _FoamIntensity;
			uniform fixed _FoamSpeed;
			uniform fixed _FoamFadeDepth;
			uniform fixed _FoamBrightness;
	#endif
 
	#if _RUNTIME_REFLECTIVE
			uniform sampler2D _ReflectionTex;
	#else
			uniform samplerCUBE _ReflectionCubeMap;
	#endif
 
	#if _RUNTIME_REFRACTIVE
			uniform sampler2D _RefractionTex;
	#endif
 
			uniform sampler2D _RippleTex;
#endif
			v2f vert (appdata_water v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv0 = v.texcoord;
#if _FANCY_STUFF
				o.uvNoise = TRANSFORM_TEX(v.texcoord, _BumpMap);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.normal = half3(0, 1 - _WaveIntensity, 0);	
	#if _FOAM_ON
				o.uvFoam = TRANSFORM_TEX(v.texcoord, _FoamTex);
	#endif
 
	#if _RUNTIME_REFLECTIVE || _RUNTIME_REFRACTIVE
				o.uvProjector = ComputeScreenPos(o.pos);
	#endif
#endif
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed waterDepth = i.color.a;
#if _FANCY_STUFF
                // noise
				half3 noise = UnpackNormal(tex2D(_BumpMap, i.uvNoise + _Time.xx * _Force.xy));
				noise += UnpackNormal(tex2D(_BumpMap, i.uvNoise + _Time.xx * _Force.zw));
				noise = normalize(noise.xzy) * _WaveIntensity; // 在水平面扰动.
 
				// ripple
				fixed4 ripple = tex2D(_RippleTex, i.uv0) * 2;
				half3 normalNoise = normalize(i.normal + noise + ripple.xyz);
 
				// fresnel
				half3 viewDir = normalize(_WorldSpaceCameraPos - i.posWorld);
				half fresnel = 1 - saturate(dot(viewDir, normalNoise));
				fresnel = 0.25 + fresnel * 0.75;
 
				// reflection
	#if _RUNTIME_REFLECTIVE
				float4 uv1 = i.uvProjector; uv1.xy += noise.xy * 0.25;
				fixed4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(uv1)) * _ReflectionColor;
	#else		
				half3 worldReflectionVector = normalize(reflect(-viewDir, normalNoise));
				fixed4 refl = texCUBE(_ReflectionCubeMap, worldReflectionVector) * _ReflectionColor;			
	#endif
				// refractive
	#if _RUNTIME_REFRACTIVE
				float4 uv2 = i.uvProjector; uv2.xy += noise.xy * 0.5;
				fixed4 refr = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(uv2));
	#else
				fixed4 refr = _MainColor;
	#endif
 
				fixed4 finalColor = lerp(refr, refl, fresnel);
 
	#if _FOAM_ON
				// foam
				half foamFactor = 1 - saturate(waterDepth / _FoamIntensity); 
				half foamGradient = 1 - tex2D(_FoamGradientTex, half2(foamFactor - _Time.y * _FoamSpeed, 0) + normalNoise.xy).r;
				foamFactor *= foamGradient;
				half4 foam = tex2D(_FoamTex, i.uvFoam + normalNoise.xy);
				finalColor += foam * foamFactor; 
    #endif
				// specular
				half3 h = normalize(_GlobalMainLightDir.xyz + viewDir);
				half nh = saturate(dot(noise, h));
				nh = pow(nh, _SpecularSharp);
				finalColor += _GlobalMainLightColor * nh * _SpecularIntensity;
 
				// alpha
	#if _FOAM_ON
				half factor = step(_FoamFadeDepth, waterDepth);
				half newDepth = waterDepth / _FoamFadeDepth;
				finalColor.a = _MainColor.a * waterDepth + foamFactor * _FoamBrightness * (factor +  newDepth * (1 - factor));
	#else
				finalColor.a = _MainColor.a * waterDepth;
	#endif
#else
				fixed4	finalColor = _MainColor;
				finalColor.a *= waterDepth;
#endif
		
				UNITY_APPLY_FOG(i.fogCoord, finalColor);
				return finalColor;
			}
 
			ENDCG
		} 
 
		// 没用Unity自带的阴影,只是用来来渲染_CameraDepthsTexture.
		Pass
		{
			Tags { "LightMode" = "ShadowCaster" }
 
			Fog { Mode Off }
			ZWrite On 
			Offset 1, 1
 
			CGPROGRAM
 
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
 
			struct v2f
			{
				V2F_SHADOW_CASTER;
			};
 
			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}
 
			fixed4 frag(v2f i) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
 
			ENDCG
		}
	}
	FallBack "Diffuse"
}
