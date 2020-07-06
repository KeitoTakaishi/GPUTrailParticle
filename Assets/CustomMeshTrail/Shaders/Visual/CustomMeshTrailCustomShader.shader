Shader "Unlit/CustomMeshTrailCustomShader"
{
	

	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{

		Tags { "RenderType" = "Opaque" }

		CGINCLUDE

		#include "UnityCG.cginc"
		#include "../utils.cginc"
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			UNITY_FOG_COORDS(1)
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;

		v2f vert(appdata v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			UNITY_TRANSFER_FOG(o,o.vertex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = tex2D(_MainTex, i.uv);
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}

		ENDCG
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			ENDCG
		}
	}
}
