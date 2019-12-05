Shader "Custom/Instancing_GPUTrailParticle"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
		#pragma target 3.0
		#include "../utils.cginc"

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent :TANGENT0;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			float2 texcoord2 : TEXCOORD2;
			uint instanceID : SV_InstanceID;
			uint vertexID : SV_VertexID;
		};


#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		StructuredBuffer<float3> positionBuffer;
#endif

		void setup() {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
#endif
		}


		int BLOCK_SIZE;
		int NumPerFace;
		void vert(inout appdata v) {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			uint _instanceID = (int)v.instanceID;
			int _vid = (int)v.vertexID;

			float4 vert = v.vertex;
			float2 uv = v.texcoord.xy;





			int newVertexID = _vid / 2;
			newVertexID = (int)(fmod((float)(_vid / 2), BLOCK_SIZE)) + (_instanceID)* BLOCK_SIZE;

			if (fmod(_vid, BLOCK_SIZE) != 0.0) {
				float3 dir = positionBuffer[newVertexID - 1] - positionBuffer[newVertexID];
				float theta = atan2(dir.y, dir.z);
				vert = mul(RotXMatrix(theta), vert);
			}
			else {
				float theta = atan2(positionBuffer[newVertexID].y, positionBuffer[newVertexID].z);
				vert = mul(RotXMatrix(theta), vert);
			}

			float3 pos = positionBuffer[newVertexID];

			//float4x4 object2world = (float4x4)0.0;
			//object2world._11_22_33_44 = float4(1.0.xxxx);
			//object2world._14_24_34 += offSet.xyz;
			//vert = mul(object2world, vert);
			vert = mul(TranslateMatrix(pos), vert);
			v.vertex = vert;
#endif
		}

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float2 uv = IN.uv_MainTex;
			uv.y *= 0.7;
			uv.x *= 4.0;
			uv.x = frac(uv.x);
            fixed4 c = tex2D (_MainTex, uv) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
