Shader "Custom/TexBlend"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
		_SubTex("SubTex", 2D) = "white" {}
		_NoiseTex("NoiseTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SubTex;
		sampler2D _NoiseTex;

		struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			fixed4 c1 = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c2 = tex2D(_SubTex, IN.uv_MainTex);
			fixed4 p = tex2D(_NoiseTex, IN.uv_MainTex);
			o.Albedo = lerp(c1, c2, p);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
