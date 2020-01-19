Shader "Custom/test" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "white"{}
	}
	SubShader
	{
		// 透明なのでTransparent
		//Tags { "Queue" = "Transparent" }

		// 透明じゃないのでOpaque
		Tags { "Queue" = "Transparent" }
		LOD 200

		// 開始
		CGPROGRAM

		// 設定
		//#pragma surface surf Standard alpha:fade	// 透明
		#pragma surface surf Standard fullforwardshadows // 普通
		#pragma target 3.0							// 知らん

		// 入力情報
		struct Input 
		{
			// 貼るテクスチャ
			float2 uv_MainTex;

			// 法線
			float3 worldNormal;

			// 視線ベクトル
			float3 viewDir;
		};

		// 変数宣言

		// テクスチャ
		sampler2D _MainTex;

		// メインのシェーダー処理
		
		// -= 透明 =- //
		//void surf(Input IN, inout SurfaceOutputStandard o)
		//{
		//	// 色
		//	o.Albedo = fixed4(1, 1, 1, 1);

		//	// 視線ベクトルと法線の内積
		//	// これで輪郭部分が強調される
		//	float alpha = 1 - (abs(dot(IN.viewDir, IN.worldNormal)));

		//	// 調整
		//	o.Alpha = alpha * 1.5f;
		//}

		// -= rim光 =- //
		//void surf(Input IN, inout SurfaceOutputStandard o)
		//{
		//	// 元の色
		//	fixed4 baseColor = fixed4(0.05, 0.1, 0, 1);

		//	// 輪郭光の色
		//	fixed4 rimColor = fixed4(0.5, 0.7, 0.5, 1);

		//	// 色
		//	o.Albedo = baseColor;

		//	// 視線ベクトルと法線の内積
		//	// これで輪郭部分が強調される
		//	float rim = 1 - (saturate(dot(IN.viewDir, o.Normal)));

		//	// 光を調整
		//	o.Emission = rimColor * pow(rim, 3);
		//}

		// -= テクスチャ =- //
		//void surf(Input IN, inout SurfaceOutputStandard o)
		//{
		//	// 出力色をテクスチャのUV座標にある色に変更
		//	o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
		//}

		// -= ステンドグラス =- //
		//void surf(Input IN, inout SurfaceOutputStandard o)
		//{
		//	fixed4 color = tex2D(_MainTex, IN.uv_MainTex);
		//	o.Albedo = color.rgb;
		//	o.Alpha = (color.r * 0.3 + color.g * 0.6 + color.b * 0.1 < 0.2) 
		//		? 1 : 0.7;
		//}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// UV座標を取得
			fixed2 uv = IN.uv_MainTex;

			// 時間をもとにUVをずらす
			uv.x += 0.1 * _Time;
			uv.y += 0.2 * _Time;

			// テクスチャからUV座標位置の色を取得
			o.Albedo = tex2D(_MainTex, uv);
		}

		// 終了
		ENDCG
	}
	FallBack "Diffuse"
}