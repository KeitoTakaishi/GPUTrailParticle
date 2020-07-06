Shader "Hidden/TransitionMaterial"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Tex("Tex", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			sampler2D _MainTex;
			sampler2D _Tex;
			float power;
            fixed4 frag (v2f i) : SV_Target
            {
				float2 uv = i.uv;
				fixed4 col = tex2D(_MainTex, i.uv);
				//float disp = tex2D(_MainTex, float2(uv.x * 10.0, uv.y) ).r;
				//col = tex2D(_MainTex, i.uv + float2(disp * power, 0.0));
				//col = tex2D(_MainTex, i.uv + disp * power);
				col.rgb *= power;
				return col;
            }
            ENDCG
        }
    }
}
