
//	Vertex Functions

	float2 _WindMultiplier;

//	Simple random function
	inline float nrand(float2 pos) {
		//return frac(sin(dot(pos, half2(12.9898f, 78.233f))) * 43758.5453f);
		return frac((dot(pos, half2(12.9898f, 78.233f))) );
	}

//	Our vertex function which handles wind and culling
#if !defined(DEPTHNORMAL)
	void vertgrass(inout appdata_grassinstanced v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input, o);
#else
	void vertgrass(inout appdata_full v, in float3 terrainNormal, in float InstanceScale) {
#endif

	// 	Scale contains some perlin noise – so we use it to add any perlin noise based variation
		//float3 unitvec = mul( (float3x3 )unity_ObjectToWorld, float3(1,0,0)); // float4 would be 0,1,0, 0 !!!!!
		//float scale = length( unitvec );
		float scale = InstanceScale;

		float3 pivot = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
		float3 dist = pivot
					  - scale.xxx * 4 /* lets break up the boring distance*/ 
					  - _WorldSpaceCameraPos.xyz;
		float SqrDist = dot(dist, dist);

	//	Calculate far fade factor
		#if defined (UNITY_PASS_SHADOWCASTER)
			float fade = 1;
		//	Depth Pass
			if (unity_LightShadowBias.z == 0.0) {
				fade = saturate(( _AtgGrassFadeProps.x - SqrDist) * _AtgGrassFadeProps.y);
			}
		//	Shadow Pass
			else {
			// TODO: Check why i can't revert this as well? Clip?
				fade = 1.0f - saturate((SqrDist - _AtgGrassShadowFadeProps.x) * _AtgGrassShadowFadeProps.y);
			}
		#else
			float fade = saturate(( _AtgGrassFadeProps.x - SqrDist) * _AtgGrassFadeProps.y);
		#endif
	//	Cull based on far culling distance
		if (fade == 0.0f) {
			v.vertex.y -= 10000.0f;
			//v.vertex = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
			return;
		} 

	//	Get some random value per instance
		fixed random = nrand(pivot.xz);

	//	Calculate near fade factor / reversed!
		float smallScaleClipping = saturate(( SqrDist - _AtgGrassFadeProps.z) * _AtgGrassFadeProps.w);
		float clip = (random < _Clip)? 1 : 0;
	//	Debug: Colorize instances which would be culled early on
		#if defined(_PARALLAXMAP)
			color.rgb = lerp(o.color.rgb, _DebugColor, clip);
		#endif
		clip = 1.0f - smallScaleClipping * clip;
	//	Cull based on near culling distance
		if ( clip == 0.0f) {
			v.vertex.y -= 10000.0f;
			//v.vertex = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
			return;
		}
		fade *= clip;

	//	Set color variation

		float normalizedScale = (scale - _MinMaxScales.x) * _MinMaxScales.y;
		//#if defined(_MIXMODE_SOFTMERGE)
		normalizedScale = saturate(normalizedScale);
		//#endif

		fixed4 color = 1;
		#if defined(GRASSUSESTEXTUREARRAYS) && defined(_DETAIL_MULX2)
			color *= lerp(_HealthyColor, _DryColor, nrand(pivot.zx));
		#else
			color *= lerp(_HealthyColor, _DryColor, normalizedScale);
		#endif

	//	Set random bend strength
		float mainBending = v.color.a * color.a;

	//	Apply fading – in fact doing it this way even speeds up rendering
		v.vertex.xyz = lerp(v.vertex.xyz, float3(0, 0, 0), 1.0 - fade);

	//	---------------------
	//	Wind
		#if defined(_METALLICGLOSSMAP)
		//	Read wind at pivot
			float4 wind = tex2Dlod(_AtgWindRT, float4( pivot.xz * _AtgWindDirSize.w + (1 - v.color.r).xx + scale * 0.025, 0, _WindLOD) );
		#else
		//	Read wind texture at vertex world position
			float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float4 wind = tex2Dlod(_AtgWindRT, float4( wPos.xz * _AtgWindDirSize.w + (1 - v.color.r).xx + scale * 0.025, 0, _WindLOD) );
		#endif

		wind.r = wind.r   *   (wind.g * 2.0f - 0.24376f  /* not a "real" normal as we want to keep the base direction*/);

		//	If not procedural instanced drawn swap direction as we have a proper WorldToObject matrix
		#if !defined (UNITY_PROCEDURAL_INSTANCING_ENABLED)
			wind.r *= -1;
		#endif

	//	Add bending from wind
		float windStrength = wind.r * _AtgWindStrengthMultipliers.x * _WindMultiplier.x * mainBending;
		
		float3 bend = UnityWorldToObjectDir(_AtgWindDirSize.xyz) * windStrength;
#if !defined(DEPTHNORMAL)		
		v.vertex.xz -= bend.xz;
#else
		v.vertex.xz += bend.xz;
#endif

	//	Add none directional "jitter" – this helps to hide the quantized wind from the texture lookup.
		float2 jitter = lerp( float2 (_AtgSinTime.x, 0), _AtgSinTime.yz, float2(random, windStrength) );
#if !defined(DEPTHNORMAL)	
		v.vertex.xz +=
			(jitter.x + jitter.y * _WindMultiplier.y)
			* (0.075 + _AtgSinTime.w) * saturate(windStrength)
		;
#else
	//	Crazy?!
		float jitterFactor = (0.075 + _AtgSinTime.w) * saturate(windStrength);
		v.vertex.zx += jitter.y * _WindMultiplier.y * jitterFactor;
		v.vertex.xz -= jitter.x * jitterFactor;
#endif		 
			

	//	Get/set normal
		#if defined(_NORMAL)
			v.normal = terrainNormal;
		#else
			v.normal = half3(0,1,0);
		#endif
		
	//	Bend normal
		v.normal.xz -= (bend.xz * UNITY_PI) * _NormalBend;
		// per pixel normalize is applied in lighting function // v.normal = normalize(v.normal);

	//	Derive ambient occlusion from baked bending
		#if !defined(DEPTHNORMAL)
			o.occ = saturate( 0.85 + v.color.a);
		#endif

	//	Calcualte texture array layer based on a new random value	
		#if defined(GRASSUSESTEXTUREARRAYS)
			#if defined(_MIXMODE_BYSIZE)
				o.layer = floor(normalizedScale * _Layers + _SizeThreshold);
			#elif defined(_MIXMODE_RANDOM)
			//	We must not use the same random value here as for the culling!
				random = nrand(pivot.zx);
				o.layer = floor(random * _Layers + 0.5 );
				// here middle texture wins most of the time...
				// o.layer = floor( (random + normalizedScale) * 0.5 * _Layers + 0.5);
			#endif
		#endif
	
	//	Store smoothness variation
		scale = lerp(0.6, 1.0, random);
		color.a = v.color.a;

	//	Set outputs needed by the Surface shaders
		#if !defined(DEPTHNORMAL)
			o.scale = scale;
			o.color = color;
		#endif
	}

