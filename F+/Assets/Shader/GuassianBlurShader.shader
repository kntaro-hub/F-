Shader "Custom/GuassianBlurShader"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{ 
		CGINCLUDE
		#include "UnityCG.cginc"

		// 変数宣言
		sampler2D _MainTex;	
		half4 _MainTex_TexelSize;
		float _BlurSize;

		// フラグメントシェーダー用構造体
		struct v2f
		{
			float4 pos : SV_POSITION;
			half2 uv[5] : TEXCOORD0;
		};

		// 垂直頂点シェーダ
		v2f vertBlurVertical(appdata_img v) 
		{
			v2f o;

			// 出力頂点位置は、モデルビューの投影行列に頂点位置を掛けたものです。つまり、3次元空間の座標が2次元ウィンドウに投影されます。
			o.pos = UnityObjectToClipPos(v.vertex);

			// テクスチャ座標を取得
			half2 uv = v.texcoord;
			
			// _MainTex_TexelSize.y各ピクセルのサイズuv配列には、この座標の上下にある2つのピクセルの座標が格納されます。
			o.uv[0] = uv;
			o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
			o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
					 
			return o;
		}
		
		// 水平頂点シェーダ
		v2f vertBlurHorizontal(appdata_img v) 
		{
			v2f o;

			// 上と同じ
			o.pos = UnityObjectToClipPos(v.vertex);

			// テクスチャ座標を取得
			half2 uv = v.texcoord;

			// _MainTex_TexelSize.y各ピクセルのサイズuv配列には、この座標の左右にある2つのピクセルの座標が格納される
			o.uv[0] = uv;
			o.uv[1] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[2] = uv - float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[3] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
			o.uv[4] = uv - float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
					 
			return o;
		}

		// フラグメントシェーダ
		fixed4 fragBlur(v2f i) : SV_Target 
		{
			// フラグメントの最終色は、ソースカラーと周囲のフラグメントカラーの割合の影響を受ける
			float weight[3] = {0.4026, 0.2442, 0.0545};

			// ソースチップの色と比重の積を取得
			fixed3 sum = tex2D(_MainTex, i.uv[0]).rgb * weight[0];

			// 周囲の色を合成
			for (int it = 1; it < 3; it++) 
			{
				sum += tex2D(_MainTex, i.uv[it * 2 - 1]).rgb * weight[it];
				sum += tex2D(_MainTex, i.uv[it * 2]).rgb * weight[it];
			}
			
			// 結果を返す
			return fixed4(sum, 1.0);
		}
		ENDCG

		// 深度テストをオン、クロップと深度書き込みをオフ
		ZTest Always Cull Off ZWrite Off
		

		// 2パス
		Pass 
		{
			NAME "GAUSSIAN_BLUR_VERTICAL"
			CGPROGRAM
			// コンパイラに頂点シェーダー関数とフラグメントシェーダー関数の名前を知らせる
			#pragma vertex vertBlurVertical  
			#pragma fragment fragBlur
			  
			ENDCG  
		}
		
		Pass
		{  
			NAME "GAUSSIAN_BLUR_HORIZONTAL"
			CGPROGRAM  
			// コンパイラに頂点シェーダー関数とフラグメントシェーダー関数の名前を知らせる
			#pragma vertex vertBlurHorizontal  
			#pragma fragment fragBlur
			
			ENDCG
		}	
	}
	Fallback "Diffuse"

}
