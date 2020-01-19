Shader "Custom/CircleDraw"
{
	Properties
	{
		posX("PositionX", float) = 0.0
		posY("PositionY", float) = 0.0
		posZ("PositionZ", float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard 
		#pragma target 3.0

		struct Input 
		{
			// ワールド座標を利用するため
			float3 worldPos;
		};

		float posX;
		float posY;
		float posZ;

		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			// 原点から描画位置の距離
			float dist = distance(fixed3(posX, posY, posZ), IN.worldPos);

			// 距離が2以上なら色を付ける
			float val = abs(sin(dist * 10.0 - _Time * 500));
			if (val > 0.98) 
			{
				o.Albedo = fixed4(1, 1, 1, 1);
			}
			else 
			{
				 o.Albedo = fixed4(110 / 255.0, 87 / 255.0, 139 / 255.0, 1);
			}
		}		
		ENDCG
	}
    FallBack "Diffuse"
}
