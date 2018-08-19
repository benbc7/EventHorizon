Shader ".CustomShaders/ReactiveBubbleShieldWithTexture" {
	Properties {
		_Position("Collision", Vector) = (-1, -1, -1, -1)
		_EffectSize("Effect Size", float) = 2
		_Color("Color", Color) = (1, 1, 1, 1)
		_EffectTime("Effect Time (ms) for back end", float) = 0
		_EffectIntensity("Effect Intensity", float) = 1.0

		_Inside("_Inside", Range(0.0,2.0)) = 0.0
		_Rim("_Rim", Range(0.0,1.0)) = 1.2
		_Texture("_Texture", 2D) = "white" {}
		_Speed("_Speed", Range(0.5,10.0)) = 0.5
		_Tile("_Tile", Range(1.0,10.0)) = 5.0
		_Strength("_Strength", Range(0.0,5.0)) = 1.5
	}
	SubShader {
		Tags {
			//render queue (after all others)
			"Queue" = "Transparent"
			//not affected by projectors
			"IgnoreProjector" = "True"
			//categorizes shader into group (opaque, alpha-tested, TRANSPARENT)
			"RenderType" = "Transparent"
		}
		//cull back faces
		Cull Back
		//turn off depth buffer
		ZWrite Off
		ZTest LEqual
		
		CGPROGRAM
		//specify shaders
		#pragma surface surf BlinnPhongEditor alpha vertex:vert alpha

		//an edit of the built in SurfaceOutput struct to make them half3's instead
		struct EditorSurfaceOutput {
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half3 Gloss;
			half Specular;
			half Alpha;
		};

		//structure for getting input from script
		struct Input {
			float currentDist;
			float4 screenPos;
			float3 viewDir;
			float2 uv_Texture;
		};

			//creating necessary variables for the shader
			float4 _Position;
			float _EffectSize;
			float4 _Color;
			float _EffectTime;
			float _EffectIntensity;

			sampler2D _CameraDepthTexture;
			fixed _Inside;
			fixed _Rim;
			sampler2D _Texture;
			fixed _Speed;
			fixed _Tile;
			fixed _Strength;

		inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light) {
			half3 spec = light.a * s.Gloss;

			half4 c;

			c.rgb = (s.Albedo * light.rgb + light.rgb * spec);

			c.a = s.Alpha + Luminance(spec);

			return c;


		}

		inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			viewDir = normalize(viewDir);
			half3 h = normalize(lightDir + viewDir);

			half diff = max(0, dot(s.Normal, lightDir));

			float nh = max(0, dot(s.Normal, h));
			float3 spec = pow(nh, s.Specular*128.0) * s.Gloss;

			half4 res;
			res.rgb = _LightColor0.rgb * (diff * atten * 2.0);
			res.w = spec * Luminance(_LightColor0.rgb);

			return LightingBlinnPhongEditor_PrePass(s, res);
		}

		//vertex shader
		void vert (inout appdata_full IN, out Input OUT) {
			UNITY_INITIALIZE_OUTPUT(Input, OUT);
			//get the distance between the collision point and the current vertex
			OUT.currentDist = distance(_Position.xyz, IN.vertex.xyz);
		}

		void surf (Input IN, inout EditorSurfaceOutput OUT) {
			OUT.Albedo = fixed3(0.0,0.0,0.0);
			OUT.Normal = fixed3(0.0,0.0,1.0);
			OUT.Emission = 0.0;
			OUT.Gloss = 0.0;
			OUT.Specular = 0.0;
			OUT.Alpha = 1.0;
			float4 ScreenDepthDiff0 = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)).r) - IN.screenPos.z;
			float4 Saturate0 = fixed4(0.3,0.3,0.3,1.0);
			float4 Fresnel0_1_NoInput = fixed4(0,0,1,1);
			float dNorm = 1.0 - dot(normalize(float4(IN.viewDir, 1.0).xyz), normalize(Fresnel0_1_NoInput.xyz));
			float4 Fresnel0 = float4(dNorm,dNorm,dNorm,dNorm);
			float4 Step0 = step(Fresnel0,float4(1.0, 1.0, 1.0, 1.0));
			float4 Clamp0 = clamp(Step0,_Inside.xxxx,float4(1.0, 1.0, 1.0, 1.0));
			float4 Pow0 = pow(Fresnel0,(_Rim).xxxx);
			float4 Multiply5 = _Time * _Speed.xxxx;
			float4 UV_Pan0 = float4((IN.uv_Texture.xyxy).x,(IN.uv_Texture.xyxy).y + Multiply5.x,(IN.uv_Texture.xyxy).z,(IN.uv_Texture.xyxy).w);
			float4 Multiply1 = UV_Pan0 * _Tile.xxxx;
			float4 Tex2D0 = tex2D(_Texture,Multiply1.xy);
			float4 Multiply2 = Tex2D0 * _Strength.xxxx;
			float4 Multiply0 = Pow0 * Multiply2;
			float4 Multiply3 = Clamp0 * Multiply0;
			float4 Multiply4 = Saturate0 * Multiply3;
			OUT.Emission = Multiply3.xyz * _Color.rgb;
			OUT.Alpha = Multiply3.w * _Color.a * (max(0.0, (_EffectIntensity * ((_EffectTime / 1) - (IN.currentDist / _EffectSize)))));
		}
		ENDCG
	}
	FallBack "Transparent/Diffuse"
}
