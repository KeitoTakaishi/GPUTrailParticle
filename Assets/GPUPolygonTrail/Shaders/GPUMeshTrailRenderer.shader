Shader "Custom/GPUMeshTrailRenderer"
{
    Properties
    {
		[HDR]
        _Color ("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Tex0 ("Tex0", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_TrailNumsq("TrailNumsq", float) = 128.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
		#pragma target 5.0
		#include "utils.cginc"
		#include "NoiseUtils.cginc"
		#include "Noise4d.cginc"

        sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
			float id;
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
		StructuredBuffer<float2> lifeBuffer;
#endif

		void setup() {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
#endif
		}


		int BLOCK_SIZE;
		uint circleResolution;
		int pulse;
		float trailNum;

		void vert(inout appdata v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			uint _instanceID = (int)v.instanceID;
			int _vid = (int)v.vertexID;

			float4 vert = v.vertex;
			float2 uv = v.texcoord.xy;

			//Trail内のID+それまでのTrail*内部点
			int newVertexID = floor(fmod((float)(_vid / circleResolution), BLOCK_SIZE)) + (_instanceID)* BLOCK_SIZE;


			float3 forward = float3(0.0, 0.0,-1.0);
			float theta = dot(forward, positionBuffer[newVertexID]);
			float axis = cross(float3(normalize(positionBuffer[newVertexID])), forward);
			//vert = mul(Rodrigues(axis, theta), vert);


			float2 scale = lifeBuffer[_instanceID];
			vert = mul(ScaleMatrix(float3(scale.x / scale.y, scale.x / scale.y, scale.x / scale.y)), vert);

			float3 pos = positionBuffer[newVertexID];
			vert = mul(TranslateMatrix(pos), vert);

			//float r = pulseBuffer[newVertexID].y * pulseBuffer[newVertexID].x;

			float3 normal = normalize(v.vertex.xyz);
			normal.z = 0.0;
			//float random = rnd(float2(fmod(_vid, circleResolution), 0.0));
			//float3 anim = normalize(normal) * r * random;
			//vert = mul(TranslateMatrix(anim), vert);


			//instanceID offSet
			//vert = mul(TranslateMatrix(float3(5.0*(_instanceID*2.0 - trailNum), 5.0, 0.0)), vert);
			
			//vert = mul(ScaleMatrix(float3(scale.x.xxx)), vert);
			
			
			v.vertex = vert;
			
			
			o.id = (float)_instanceID;
#endif
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _Tex0;
		float _TrailNumsq;
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			
			//fixed4 c = tex2D(_MainTex, uv) * _Color;
			//c = _Color;
			
			fixed4 c =  _Color;
			//float2 uv = float2(fmod(IN.id, 128.0) / 128.0, (IN.id / 128.0) / 128.0);
			float2 uv = float2(fmod(IN.id, 128.0) / 128.0, (IN.id / 128.0) / 128.0) ;
			float2 delta = float2(1.0 / 128.0, 1.0 / 128.0);
			uv.x += 0.5 * delta.x ;
			uv.y -= 0.5 * delta.y;
			fixed4 texCol = tex2D(_Tex0, uv) * c;
			//fixed4 texCol = tex2D(_Tex0, uv);
			//o.Albedo = float3(fmod(IN.id, 128.0) / 128.0, 0., 0.0);
			//o.Albedo = float3(fmod(IN.id, 128.0)/128.0, (IN.id / 128.0)/128.0, 0.0);
			
			o.Albedo = texCol;
			//o.Albedo = float4(1.0, 0.0, 0.0, 1.0);
			
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}