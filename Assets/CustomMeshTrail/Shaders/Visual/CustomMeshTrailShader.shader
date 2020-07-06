Shader "Custom/CustomMeshTrailShader"
{
	Properties
	{
		[HDR]
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Cull off
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
		#pragma target 5.0
		#include "../utils.cginc"
		
		//#include "../NoiseUtils.cginc"
		//#include "../Noise4d.cginc"

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


		int trailPointNum;
		uint circleResolution;
		float trailNum;

		void vert(inout appdata v) {
	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			uint _instanceID = (int)v.instanceID;
			int _vid = (int)v.vertexID;

			float4 vert = v.vertex;
			float2 uv = v.texcoord.xy;


			//1mesh内でのID
			int IDinMesh = floor(fmod((float)(_vid / circleResolution), trailPointNum));
			//インスタンス全体で何番目の中心ポイントなのか
			int newVertexID = IDinMesh + (_instanceID)* trailPointNum;


			float3 forward = float3(0.0, 0.0, -1.0);
			float theta = dot(forward, positionBuffer[newVertexID]);
			float axis = cross(float3(normalize(positionBuffer[newVertexID])), forward);
			//vert = mul(Rodrigues(axis, theta), vert);


			float3 pos = positionBuffer[newVertexID];
			//vert = mul(TranslateMatrix(pos), vert);

			//float r = pulseBuffer[newVertexID].y * pulseBuffer[newVertexID].x;

			float3 normal = normalize(v.vertex.xyz);
			normal.z = 0.0;
			//float random = rnd(float2(fmod(_vid, circleResolution), 0.0));
			//float3 anim = normalize(normal) * r * random;
			//vert = mul(TranslateMatrix(anim), vert);


			//instanceID offSet
			float x = fmod(_instanceID, 30.0);
			float y = _instanceID / 30.0;
			//vert = mul(TranslateMatrix(float3(5.0*(_instanceID*2.0 - trailNum), 5.0, 0.0)), vert);
			vert = mul(TranslateMatrix(float3(x*10.0, 5.0, y*50.0)), vert);
			v.vertex = vert;
	#endif
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)

		{
			float2 uv = IN.uv_MainTex;
			fixed4 c = tex2D(_MainTex, uv) * _Color;
			c = _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
			FallBack "Diffuse"
}
