// Unlit texture shader which casts shadow on Forward/Defered

Shader "Custom/UnlitShadow"
	{
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader 
	{
		Stencil 
		{
			Ref 0  // リファレンス値
			Comp Equal // ピクセルのリファレンス値がバッファの値と等しい場合のみレンダリングします。
		}

		Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1"}
		LOD 100
		
		Pass {
			Lighting Off
			SetTexture [_MainTex] { combine texture } 
		}
		
		// Pass to render object as a shadow caster
		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
	
			struct v2f { 
				V2F_SHADOW_CASTER;
			};
	
			v2f vert( appdata_base v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}
	
			float4 frag( v2f i ) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}

	}
	
}