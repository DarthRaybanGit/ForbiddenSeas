﻿Shader "Custom/Ocena" {
	Properties
	{
		_GradientTex("Gradient", 2D) = "white" {}
	_GradientNorm("GradientNormal", 2D) = "normal" {}

	_ColorTint("Gradient", 2D) = "white" {}
	}
		SubShader
	{
		Pass
	{
		Tags
	{
		//"LightMode" = "ForwardBase"
		"Queue" = "Transparent-1"
	}

		LOD 200

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"

#define PI 3.14159265358979323846

		struct appdata
	{
		float4 vertex : POSITION;
		float4 texcoord : TEXCOORD0;
		float4 texcoord1 : TEXCOORD1;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;

		float2 uv_GradientTex : TEXCOORD0;
		float2 uv_GradientNorm : TEXCOORD1;

		float4 color : TEXCOORD2;
		float bump : COLOR1;
	};

	sampler2D _GradientTex;
	sampler2D _GradientNorm;

	float4 _ColorTint;
	half _Glossiness;
	half _Metallic;

	float rand(float2 co)
	{
		return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
	}

	float rand(float2 co, float l)
	{
		return rand(float2(rand(co), l));
	}

	float rand(float2 co, float l, float t)
	{
		return rand(float2(rand(co, l), t));
	}

	float perlin(float2 p, float dim, float time)
	{
		float2 pos = floor(p * dim);
		float2 posx = pos + float2(1.0, 0.0);
		float2 posy = pos + float2(0.0, 1.0);
		float2 posxy = pos + float2(1.0, 1.0);

		float c = rand(pos, dim, time);
		float cx = rand(posx, dim, time);
		float cy = rand(posy, dim, time);
		float cxy = rand(posxy, dim, time);

		float2 d = frac(p * dim);
		d = -0.5 * cos(d * PI) + 0.5;

		float ccx = lerp(c, cx, d.x);
		float cycxy = lerp(cy, cxy, d.x);
		float center = lerp(ccx, cycxy, d.y);

		return center * 2.0 - 1.0;
	}

	float perlin(float2 p, float dim)
	{
		return perlin(p, dim, 0.0);
	}

	v2f vert(appdata  v)
	{
		v2f o;
		float delta = 1;

		float x = v.vertex.x;
		float y = v.vertex.y;

		float perl = perlin(float2(x + delta * _Time.w, y + delta * _Time.w), 0.1);
		perl += perlin(float2(x + delta * _Time.z, y + delta * _Time.z), 0.5);
		perl += perlin(float2(x + delta * _SinTime.z, y + delta * _SinTime.z), 1);
		perl += perlin(float2(x + delta * _SinTime.x, y + delta * _SinTime.x), 2);

		perl /= 4;
		perl = (perl + 1) / 2;

		o.vertex = mul(UNITY_MATRIX_MVP, float4(v.vertex.x, v.vertex.y , v.vertex.z + perl, v.vertex.w));

		o.color = _ColorTint;
		o.bump = perl;

		return o;
	}

	float4 frag(v2f i) : SV_Target
	{
		return tex2D(_GradientTex, i.uv_GradientTex);
	}
		ENDCG
	}
	}
		Fallback "Diffuse"
}