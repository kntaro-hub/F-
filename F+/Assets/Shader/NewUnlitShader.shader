Shader "Custom/sample" 
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader
	{
		Stencil {
				Ref 2 // リファレンス値
				Comp Always  // 常にステンシルテストをパスさせます。
				Pass Replace    // リファレンス値をバッファに書き込みます。
			}
			Tags { "Queue" = "Transparent" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard
			#pragma target 3.0

			fixed4 _Color;
			sampler2D _MainTex;

			struct Input 
	{
				float3 worldNormal;
					float3 viewDir;
					float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// 色
				o.Albedo = _Color;
				float d = (1.0 - (abs(dot(IN.viewDir, IN.worldNormal)))) * 1.4;
				o.Albedo = fixed4(max(0.0, o.Albedo.r - d), max(0.0, o.Albedo.g - d), max(0.0, o.Albedo.b - d), 1.0f);
				o.Alpha = 1.0;

				// ライティング
				o.Emission = o.Albedo.rgb;
				o.Metallic = 0.0;
			}
			ENDCG
	}
		FallBack "Diffuse"
}
