Shader "Unlit/Deco_Stencil"
{
	Properties
    {
		_Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
		Tags { "RenderType" = "Transparent" "Queue" = "Geometry+1"}
        LOD 100

		Pass
		{
			ZWrite ON
			ColorMask 0
		}

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha

		Stencil {
			Ref 0  // リファレンス値
			Comp Equal // ピクセルのリファレンス値がバッファの値と等しい場合のみレンダリングします。
		}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
		
		struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }

		Pass {
				Stencil {
			Ref 2  // リファレンス値
			Comp Equal // ピクセルのリファレンス値がバッファの値と等しい場合のみレンダリングします。
		}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

				#pragma multi_compile_fog

				#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			v2f vert(appdata v)
			{
				v2f o; 
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}
			half4 frag(v2f i) : SV_Target
			{
				fixed4 texCol = tex2D(_MainTex, i.uv);

				UNITY_APPLY_FOG(i.fogCoord, col);

				float sum = min(min(texCol.r, texCol.g), min(texCol.r, texCol.b)) + max(max(texCol.r, texCol.g), max(texCol.r, texCol.b));

				return fixed4(max((sum - texCol.r) - 0.3, 0.0), max((sum - texCol.g) - 0.3, 0.0), max((sum - texCol.b) - 0.3, 0.0), 1.0);
				//return fixed4(0.3, 0.3, 0.3, texCol.a);
			}
			ENDCG
		}
    }
}