//	----------------------------------------------
//	Currently unsused functions

	void rotate2D(inout float2 v, float r) {
		float s, c;
		sincos(r, s, c);
		v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
	}

	// http://answers.unity3d.com/questions/218333/shader-inversefloat4x4-function.html
	inline float4x4 inverseMat(float4x4 input) {
    	#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
		float4x4 cofactors = float4x4(
        	minor(_22_23_24, _32_33_34, _42_43_44), 
           -minor(_21_23_24, _31_33_34, _41_43_44),
        	minor(_21_22_24, _31_32_34, _41_42_44),
           -minor(_21_22_23, _31_32_33, _41_42_43),
         
           -minor(_12_13_14, _32_33_34, _42_43_44),
        	minor(_11_13_14, _31_33_34, _41_43_44),
           -minor(_11_12_14, _31_32_34, _41_42_44),
        	minor(_11_12_13, _31_32_33, _41_42_43),
         
        	minor(_12_13_14, _22_23_24, _42_43_44),
           -minor(_11_13_14, _21_23_24, _41_43_44),
        	minor(_11_12_14, _21_22_24, _41_42_44),
           -minor(_11_12_13, _21_22_23, _41_42_43),
         
           -minor(_12_13_14, _22_23_24, _32_33_34),
         	minor(_11_13_14, _21_23_24, _31_33_34),
           -minor(_11_12_14, _21_22_24, _31_32_34),
        	minor(_11_12_13, _21_22_23, _31_32_33)
     	);
     	#undef minor
     	return transpose(cofactors) / determinant(input);
    }