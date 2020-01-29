Shader "Custom/Grass"
{
    Properties
    {
        _MainTex ("main", 2D) = "white" {}
		_NoiseTex("noise", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Threshold("Threshold", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _NoiseTex;
		fixed4 _Color;
		half _Threshold;

        struct Input
        {
            float2 uv_MainTex;
        };

		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 m = tex2D(_NoiseTex, IN.uv_MainTex);
			half g = m.r * 0.2 + m.g * 0.7 + m.b * 0.1;
			if (g < _Threshold) {
				discard;
			}

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Smoothness = 0;
			o.Alpha = c.a;
		}
        ENDCG
    }
    FallBack "Diffuse"
}
