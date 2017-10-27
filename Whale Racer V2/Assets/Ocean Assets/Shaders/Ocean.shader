// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Mobile/Ocean" {
	Properties {
	    _SurfaceColor ("SurfaceColor", Color) = (1,1,1,1)
	    _WaterColor ("WaterColor", Color) = (1,1,1,1)
		_ShadowTex("Cookie", 2D) = "" { TexGen ObjectLinear }
		_FogColor("FogColor", Color) = (0.05, 0.15, 0.47, 0.96)

		// Shadow Stuff
		_Cutoff("Alpha Cutoff", Range(2.0,1.0)) = 0.5
		//_MainTex("Albedo", 2D) = "white" {}
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}

		_Specularity ("Specularity", Range(0.01,8)) = 0.3
		_SpecPower("Specularity Power", Range(0,1)) = 1

		[HideInInspector] _SunColor ("SunColor", Color) = (1,1,0.901,1)

		//_MainTex("Base (RGB)", 2D) = "white" {}
		_Refraction ("Refraction (RGB)", 2D) = "white" {}
		_Reflection ("Reflection (RGB)", 2D) = "white" {}
		_BackRefraction("Refraction (RGB)", 2D) = "blue" {}
		_BackReflection("Reflection (RGB)", 2D) = "blue" {}
		_Bump ("Bump (RGB)", 2D) = "bump" {}
		_Foam("Foam (RGB)", 2D) = "white" {}
		_FoamBump ("Foam B(RGB)", 2D) = "bump" {}
		_FoamFactor("Foam Factor", Range(0,3)) = 1.8
		
		_Size ("UVSize", Float) = 0.015625//this is the best value (1/64) to have the same uv scales of normal and foam maps on all ocean sizes
		_FoamSize ("FoamUVSize", Float) = 2//tiling of the foam texture
		[HideInInspector] _SunDir ("SunDir", Vector) = (0.3, -0.6, -1, 0)

		_FakeUnderwaterColor ("Water Color LOD1", Color) = (0.196, 0.262, 0.196, 1)
		_WaterLod1Alpha ("Water Transparency", Range(0,1)) = 0.95

		[NoScaleOffset] _FoamGradient ("Foam gradient ", 2D) = "white" {}
		_ShoreDistance("Shore Distance", Range(0,20)) = 4
		_ShoreStrength("Shore Strength", Range(1,4)) = 1.5
		_Translucency("Translucency Factor", Range(0,6)) = 2.5

		//FOG PROPERTIES
		_FogNear ("Fog Near", range (0,100)) = 5
        _FogFar ("Fog Far", range (0,100)) = 10
        _FogAltScale ("Fog Alt. Scale", range (0,100)) = 10
        _FogThinning ("Fog Thinning", range (0,100)) = 100
        _FogColor ("Fog Color", Color) = (0.5,0.5,0.5,1)
	}

	//water bump/foam bump/double foam/reflection/refraction//shore foam
	SubShader {
	    Tags { "RenderType" = "Opaque" "Queue"="Geometry"}
		LOD 8

		//Base Forward Pass For Top of Ocean
    	Pass {
			Tags { "LightMode" = "ForwardBase" }
			//Fog{ Mode On }
			Cull Back
			//Blend One Zero

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma surface surf Lambert finalcolor:mycolor vertex:vert
			//#pragma multi_compile_fog
			#pragma multi_compile SHORE_ON SHORE_OFF
			#pragma multi_compile FOGON FOGOFF
			#pragma target 3.0

			#include "UnityCG.cginc"
			//#include "UnityStandardCausticsCore.cginc"

			//added snippet
			sampler2D _Refraction;
			sampler2D _Reflection;
			sampler2D _Bump;
			sampler2D _Foam;
			sampler2D _FoamBump;
			//sampler2D _Fresnel;
			
			half _Size; //half type supposedly saves GPU time with DirectX and XNA support according to: www.gamedev.net/topic/457132-half-shader-data-type/
			half _FoamFactor;
			half4 _SunDir;
			half _FoamSize;
			half _Translucency;

			//Color
			half4 _WaterColor;
			half4 _SurfaceColor; //Front specularity

			half _Specularity; //shininess
			half _BackSpecularity;

			//Intensity and Sun
			half _SpecPower;
			half4 _SunColor;

			#ifdef SHORE_ON
			uniform sampler2D _CameraDepthTexture;
			sampler2D _FoamGradient;
			half _ShoreDistance;
			half _ShoreStrength;
			#endif
			//end of added

			#ifdef FOGON
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			uniform half4 unity_FogDensity;
			#endif

			struct v2f {  //Creates Directions and Normals
    			float4 pos : SV_POSITION;
    			half4  projTexCoord : TEXCOORD0;
    			float4  bumpTexCoord : TEXCOORD1;

				#ifdef SHORE_ON
				float4 ref : TEXCOORD2;
				#endif

    			half4  objSpaceNormal : TEXCOORD3;
    			half3  lightDir : TEXCOORD4;
				float4 buv : TEXCOORD5;
				half3 normViewDir : TEXCOORD6;
				//UNITY_FOG_COORDS(7)
				#ifdef FOGON
				half dist : TEXCOORD7;
				#endif
			};
			
			v2f vert (appdata_tan v) { //Initializes where Everything should be.. i.e. Foam, Waves, Color Projections
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz*_Size;///64;//float2(_Size.x, _Size.z);

    			o.pos = UnityObjectToClipPos (v.vertex);

				o.bumpTexCoord.z = v.tangent.w * _FoamFactor;

  				half4 projSource = half4(v.vertex.x, 0.0, v.vertex.z, 1);// was x,x,x, 1
    			half4 tmpProj = UnityObjectToClipPos( projSource);
    			//o.projTexCoord = tmpProj;

				o.projTexCoord.xy = 0.5 * tmpProj.xy * half2(1, _ProjectionParams.x) / tmpProj.w + half2(0.5, 0.5);

    			half3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			half3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				half3x3 rotation = half3x3( v.tangent.xyz, binormal, v.normal );
				
    			o.objSpaceNormal.xyz = v.normal;
    			half3 viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, half3(_SunDir.xyz));

				o.bumpTexCoord.w = _SinTime.y * 0.5;

				o.buv = float4(o.bumpTexCoord.x + _CosTime.x * 0.2, o.bumpTexCoord.y + _SinTime.x *0.3, o.bumpTexCoord.x + _CosTime.y * 0.04, o.bumpTexCoord.y + o.bumpTexCoord.w );

				//World UV's
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				//o.worldPos = mul(_Object2World, v.vertex).xyz;
				//o.bumpuv.xyzw = o.worldPos.xzxz  * _WaveTiling*0.005  + frac(_Time.xxxx * _WaveDirection);

				
				o.buv = float4(worldPos.x + _CosTime.x * 0.2, worldPos.z + _SinTime.x *0.3 ,worldPos.x + _CosTime.y * 0.04, worldPos.z + o.bumpTexCoord.w )*0.05;
				//-End World UV's

				o.normViewDir = normalize(viewDir);

				half3 transLightDir = -o.lightDir + v.normal;

				o.objSpaceNormal.w = pow ( max (0, dot ( o.normViewDir, -transLightDir ) ), 1 ) * 0.5;
				#ifdef SHORE_ON
				o.ref = ComputeScreenPos(o.pos);
				#endif

				#ifdef FOGON
				//manual fog
				o.dist = (unity_FogEnd.x - length(o.pos.xyz)) / (unity_FogEnd.x - unity_FogStart.x);
                #endif
				//autofog
				//UNITY_TRANSFER_FOG(o, o.pos);
    			return o;
			}


			half4 frag(v2f i) : COLOR{
				
				//ambiance
				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _WaterColor.rgb;
				
				//foam
				half _foam = tex2D(_Foam, -i.buv.xy  *_FoamSize).r;
				half foam = clamp(_foam  * tex2D(_Foam, i.buv.zy * _FoamSize).r - 0.15, 0.0, 1.0)  * i.bumpTexCoord.z;
				//-----------------------------------------------------------------------------------------------------------------------------------------------------------

				//bumps			
				half3 tangentNormal0 = (tex2D(_Bump, i.buv.xy) * 2) + (tex2D(_Bump, i.buv.zw) * 2) - 2 + (tex2D(_FoamBump, i.bumpTexCoord.xy*_FoamSize) * 4 - 1)*foam;
				half3 tangentNormal = normalize(tangentNormal0);

				half2 bumpSampleOffset = (i.objSpaceNormal.xz + tangentNormal.xy) * 0.05 + i.projTexCoord.xy;

				//-----------------------------------------------------------------------------------------------------------------------------------------------------------
				//fresnel
				//#ifdef SHORE_ON
				half fresnelTerm = 1.0 - saturate(dot(i.normViewDir, tangentNormal0));
				/*#else
				half fresnelLookup = dot(tangentNormal, i.normViewDir);
				float bias = 0.06;
				float power = 4.0;
				half fresnelTerm = 0.06 + (1.0 - 0.06)*pow(1.0 - fresnelLookup, 4);
				#endif*/

				//-----------------------------------------------------------------------------------------------------------------------------------------------------------
				half3 floatVec = normalize(i.normViewDir - normalize(i.lightDir));
				//specular
				#ifdef SHORE_ON
				half specular = pow(max(dot(floatVec, tangentNormal), 0.0), 250.0 * _Specularity) * _SpecPower;
				#else
				half specular = pow(max(dot(floatVec, tangentNormal), 0.0), 250.0 * _Specularity) *(1.2 - foam) * _SpecPower;
				#endif

				//-----------------------------------------------------------------------------------------------------------------------------------------------------------


				//SHORELINES
				#ifdef SHORE_ON
				float zdepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float intensityFactor = 1 - saturate((zdepth - i.ref.z) / _ShoreDistance);
				half3 foamGradient = _ShoreStrength - tex2D(_FoamGradient, float2(intensityFactor - i.bumpTexCoord.w, 0) + tangentNormal.xy);
				foam += foamGradient * intensityFactor * _foam;
				#endif
				//-----------------------------------------------------------------------------------------------------------------------------------------------------------
				//translucency
				half3 wc = _WaterColor.rgb * i.objSpaceNormal.w *_Translucency * _SunColor.rgb;//* floatVec.z 

				//wc = ambientLighting + (1 - w) * wc;

				half4 result = half4(wc.x, wc.y, wc.z, .8);// +half4(ambientLighting + _WaterColor.rgb + _SurfaceColor.rgb, 1.0);
				//-----------------------------------------------------------------------------------------------------------------------------------------------------------
				//reflection refraction
				half3 reflection = tex2D(_Reflection, bumpSampleOffset) * _SurfaceColor.rgb; // 1.3;
				half3 refraction = tex2D(_Refraction, bumpSampleOffset) * _WaterColor.rgb / 0.82;// / _SurfaceColor.rgb;

				//-----------------------------------------------------------------------------------------------------------------------------------------------------------
				//half4 result = half4(0 , 0 , 0, 1);
				//method2
				result.rgb += lerp(refraction, reflection, fresnelTerm)*_SunColor.rgb + clamp(foam, 0.1, 1.0)*_SunColor.b + specular*_SunColor.rgb;
				//fog
				//UNITY_APPLY_FOG(i.fogCoord, result); 

				#ifdef FOGON
				//manual fog (linear) (reduces instructions on d3d9)
				float ff = saturate(i.dist);
				result.rgb = lerp(unity_FogColor.rgb, result.rgb, ff);
				#endif
				return result;
			}

			ENDCG
		}

		/*
		//Base Forward Pass For Caustics
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			CGPROGRAM
			#pragma target 3.0
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles

			// -------------------------------------

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP 
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog

			#pragma vertex vertForwardBase
			#pragma fragment fragForwardBase

			#include "UnityStandardCausticsCore.cginc"

			ENDCG
		}

			//Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			Fog { Color (0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
			#pragma target 3.0
			// GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles

			// -------------------------------------

			
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP
			
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			
			#pragma vertex vertForwardAdd
			#pragma fragment fragForwardAdd

			#include "UnityStandardCausticsCore.cginc"

			ENDCG
		}
		
		//Shadow Caustic Rendering Pass
		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 3.0
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers gles
			
			// -------------------------------------


			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"

			ENDCG
		}
		//Deferred Pass
		Pass
		{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			CGPROGRAM
			#pragma target 3.0
			// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
			#pragma exclude_renderers nomrt gles
			

			// -------------------------------------

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
			#pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
			
			#pragma vertex vertDeferred
			#pragma fragment fragDeferred

			#include "UnityStandardCausticsCore.cginc"

			ENDCG
		}
		*/
		//Shadow Pass
		Pass
		{
			 Name "ShadowCaster"
			 Tags { "LightMode" = "ShadowCaster" "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
			 //Blend Zero OneMinusSrcAlpha
			 ZWrite Off //no depth change
			 Fog {Mode Off}
			 ZWrite On ZTest Less Cull Off
			 Offset 3, 3

			 CGPROGRAM

			 #pragma vertex vert
			 #pragma fragment frag
			 #pragma fragmentoption ARB_precision_hint_fastest
			 #pragma multi_compile_shadowcaster
			 //#pragma addshadow
			 #include "UnityCG.cginc"

			// User-specified properties
			uniform sampler2D _ShadowTex;

			sampler2D _MainTex;
			sampler2D _WaterColor;
			sampler2D _Bump;

			uniform half4 _MainTex_ST;
			half _Translucency;
			half _Cutoff;

             struct v2f
             {
                 float2 uv : TEXCOORD3;
				 V2F_SHADOW_CASTER;
             };
 
 
			 v2f vert(appdata_full v )
             {
				 v2f o; //vertex out put
				 TRANSFER_SHADOW_CASTER(o);
				//o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				 //o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
               return o;
             }
 
             float4 frag(v2f input ) : COLOR
             {
				 fixed4 texcol = tex2D(_WaterColor, input.uv);
                 clip(texcol.a - _Cutoff);
				 SHADOW_CASTER_FRAGMENT(input)
             }
             ENDCG
		}

		//UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		//FallBack "Diffuse"

		//Fog Pass
		/*Pass
		{
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma frament frag
			#pragma target 3.0

			sampler2D _MainTex;

			struct v2f 
			{
				float fogHeight;
				float fogDensity;
				half4 _FogColor;
			};

			v2f vert(appdata_base v) 
			{
				v2f o;

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float3 uv = UNITY_PROJ_COORD(IN.grabUV);
				float dpth = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, uv));
				dpth = LinearEyeDepth(dpth);
				float4 wsPos = (_CameraWS + dpth * IN.interpolatedRay); // Here is the problem but how to fix it
				float fogVert = max(0.0, (wsPos.y - _Depth) * (_DepthScale * 0.1f));
				fogVert *= fogVert;
				fogVert = (exp(-fogVert));
				return fogVert;
			}
			ENDCG

		}*/
		//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		Pass  //Third Pass - Underwater
		{
			Tags { "LightMode" = "ForwardBase" }
			  //Pass ambient light and first sources of light
			//Fog{ Mode On }
		    Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile SHORE_ON SHORE_OFF
			#pragma multi_compile FOGON FOGOFF
			#pragma target 3.0

			#include "UnityCG.cginc"

			//added snippet
			sampler2D _Refraction;
			sampler2D _Reflection;
			sampler2D _BackRefraction;
			sampler2D _BackReflection;
			sampler2D _Bump;
			sampler2D _Foam;
			sampler2D _FoamBump;
			sampler2D _Fresnel;
			
			half _Size; //half type supposedly saves GPU time with DirectX and XNA support according to: www.gamedev.net/topic/457132-half-shader-data-type/
			half _FoamFactor;
			half4 _SunDir;
			half _FoamSize;
			half _Translucency;

			//Color
			half4 _WaterColor;
			half4 _SurfaceColor; //Front specularity
			half4 _BackWaterColor;
			half4 _BackSurfaceColor;
			//half4 _BackSpecColor;

			half _Specularity; //shininess
			half _BackSpecularity;

			//Intensity and Sun
			half _SpecPower;
			half4 _SunColor;

			#ifdef SHORE_ON
			uniform sampler2D _CameraDepthTexture;
			sampler2D _FoamGradient;
			half _ShoreDistance;
			half _ShoreStrength;
			#endif
			//end of added

			#ifdef FOGON
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			uniform half4 unity_FogDensity;
			#endif

			struct v2f {  //Creates Directions and Normals
    			float4 pos : SV_POSITION;
    			half4  projTexCoord : TEXCOORD0;
    			float4  bumpTexCoord : TEXCOORD1;

				#ifdef SHORE_ON
				float4 ref : TEXCOORD2;
				#endif

    			half4  objSpaceNormal : TEXCOORD3;
    			half3  lightDir : TEXCOORD4;
				float4 buv : TEXCOORD5;
				half3 normViewDir : TEXCOORD6;
				//UNITY_FOG_COORDS(7)
				#ifdef FOGON
				half dist : TEXCOORD7;
				#endif
			};
			
			v2f vert (appdata_tan v) { //Initializes where Everything should be.. i.e. Foam, Waves, Color Projections
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz*_Size;///64;//float2(_Size.x, _Size.z);

    			o.pos = UnityObjectToClipPos (v.vertex);

				o.bumpTexCoord.z = v.tangent.w * _FoamFactor;

  				half4 projSource = half4(v.vertex.x, 0.0, v.vertex.z, 1.0);
    			half4 tmpProj = UnityObjectToClipPos( projSource);
    			//o.projTexCoord = tmpProj;

				o.projTexCoord.xy = 0.5 * tmpProj.xy * half2(1, _ProjectionParams.x) / tmpProj.w + half2(0.5, 0.5);

    			half3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			half3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) ); //opposite normal - bluer underwater
				half3x3 rotation = half3x3( v.tangent.xyz, binormal, v.normal ); //opposite normal - creates more of a white underwater
				
    			o.objSpaceNormal.xyz = v.normal;  //Opposite Underwater Direction - grey underwater
    			half3 viewDir = mul(-rotation, objSpaceViewDir); //Underwater -> Opposite Rotation of Normal Direction - See some blue underwater
				o.lightDir = mul(rotation, half3(_SunDir.xyz));

				o.bumpTexCoord.w = _SinTime.y * 0.5;

				o.buv = float4(o.bumpTexCoord.x + _CosTime.x * 0.2, o.bumpTexCoord.y + _SinTime.x *0.3, o.bumpTexCoord.x + _CosTime.y * 0.04, o.bumpTexCoord.y + o.bumpTexCoord.w );

				//World UV's
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				//o.worldPos = mul(_Object2World, v.vertex).xyz;
				//o.bumpuv.xyzw = o.worldPos.xzxz  * _WaveTiling*0.005  + frac(_Time.xxxx * _WaveDirection);

				
				o.buv = float4(worldPos.x + _CosTime.x * 0.2, worldPos.z + _SinTime.x *0.3 ,worldPos.x + _CosTime.y * 0.04, worldPos.z + o.bumpTexCoord.w )*0.05;
				//-End World UV's

				o.normViewDir = normalize(viewDir);

				half3 transLightDir = -o.lightDir + v.normal; //Changed From (-o.lightDir + v.normal) Not sure if anything happend

				o.objSpaceNormal.w = pow ( max (0, dot ( o.normViewDir, -transLightDir ) ),1 ) * .5; //increases brightness of water from underwater
				#ifdef SHORE_ON
				o.ref = ComputeScreenPos(o.pos);
				#endif

				#ifdef FOGON
				//manual fog
				o.dist = (unity_FogEnd.x + length(o.pos.xyz)) / (unity_FogEnd.x - unity_FogStart.x);
                #endif

				//autofog
				UNITY_TRANSFER_FOG(o, o.pos);
    			return o;
			}


			half4 frag(v2f i) : COLOR{

				//foam
				half _foam = tex2D(_Foam, -i.buv.xy  *_FoamSize).r;
				half foam = clamp(_foam  * tex2D(_Foam, i.buv.zy * _FoamSize).r - 0.15, 0.0, 1.0)  * i.bumpTexCoord.z;
				//-----------------------------------------------------------------------------------------------------------------------------------------------------------

				//bumps			
				half3 tangentNormal0 = (tex2D(_Bump, i.buv.xy) * 2) + (tex2D(_Bump, i.buv.zw) * 2) - 2 + (tex2D(_FoamBump, i.bumpTexCoord.xy*_FoamSize) * 4 - 1)*foam;
				half3 tangentNormal = normalize(-tangentNormal0);

				half2 bumpSampleOffset = (i.objSpaceNormal.xz + tangentNormal.xy) * 0.05 + i.projTexCoord.xy;

				//-----------------------------------------------------------------------------------------------------------------------------------------------------------
				//fresnel
				#ifdef SHORE_ON
				half fresnelTerm = .8 - saturate(dot(i.normViewDir, tangentNormal0));
				#else
				/half fresnelLookup = dot(tangentNormal, i.normViewDir);
				float bias = 0.06;
				float power = 4.0;
				half fresnelTerm = 0.06 + (1.0 - 0.06)*pow(1.0 - fresnelLookup, 4);
				#endif

				//-----------------------------------------------------------------------------------------------------------------------------------------------------------
				half3 floatVec = normalize(i.normViewDir - normalize(i.lightDir)); //-norm view direction change
				//specular
				#ifdef SHORE_ON
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity) * _SpecPower;
				#else
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity) *(1.2 - foam) * _SpecPower;
				#endif

				//-----------------------------------------------------------------------------------------------------------------------------------------------------------


				//SHORELINES
				#ifdef SHORE_ON
				float zdepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float intensityFactor = 1 - saturate((zdepth - i.ref.z) / _ShoreDistance);
				half3 foamGradient = _ShoreStrength - tex2D(_FoamGradient, float2(intensityFactor - i.bumpTexCoord.w, 0) + tangentNormal.xy);
				foam += foamGradient * intensityFactor * _foam;
				#endif
				//-----------------------------------------------------------------------------------------------------------------------------------------------------------
				//translucency
				half3 wc = -_WaterColor.rgb * i.objSpaceNormal.w * _Translucency * _SunColor.rgb * floatVec.z; //can see the sky better through water WHEN NEGATIVE

			   half4 result = half4(wc.x , wc.y , wc.z, .65);  //Last value changes the translucency value --> For clearer water
			   //-----------------------------------------------------------------------------------------------------------------------------------------------------------
			   //reflection refraction
			   half3 reflection = tex2D(_Reflection,  -bumpSampleOffset*1) * _SurfaceColor.rgb * 2.5;
			   half3 refraction = tex2D(_Refraction,  bumpSampleOffset) * _WaterColor.rgb * 2.5;//*_FakeUnderwaterColor

			   //-----------------------------------------------------------------------------------------------------------------------------------------------------------
			   //half4 result = half4(0 , 0 , 0, 1);
			   //method2
			   result.rgb += lerp(refraction, reflection, fresnelTerm)*_SunColor.rgb - clamp(foam, 0.0, 1.0)*_SunColor.b + specular*_SunColor.rgb;
			   //fog
			   UNITY_APPLY_FOG(i.fogCoord, -result*2); 

			   #ifdef FOGON
			   //manual fog (linear) (reduces instructions on d3d9)
			   float ff = saturate(i.dist);
			   result.rgb = lerp(unity_FogColor.rgb, result.rgb, ff);
			   //result.alpha = .5;
			   #endif
			  return result;
			}
			ENDCG
		}

		//Uncomment when making game
		//FallBack "Diffuse"
		//Fallback "VertexLit"
    }

	//water bump/double foam/reflection/alpha/shore foam
	SubShader {
	    Tags  { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 7
		Blend SrcAlpha OneMinusSrcAlpha
		/*Pass
		{
			//Color(1,0,1,1)
			//Cull Off
			Name "BASE"
			Tags{ "LightMode" = "Always"}
			Cull Off
			Color[_PPLAmbient]
			SetTexture[_MainTex]{ constantColor[_Color] Combine texture * primary DOUBLE, texture * constant }
		}*/

    	Pass {
			Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile SHORE_ON SHORE_OFF
			#pragma multi_compile FOGON FOGOFF

			#pragma target 2.0

			#include "UnityCG.cginc"

			struct v2f {
    			float4 pos : SV_POSITION;
    			half4  projTexCoord : TEXCOORD0;
    			float4  bumpTexCoord : TEXCOORD1;
				#ifdef SHORE_ON
				float4 ref : TEXCOORD2;
				#endif
    			half3  objSpaceNormal : TEXCOORD3;
    			half3  lightDir : TEXCOORD4;
				float4 buv : TEXCOORD5;
				half3 normViewDir : TEXCOORD6;
				//UNITY_FOG_COORDS(7)
				#ifdef FOGON
				half dist : TEXCOORD7;
				#endif
			};

			half _Size;
			half _FoamFactor;
			half4 _SunDir;
			#ifdef FOGON
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			uniform half4 unity_FogDensity;
			#endif

			v2f vert (appdata_tan v) {
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz*_Size;///64;//float2(_Size.x, _Size.z);

    			o.pos = UnityObjectToClipPos (v.vertex);

				o.bumpTexCoord.z = v.tangent.w * _FoamFactor;
 
  				half4 projSource = half4(v.vertex.x, 0.0, v.vertex.z, 1.0);
    			half4 tmpProj = UnityObjectToClipPos( projSource);
    			//o.projTexCoord = tmpProj;

				o.projTexCoord.xy = 0.5 * tmpProj.xy * half2(1, _ProjectionParams.x) / tmpProj.w + half2(0.5, 0.5);

    			half3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			half3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				half3x3 rotation = half3x3( v.tangent.xyz, binormal, v.normal );
    
    			o.objSpaceNormal = v.normal;
    			half3 viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, half3(_SunDir.xyz));

				o.bumpTexCoord.w = _SinTime.y * 0.5;

				o.buv = float4(o.bumpTexCoord.x + _CosTime.x * 0.2, o.bumpTexCoord.y + _SinTime.x * 0.3, o.bumpTexCoord.x + _CosTime.y * 0.04, o.bumpTexCoord.y + o.bumpTexCoord.w);

				o.normViewDir = normalize(viewDir);

				#ifdef SHORE_ON
				o.ref = ComputeScreenPos(o.pos);
				#endif
  
				#ifdef FOGON
				//manual fog
				o.dist = (unity_FogEnd.x - length(o.pos.xyz)) / (unity_FogEnd.x - unity_FogStart.x);
				#endif
				            
				//autofog
				//UNITY_TRANSFER_FOG(o, o.pos);

    			return o;
			}

			sampler2D _Refraction;
			sampler2D _Reflection;
			sampler2D _Bump;
			sampler2D _Foam;
			sampler2D _FoamBump;
			#ifdef SHORE_ON
			uniform sampler2D _CameraDepthTexture;
			sampler2D _FoamGradient;
			half _ShoreDistance;
			half _ShoreStrength;
			#endif
			half _FoamSize;
			half4 _SurfaceColor;
			half4 _WaterColor;
			half _Specularity;
			half _SpecPower;
			half4 _SunColor;
			half _WaterLod1Alpha;
			half4 _FakeUnderwaterColor;

			half4 frag (v2f i) : COLOR {
				half _foam =  tex2D(_Foam, -i.buv.xy  *_FoamSize).r;
				half foam = clamp( _foam * tex2D(_Foam, i.buv.zy * _FoamSize).r -0.15, 0.0, 1.0)  * i.bumpTexCoord.z;
								
				half3 tangentNormal0 = (tex2D(_Bump, i.buv.xy) * 2 )+( tex2D(_Bump, i.buv.zw) * 2 ) - 2 + (  tex2D(_FoamBump, i.bumpTexCoord.xy*_FoamSize)*4   - 1)*foam; 
				half3 tangentNormal = normalize(tangentNormal0 );

				half4 result = half4(0, 0, 0, 1);

				half2 bumpSampleOffset = (i.objSpaceNormal.xz  + tangentNormal.xy) * 0.05  + i.projTexCoord.xy;// + projTexCoord.xy
	
				half3 reflection = tex2D( _Reflection,  bumpSampleOffset) * _SurfaceColor *_FakeUnderwaterColor.a ;

				//fresnel
				//#ifdef SHORE_ON
				half fresnelTerm = 1.0 - saturate(dot (i.normViewDir, tangentNormal0));
				//#else
				//half fresnelLookup = dot(tangentNormal, i.normViewDir);
				//float bias = 0.06;
				//float power = 4.0;
				//half fresnelTerm = 0.06 + (1.0 - 0.06)*pow(1.0 - fresnelLookup, 4);
				//#endif

				half3 floatVec = normalize(i.normViewDir - normalize(i.lightDir));

				#ifdef SHORE_ON
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) * _SpecPower;
				#else
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) *(1.2-foam) * _SpecPower;
				#endif

				//SHORELINES
				#ifdef SHORE_ON
				float zdepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float intensityFactor = 1 - saturate((zdepth - i.ref.z) / _ShoreDistance);  
				half3 foamGradient = _ShoreStrength - tex2D(_FoamGradient, float2(intensityFactor - i.bumpTexCoord.w, 0) + tangentNormal.xy);
				foam += foamGradient * intensityFactor * _foam;
				#endif

				//method2
				result.rgb = lerp(_WaterColor*_FakeUnderwaterColor, reflection, fresnelTerm)*_SunColor.rgb + clamp(foam, 0.0, 1.0)*_SunColor.b + specular*_SunColor.rgb;
				result.a = _WaterLod1Alpha;


				//fog
				//UNITY_APPLY_FOG(i.fogCoord, result); 

				#ifdef FOGON
				//manual fog (linear) (reduces instructions on d3d9)
				float ff = saturate(i.dist);
				result.rgb = lerp(unity_FogColor.rgb, result.rgb, ff);
				#endif

    			return result;
			}

			ENDCG
		}
    }

	//water bump/foam/reflection/alpha/shore foam
	SubShader {
	    Tags  { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 6
		Blend SrcAlpha OneMinusSrcAlpha
		/*Pass
		{
			//Color(1,0,1,1)
			//Cull Off
			Name "BASE"
			Tags{ "LightMode" = "Always"}
			Cull Off
			Color[_PPLAmbient]
			SetTexture[_MainTex]{ constantColor[_Color] Combine texture * primary DOUBLE, texture * constant }
		}*/
    	Pass {
			Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile SHORE_ON SHORE_OFF
			#pragma multi_compile FOGON FOGOFF

			#pragma target 2.0

			#include "UnityCG.cginc"

			struct v2f {
    			float4 pos : SV_POSITION;
    			half4  projTexCoord : TEXCOORD0;
    			float4  bumpTexCoord : TEXCOORD1;
				#ifdef SHORE_ON
				float4 ref : TEXCOORD2;
				#endif
    			half3  objSpaceNormal : TEXCOORD3;
    			half3  lightDir : TEXCOORD4;
				float4 buv : TEXCOORD5;
				half3 normViewDir : TEXCOORD6;
				//UNITY_FOG_COORDS(7)
				#ifdef FOGON
				half dist : TEXCOORD7;
				#endif
			};

			half _Size;
			half _FoamFactor;
			half4 _SunDir;
			#ifdef FOGON
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			uniform half4 unity_FogDensity;
			#endif

			v2f vert (appdata_tan v) {
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz*_Size;///64;//float2(_Size.x, _Size.z);

    			o.pos = UnityObjectToClipPos (v.vertex);

				o.bumpTexCoord.z = v.tangent.w * _FoamFactor;
 
  				half4 projSource = half4(v.vertex.x, 0.0, v.vertex.z, 1.0);
    			half4 tmpProj = UnityObjectToClipPos( projSource);
    			//o.projTexCoord = tmpProj;

				o.projTexCoord.xy = 0.5 * tmpProj.xy * half2(1, _ProjectionParams.x) / tmpProj.w + half2(0.5, 0.5);

    			half3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			half3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				half3x3 rotation = half3x3( v.tangent.xyz, binormal, v.normal );
    
    			o.objSpaceNormal = v.normal;
    			half3 viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, half3(_SunDir.xyz));

				o.bumpTexCoord.w = _SinTime.y * 0.5;

				o.buv = float4(o.bumpTexCoord.x + _CosTime.x * 0.2, o.bumpTexCoord.y + _SinTime.x * 0.3, o.bumpTexCoord.x + _CosTime.y * 0.04, o.bumpTexCoord.y + o.bumpTexCoord.w);

				o.normViewDir = normalize(viewDir);

  				#ifdef SHORE_ON
				o.ref = ComputeScreenPos(o.pos);
				#endif
				    
				#ifdef FOGON
				//manual fog
				o.dist = (unity_FogEnd.x - length(o.pos.xyz)) / (unity_FogEnd.x - unity_FogStart.x);
				#endif
				      
				//autofog
				//UNITY_TRANSFER_FOG(o, o.pos);

    			return o;
			}

			sampler2D _Refraction;
			sampler2D _Reflection;
			sampler2D _Bump;
			sampler2D _Foam;
			#ifdef SHORE_ON
			uniform sampler2D _CameraDepthTexture;
			sampler2D _FoamGradient;
			half _ShoreDistance;
			half _ShoreStrength;
			#endif
			half _FoamSize;
			half4 _SurfaceColor;
			half4 _WaterColor;
			half _Specularity;
			half _SpecPower;
			half4 _SunColor;
			half _WaterLod1Alpha;
			half4 _FakeUnderwaterColor;

			half4 frag (v2f i) : COLOR {

				half _foam = tex2D(_Foam, -i.buv.xy *_FoamSize).r;
				half foam = clamp(_foam  - 0.5, 0.0, 1.0) * i.bumpTexCoord.z;
								
				half3 tangentNormal0 = (tex2D(_Bump, i.buv.xy) * 2.0) + (tex2D(_Bump, i.buv.zw) * 2.0) - 2;
				half3 tangentNormal = normalize(tangentNormal0 );

				half4 result = half4(0, 0, 0, 1);

				half2 bumpSampleOffset = (i.objSpaceNormal.xz  + tangentNormal.xy) * 0.05  + i.projTexCoord.xy;// + projTexCoord.xy
	
				half3 reflection = tex2D( _Reflection,  bumpSampleOffset) * _SurfaceColor *_FakeUnderwaterColor.a ;


				//fresnel
				//#ifdef SHORE_ON
				half fresnelTerm = 1.0 - saturate(dot (i.normViewDir, tangentNormal0));
				//#else
				//half fresnelLookup = dot(tangentNormal, i.normViewDir);
				//float bias = 0.06;
				//float power = 4.0;
				//half fresnelTerm = 0.06 + (1.0 - 0.06)*pow(1.0 - fresnelLookup, 4);
				//#endif

				half3 floatVec = normalize(i.normViewDir - normalize(i.lightDir));

				#ifdef SHORE_ON
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) * _SpecPower;
				#else
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) *(1.2-foam) * _SpecPower;
				#endif

				//SHORELINES
				#ifdef SHORE_ON
				float zdepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float intensityFactor = 1 - saturate((zdepth - i.ref.z) / _ShoreDistance);  
				half3 foamGradient = _ShoreStrength - tex2D(_FoamGradient, float2(intensityFactor - i.bumpTexCoord.w, 0) + tangentNormal.xy);
				foam += foamGradient * intensityFactor * _foam;
				#endif

				//method2
				result.rgb = lerp(_WaterColor*_FakeUnderwaterColor, reflection, fresnelTerm)*_SunColor.rgb + clamp(foam, 0.0, 1.0)*_SunColor.b + specular*_SunColor.rgb;
				result.a = _WaterLod1Alpha;

				//fog
				//UNITY_APPLY_FOG(i.fogCoord, result); 

				#ifdef FOGON
				//manual fog (linear) (reduces instructions on d3d9)
				float ff = saturate(i.dist);
				result.rgb = lerp(unity_FogColor.rgb, result.rgb, ff);
				#endif

    			return result;
			}

			ENDCG
		}
    }

	//water bump/foam bump/double foam/reflection/refraction
	SubShader {
	    Tags {"RenderType" = "Opaque" "Queue"="Geometry"}
		LOD 5
		//Blend SrcAlpha OneMinusSrcAlpha
		/*Pass
		{
			//Blend SrcAlpha OneMinusSrcAlpha
			//Color(1,0,1,1)
			//Cull Off
			Name "BASE"
			Tags{ "LightMode" = "Always"}
			Cull Off
			Color[_PPLAmbient]
			SetTexture[_MainTex]{ constantColor[_Color] Combine texture * primary DOUBLE, texture * constant }
		}*/
    	Pass {
			//Cull Off
			//Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile SHORE_ON SHORE_OFF
			#pragma multi_compile FOGON FOGOFF

			#pragma target 2.0

			#include "UnityCG.cginc"
			
			struct v2f {
    			float4 pos : SV_POSITION;
    			half4  projTexCoord : TEXCOORD0;
    			float4  bumpTexCoord : TEXCOORD1;
				#ifdef SHORE_ON
				float4 ref : TEXCOORD2;
				#endif
    			half3  objSpaceNormal : TEXCOORD3;
    			half3  lightDir : TEXCOORD4;
				float4 buv : TEXCOORD5;
				half3 normViewDir : TEXCOORD6;
				//UNITY_FOG_COORDS(7)
				#ifdef FOGON
				half dist : TEXCOORD7;
				#endif
			};

			half _Size;
			half _FoamFactor;
			half4 _SunDir;
			#ifdef FOGON
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			uniform half4 unity_FogDensity;
			#endif

			v2f vert (appdata_tan v) {
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				
    			o.bumpTexCoord.xy = v.vertex.xz*_Size;///64;//float2(_Size.x, _Size.z);

    			o.pos = UnityObjectToClipPos (v.vertex);

				o.bumpTexCoord.z = v.tangent.w * _FoamFactor;

  				half4 projSource = half4(v.vertex.x, v.vertex.y, v.vertex.z, 1.0);
    			half4 tmpProj = UnityObjectToClipPos( projSource);
    			//o.projTexCoord = tmpProj;

				o.projTexCoord.xy = 0.5 * tmpProj.xy * half2(1, _ProjectionParams.x) / tmpProj.w + half2(0.5, 0.5);

    			half3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			half3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				half3x3 rotation = half3x3( v.tangent.xyz, binormal, v.normal );
    
    			o.objSpaceNormal = v.normal;
    			half3 viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, half3(_SunDir.xyz));

				o.bumpTexCoord.w = _SinTime.y * 0.5;

				o.buv = float4(o.bumpTexCoord.x + _CosTime.x * 0.2, o.bumpTexCoord.y + _SinTime.x * 0.3, o.bumpTexCoord.x + _CosTime.y * 0.04, o.bumpTexCoord.y + o.bumpTexCoord.w);
				//World UV's
				//o.worldPos = mul(_Object2World, v.vertex).xyz;	
				//o.bumpuv.xyzw = o.worldPos.xzxz  * _WaveTiling*0.005  + frac(_Time.xxxx * _WaveDirection);

				o.normViewDir = normalize(viewDir);
				
	  			#ifdef SHORE_ON
				o.ref = ComputeScreenPos(o.pos);
				#endif
							
				#ifdef FOGON
				//manual fog
				o.dist = (unity_FogEnd.x - length(o.pos.xyz)) / (unity_FogEnd.x - unity_FogStart.x);
				#endif

				//autofog
				//UNITY_TRANSFER_FOG(o, o.pos);

    			return o;
			}

			sampler2D _Refraction;
			sampler2D _Reflection;
			sampler2D _Bump;
			sampler2D _Foam;
			sampler2D _FoamBump;
			#ifdef SHORE_ON
			uniform sampler2D _CameraDepthTexture;
			sampler2D _FoamGradient;
			half _ShoreDistance;
			half _ShoreStrength;
			#endif
			half _FoamSize;
			half4 _SurfaceColor;
			half4 _WaterColor;
			half _Specularity;
			half _SpecPower;
			half4 _SunColor;


			half4 frag (v2f i) : COLOR {

				half _foam =  tex2D(_Foam, -i.buv.xy  *_FoamSize).r;
				half foam = clamp( _foam * tex2D(_Foam, i.buv.zy * _FoamSize).r -0.15, 0.0, 1.0)  * i.bumpTexCoord.z;
								
				half3 tangentNormal0 = (tex2D(_Bump, i.buv.xy) * 2 )+( tex2D(_Bump, i.buv.zw) * 2 ) - 2 + (  tex2D(_FoamBump, i.bumpTexCoord.xy*_FoamSize)*4   - 1)*foam;
				half3 tangentNormal = normalize(tangentNormal0 );

				half4 result = half4(0, 0, 0, 1);

				half2 bumpSampleOffset = (i.objSpaceNormal.xz  + tangentNormal.xy) * 0.05  + i.projTexCoord.xy;
	
				half3 reflection = tex2D( _Reflection,  bumpSampleOffset) * _SurfaceColor  ;
				half3 refraction = tex2D( _Refraction,  bumpSampleOffset ) * _WaterColor ;

				//fresnel
				//#ifdef SHORE_ON
				half fresnelTerm = 1.0 - saturate(dot (i.normViewDir, tangentNormal0));
				//#else
				//half fresnelLookup = dot(tangentNormal, i.normViewDir);
				//float bias = 0.06;
				//float power = 4.0;
				//half fresnelTerm = 0.06 + (1.0 - 0.06)*pow(1.0 - fresnelLookup, 4);
				//#endif

				half3 floatVec = normalize(i.normViewDir - normalize(i.lightDir));

				#ifdef SHORE_ON
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) * _SpecPower;
				#else
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) *(1.2-foam) * _SpecPower;
				#endif

				//SHORELINES
				#ifdef SHORE_ON
				float zdepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float intensityFactor = 1 - saturate((zdepth - i.ref.z) / _ShoreDistance);  
				half3 foamGradient = _ShoreStrength - tex2D(_FoamGradient, float2(intensityFactor - i.bumpTexCoord.w, 0) + tangentNormal.xy);
				foam += foamGradient * intensityFactor * _foam;
				#endif

				//simple
				//result.rgb = lerp(refraction, reflection, fresnelTerm)+ clamp(foam, 0.0, 1.0) + specular;

				//method1
				//result.rgb = lerp(refraction, reflection, fresnelTerm) + clamp(foam, 0.0, 1.0)*_SunColor.b + specular;
				//result.rgb *= _SunColor.rgb;

				//method2
				result.rgb = lerp(refraction, reflection, fresnelTerm)*_SunColor.rgb + clamp(foam, 0.0, 1.0)*_SunColor.b + specular*_SunColor.rgb;


				//fog
				//UNITY_APPLY_FOG(i.fogCoord, result); 

				#ifdef FOGON
				//manual fog (linear) (reduces instructions on d3d9)
				float ff = saturate(i.dist);
				result.rgb = lerp(unity_FogColor.rgb, result.rgb, ff);
				#endif

    			return result;
			}
			
			ENDCG
		}

    }
		
	//water bump/foam/reflection/refraction
	SubShader{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}
		LOD 4
		//Blend SrcAlpha OneMinusSrcAlpha
		/*Pass
		{
			//Color(1,0,1,1)
			//Cull Off
			Name "BASE"
			Tags{ "LightMode" = "Always"}
			Cull Off
			Color[_PPLAmbient]
			SetTexture[_MainTex]{ constantColor[_Color] Combine texture * primary DOUBLE, texture * constant }
		}*/
		Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile SHORE_ON SHORE_OFF
			#pragma multi_compile FOGON FOGOFF

			#pragma target 2.0

			#include "UnityCG.cginc"
			struct v2f {
    			float4 pos : SV_POSITION;
    			half4  projTexCoord : TEXCOORD0;
    			float4  bumpTexCoord : TEXCOORD1;
				#ifdef SHORE_ON
				float4 ref : TEXCOORD2;
				#endif
    			half3  objSpaceNormal : TEXCOORD3;
    			half3  lightDir : TEXCOORD4;
				float4 buv : TEXCOORD5;
				half3 normViewDir : TEXCOORD6;
				//UNITY_FOG_COORDS(7)
				#ifdef FOGON
				half dist : TEXCOORD7;
				#endif
			};

			half _Size;
			half _FoamFactor;
			half4 _SunDir;
			#ifdef FOGON
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			uniform half4 unity_FogDensity;
			#endif

			v2f vert (appdata_tan v) {
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz*_Size;///64;//float2(_Size.x, _Size.z);

    			o.pos = UnityObjectToClipPos (v.vertex);

				o.bumpTexCoord.z = v.tangent.w * _FoamFactor;
 
  				half4 projSource = half4(v.vertex.x, 0.0, v.vertex.z, 1.0);
    			half4 tmpProj = UnityObjectToClipPos( projSource);
    			//o.projTexCoord = tmpProj;

				o.projTexCoord.xy = 0.5 * tmpProj.xy * half2(1, _ProjectionParams.x) / tmpProj.w + half2(0.5, 0.5);

    			half3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			half3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				half3x3 rotation = half3x3( v.tangent.xyz, binormal, v.normal );
    
    			o.objSpaceNormal = v.normal;
    			half3 viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, half3(_SunDir.xyz));

				o.bumpTexCoord.w = _SinTime.y * 0.5;

				o.buv = float4(o.bumpTexCoord.x + _CosTime.x * 0.2, o.bumpTexCoord.y + _SinTime.x * 0.3, o.bumpTexCoord.x + _CosTime.y * 0.04, o.bumpTexCoord.y + o.bumpTexCoord.w);

				o.normViewDir = normalize(viewDir);

  	  			#ifdef SHORE_ON
				o.ref = ComputeScreenPos(o.pos);
				#endif
				 
				#ifdef FOGON
				//manual fog
				o.dist = (unity_FogEnd.x - length(o.pos.xyz)) / (unity_FogEnd.x - unity_FogStart.x);
				#endif

				//autofog
				//UNITY_TRANSFER_FOG(o, o.pos);

    			return o;
			}

			sampler2D _Refraction;
			sampler2D _Reflection;
			sampler2D _Bump;
			sampler2D _Foam;
			half _FoamSize;
			#ifdef SHORE_ON
			uniform sampler2D _CameraDepthTexture;
			sampler2D _FoamGradient;
			half _ShoreDistance;
			half _ShoreStrength;
			#endif
			half4 _SurfaceColor;
			half4 _WaterColor;
			half _Specularity;
			half _SpecPower;
			half4 _SunColor;

			half4 frag (v2f i) : COLOR {

				half _foam = tex2D(_Foam, -i.buv.xy *_FoamSize).r;
				half foam = clamp(_foam  - 0.5, 0.0, 1.0) * i.bumpTexCoord.z;
								
				half3 tangentNormal0 = (tex2D(_Bump, i.buv.xy) * 2.0) + (tex2D(_Bump, i.buv.zw) * 2.0) - 2;
				half3 tangentNormal = normalize(tangentNormal0 );

				half4 result = half4(0, 0, 0, 1);

				half2 bumpSampleOffset = (i.objSpaceNormal.xz  + tangentNormal.xy) * 0.05  + i.projTexCoord.xy;// + projTexCoord.xy
	
				half3 reflection = tex2D( _Reflection,  bumpSampleOffset) * _SurfaceColor  ;
				half3 refraction = tex2D( _Refraction,  bumpSampleOffset ) * _WaterColor ;

				//fresnel
				//#ifdef SHORE_ON
				half fresnelTerm = 1.0 - saturate(dot (i.normViewDir, tangentNormal0));
				//#else
				//half fresnelLookup = dot(tangentNormal, i.normViewDir);
				//float bias = 0.06;
				//float power = 4.0;
				//half fresnelTerm = 0.06 + (1.0 - 0.06)*pow(1.0 - fresnelLookup, 4);
				//#endif

				half3 floatVec = normalize(i.normViewDir - normalize(i.lightDir));

				#ifdef SHORE_ON
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) * _SpecPower;
				#else
				half specular = pow(max(dot(floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) *(1.2-foam) * _SpecPower;
				#endif

				//SHORELINES
				#ifdef SHORE_ON
				float zdepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float intensityFactor = 1 - saturate((zdepth - i.ref.z) / _ShoreDistance);  
				half3 foamGradient = _ShoreStrength - tex2D(_FoamGradient, float2(intensityFactor - i.bumpTexCoord.w, 0) + tangentNormal.xy);
				foam += foamGradient * intensityFactor * _foam;
				#endif

				//simple
				//result.rgb = lerp(refraction, reflection, fresnelTerm)+ clamp(foam, 0.0, 1.0) + specular;

				//method1
				//result.rgb = lerp(refraction, reflection, fresnelTerm) + clamp(foam, 0.0, 1.0)*_SunColor.b + specular;
				//result.rgb *= _SunColor.rgb;

				//method2
				result.rgb = lerp(refraction, reflection, fresnelTerm)*_SunColor.rgb + clamp(foam, 0.0, 1.0)*_SunColor.b + specular*_SunColor.rgb;


				//fog
				//UNITY_APPLY_FOG(i.fogCoord, result); 

				#ifdef FOGON
				//manual fog (linear) (reduces instructions on d3d9)
				float ff = saturate(i.dist);
				result.rgb = lerp(unity_FogColor.rgb, result.rgb, ff);
				#endif

    			return result;
			}

			ENDCG
		}
    }
		
 	//water bump/foam
	SubShader {
        Tags { "RenderType" = "Opaque" "Queue"="Geometry"}
        LOD 3
		//Blend SrcAlpha OneMinusSrcAlpha
		/*Pass
		{
			//Color(1,0,1,1)
			Name "BASE"
			Tags{ "LightMode" = "Always" }
			Cull Off
			Color[_PPLAmbient]
			SetTexture[_MainTex]{ constantColor[_Color] Combine texture * primary DOUBLE, texture * constant }
		}*/
    	Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile SHORE_ON SHORE_OFF
			#pragma multi_compile FOGON FOGOFF

			#pragma target 2.0
			#include "UnityCG.cginc"

			struct v2f {
    			float4 pos : SV_POSITION;
				half3 floatVec : TEXCOORD0;
    			float4  bumpTexCoord : TEXCOORD1;
				#ifdef SHORE_ON
				float4 ref : TEXCOORD2;
				#endif
    			half3  lightDir : TEXCOORD3;
				half4 buv : TEXCOORD4;
				half3 normViewDir : TEXCOORD5;
				//UNITY_FOG_COORDS(7)
				#ifdef FOGON
				half dist : TEXCOORD7;
				#endif
			};

			half _Size;
			half _FoamFactor;
			half4 _SunDir;
			half4 _FakeUnderwaterColor;
			#ifdef FOGON
      		uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			uniform half4 unity_FogDensity;
			#endif
			      
			v2f vert (appdata_tan v) {
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz*_Size;///float2(_Size.x, _Size.z)*5;
    			o.pos = UnityObjectToClipPos (v.vertex);
    			o.bumpTexCoord.z = v.tangent.w * _FoamFactor;

    			half3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			half3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				half3x3 rotation = half3x3( v.tangent.xyz, binormal, v.normal );
    
    			half3 viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, half3(_SunDir.xyz));

				o.bumpTexCoord.w = _SinTime.y * 0.5;

				o.buv = float4(o.bumpTexCoord.x + _CosTime.x * 0.2, o.bumpTexCoord.y + _SinTime.x * 0.3, o.bumpTexCoord.x + _CosTime.y * 0.04, o.bumpTexCoord.y + o.bumpTexCoord.w);

				o.normViewDir = normalize(viewDir);

				o.floatVec = normalize(o.normViewDir - normalize(o.lightDir));

    	  		#ifdef SHORE_ON
				o.ref = ComputeScreenPos(o.pos);
				#endif

				#ifdef FOGON
				//manual fog
				o.dist = (unity_FogEnd.x - length(o.pos.xyz)) / (unity_FogEnd.x - unity_FogStart.x);
				#endif

				//autofog
				//UNITY_TRANSFER_FOG(o, o.pos);

    			return o;
			}

			sampler2D _Bump;
			sampler2D _Foam;
			half _FoamSize;
			#ifdef SHORE_ON
			uniform sampler2D _CameraDepthTexture;
			sampler2D _FoamGradient;
			half _ShoreDistance;
			half _ShoreStrength;
			#endif
			half4 _WaterColor;//Lod1;
			half4 _SurfaceColor;
			half _Specularity;
			half _SpecPower;
            half4 _SunColor;

			half4 frag (v2f i) : COLOR {

				half3 tangentNormal0 = (tex2D(_Bump, i.buv.xy) * 2.0) + (tex2D(_Bump, i.buv.zw) * 2.0) - 2;
				half3 tangentNormal = normalize(tangentNormal0);

				half4 result = half4(0, 0, 0, 1);
                
				//fresnel
				//#ifdef SHORE_ON
				half fresnelTerm = 1.0 - saturate(dot (i.normViewDir, tangentNormal0));
				//#else
				//half fresnelLookup = dot(tangentNormal, i.normViewDir);
				//float bias = 0.06;
				//float power = 4.0;
				//half fresnelTerm = 0.06 + (1.0 - 0.06)*pow(1.0 - fresnelLookup, 4);
				//#endif

				half _foam = tex2D(_Foam, -i.buv.xy *_FoamSize).r;
				half foam = clamp(_foam  - 0.5, 0.0, 1.0) * i.bumpTexCoord.z;

				#ifdef SHORE_ON
				half specular = pow(max(dot(i.floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) * _SpecPower;
				#else
				half specular = pow(max(dot(i.floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) *(1.2-foam) * _SpecPower;
				#endif

				//SHORELINES
				#ifdef SHORE_ON
				float zdepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float intensityFactor = 1 - saturate((zdepth - i.ref.z) / _ShoreDistance);  
				half3 foamGradient = _ShoreStrength - tex2D(_FoamGradient, float2(intensityFactor - i.bumpTexCoord.w, 0) + tangentNormal.xy);
				foam += foamGradient * intensityFactor * _foam;
				#endif
                
				result.rgb = lerp(_WaterColor*_FakeUnderwaterColor, _SunColor.rgb*_SurfaceColor*0.85, fresnelTerm*0.65) + clamp(foam.r, 0.0, 1.0)*_SunColor.b + specular*_SunColor.rgb;


				//fog
				//UNITY_APPLY_FOG(i.fogCoord, result); 

				#ifdef FOGON
				//manual fog (linear) (reduces instructions on d3d9)
				float ff = saturate(i.dist);
				result.rgb = lerp(unity_FogColor.rgb, result.rgb, ff);
				#endif

    			return result;
			}
			ENDCG
			

		}
    }
 
	//water bump
     SubShader {
        Tags { "RenderType" = "Opaque" "Queue"="Geometry"}
        LOD 2
		 //Blend SrcAlpha OneMinusSrcAlpha
		 /*Pass
		{
			//Color(1,0,1,1)
			//Cull Off
			Name "BASE"
			Tags{ "LightMode" = "Always"}
		 Cull Off
		 Color[_PPLAmbient]
		 SetTexture[_MainTex]{ constantColor[_Color] Combine texture * primary DOUBLE, texture * constant }
		}*/
    	Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile FOGON FOGOFF

			#pragma target 2.0
			#include "UnityCG.cginc"

			struct v2f {
    			float4 pos : SV_POSITION;
				half3 floatVec : TEXCOORD0;
    			float2  bumpTexCoord : TEXCOORD1;
    			//half3  viewDir : TEXCOORD2;
    			half3  lightDir : TEXCOORD3;
				half2 buv : TEXCOORD4;
				half3 normViewDir : TEXCOORD5;
				//UNITY_FOG_COORDS(7)
				#ifdef FOGON
				half dist : TEXCOORD7;
				#endif
			};

			half _Size;
			half4 _SunDir;
			half4 _FakeUnderwaterColor;
			#ifdef FOGON
       		uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			uniform half4 unity_FogDensity;
			#endif
			     
			v2f vert (appdata_tan v) {
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz*_Size;///float2(_Size.x, _Size.z)*5;
    			o.pos = UnityObjectToClipPos (v.vertex);
    
    			half3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			half3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				half3x3 rotation = half3x3( v.tangent.xyz, binormal, v.normal );
    
    			half3 viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, half3(_SunDir.xyz));

				o.buv = float2(o.bumpTexCoord.x + _CosTime.x * 0.2, o.bumpTexCoord.y + _SinTime.x * 0.3);

				o.normViewDir = normalize(viewDir);

				o.floatVec = normalize(o.normViewDir - normalize(o.lightDir));
  
				#ifdef FOGON
				//manual fog
				o.dist = (unity_FogEnd.x - length(o.pos.xyz)) / (unity_FogEnd.x - unity_FogStart.x);
				#endif
				//autofog
				//UNITY_TRANSFER_FOG(o, o.pos);

    			return o;
			}

			sampler2D _Bump;
			half4 _WaterColor;
			half4 _SurfaceColor;
			half _Specularity;
			half _SpecPower;
            half4 _SunColor;

			half4 frag (v2f i) : COLOR {

				half3 tangentNormal0 = (tex2D(_Bump, i.buv.xy) * 2.0) -1;
				half3 tangentNormal = normalize(tangentNormal0);

				half4 result = half4(0, 0, 0, 1);
                
				half fresnelTerm = 1.0 - saturate(dot (i.normViewDir, tangentNormal0));
				//half fresnelLookup = dot(tangentNormal,i. normViewDir);
				//float bias = 0.06;
				//float power = 4.0;
				//half fresnelTerm = 0.06 + (1.0-0.06)*pow(1.0 - fresnelLookup, 4.0);

				half specular = pow(max(dot(i.floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) * _SpecPower;
                
				result.rgb = lerp(_WaterColor*_FakeUnderwaterColor, _SunColor.rgb*_SurfaceColor*0.85, fresnelTerm*0.65)  + specular*_SunColor.rgb;


				//fog
				//UNITY_APPLY_FOG(i.fogCoord, result); 
				#ifdef FOGON
				//manual fog (linear) (reduces instructions on d3d9)
				float ff = saturate(i.dist);
				result.rgb = lerp(unity_FogColor.rgb, result.rgb, ff);
				#endif

    			return result;
			}
			ENDCG
			

		}
    }

	//water bump/foam/alpha
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 1
		Blend SrcAlpha OneMinusSrcAlpha
		/*Pass
		{
			//Color(1,0,1,1)
			//Cull Off
			Name "BASE"
			Tags{ "LightMode" = "Always"}
			Cull Off
			Color[_PPLAmbient]
			SetTexture[_MainTex]{ constantColor[_Color] Combine texture * primary DOUBLE, texture * constant }
		}*/
    	Pass {
			//Cull Off | Front | Off
		    Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile SHORE_ON SHORE_OFF
			#pragma multi_compile FOGON FOGOFF

			#pragma target 2.0
			#include "UnityCG.cginc"
			
			struct v2f {
    			float4 pos : SV_POSITION;
				half3 floatVec : TEXCOORD0;
    			float4  bumpTexCoord : TEXCOORD1;
				#ifdef SHORE_ON
				float4 ref : TEXCOORD2;
				#endif
    			half3  lightDir : TEXCOORD3;
				float4 buv : TEXCOORD4;
				half3 normViewDir : TEXCOORD5;
				//UNITY_FOG_COORDS(7)
				#ifdef FOGON
				half dist : TEXCOORD7;
				#endif
			};

			half _Size;
			half _FoamFactor;
			half4 _SunDir;
			half4 _FakeUnderwaterColor;
			#ifdef FOGON
   			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			uniform half4 unity_FogDensity;
			#endif
			         
			v2f vert (appdata_tan v) {
    			v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

    			o.bumpTexCoord.xy = v.vertex.xz*_Size;///float2(_Size.x, _Size.z)*5;
    			o.pos = UnityObjectToClipPos (v.vertex);
    			o.bumpTexCoord.z = v.tangent.w * _FoamFactor;

    			half3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
    			half3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
				half3x3 rotation = half3x3( v.tangent.xyz, binormal, v.normal );
    
    			half3 viewDir = mul(rotation, objSpaceViewDir);
    			o.lightDir = mul(rotation, half3(_SunDir.xyz));

				o.bumpTexCoord.w = _SinTime.y * 0.5;

				o.buv = float4(o.bumpTexCoord.x + _CosTime.x * 0.2, o.bumpTexCoord.y + _SinTime.x * 0.3, o.bumpTexCoord.x + _CosTime.y * 0.04, o.bumpTexCoord.y + o.bumpTexCoord.w);

				o.normViewDir = normalize(viewDir);

				o.floatVec = normalize(o.normViewDir - normalize(o.lightDir));

	    	  	#ifdef SHORE_ON
				o.ref = ComputeScreenPos(o.pos);
				#endif
							
				#ifdef FOGON
				//manual fog
				o.dist = (unity_FogEnd.x - length(o.pos.xyz)) / (unity_FogEnd.x - unity_FogStart.x);
				#endif

				//autofog
				//UNITY_TRANSFER_FOG(o, o.pos);

    			return o;
			}

			sampler2D _Bump;
			sampler2D _Foam;
			half _FoamSize;
			#ifdef SHORE_ON
			uniform sampler2D _CameraDepthTexture;
			sampler2D _FoamGradient;
			half _ShoreDistance;
			half _ShoreStrength;
			#endif
			half4 _WaterColor;//Lod1;
			half4 _SurfaceColor;
            half _WaterLod1Alpha;
			half _Specularity;
			half _SpecPower;
            half4 _SunColor;

			half4 frag (v2f i) : COLOR {

				half3 tangentNormal0 = (tex2D(_Bump, i.buv.xy) * 2.0) + (tex2D(_Bump, i.buv.zw) * 2.0) - 2;
				half3 tangentNormal = normalize(tangentNormal0);

				half4 result = half4(0, 0, 0, 1);
                
				//fresnel
				//#ifdef SHORE_ON
				half fresnelTerm = 1.0 - saturate(dot (i.normViewDir, tangentNormal0));
				//#else
				//half fresnelLookup = dot(tangentNormal, i.normViewDir);
				//float bias = 0.06;
				//float power = 4.0;
				//half fresnelTerm = 0.06 + (1.0 - 0.06)*pow(1.0 - fresnelLookup, 4);
				//half fresnelTerm = UNITY_SAMPLE_1CHANNEL( _Fresnel, float2(fresnelLookup,fresnelLookup) );
				//#endif

				half _foam = tex2D(_Foam, -i.buv.xy *_FoamSize).r;
				half foam = clamp(_foam  - 0.5, 0.0, 1.0) * i.bumpTexCoord.z;

				#ifdef SHORE_ON
				half specular = pow(max(dot(i.floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) * _SpecPower;
				#else
				half specular = pow(max(dot(i.floatVec,  tangentNormal) , 0.0), 250.0 * _Specularity ) *(1.2-foam) * _SpecPower;
				#endif
   
   				//SHORELINES
				#ifdef SHORE_ON
				float zdepth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float intensityFactor = 1 - saturate((zdepth - i.ref.z) / _ShoreDistance);  
				half3 foamGradient = _ShoreStrength - tex2D(_FoamGradient, float2(intensityFactor - i.bumpTexCoord.w, 0) + tangentNormal.xy);
				foam += foamGradient * intensityFactor * _foam;
				#endif
				             
				result.rgb = lerp(_WaterColor*_FakeUnderwaterColor, _SunColor.rgb*_SurfaceColor*0.85, fresnelTerm*0.65) + clamp(foam.r, 0.0, 1.0)*_SunColor.b + specular*_SunColor.rgb;
                result.a = _WaterLod1Alpha;


				//fog
				//UNITY_APPLY_FOG(i.fogCoord, result); 

				#ifdef FOGON
				//manual fog (linear) (reduces instructions on d3d9)
				float ff = saturate(i.dist);
				result.rgb = lerp(unity_FogColor.rgb, result.rgb, ff);

				#endif

    			return result;
			}
			ENDCG

		}
    }

		   
}
