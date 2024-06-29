
void perturb_normal_float(float3 localNormal, float3 N, float3 V, float2 uv, out float3 output)
{
	float3 dp1 = ddx(-V);
	float3 dp2 = ddy(-V);
	float2 duv1 = ddx(uv);
	float2 duv2 = ddy(uv);

	float3 dp2perp = cross(dp2, N);
	float3 dp1perp = cross(N, dp1);
	float3 T = dp2perp * duv1.x + dp1perp * duv2.x;
	float3 B = dp2perp * duv1.y + dp1perp * duv2.y;

	float invmax = rsqrt(max(dot(T, T), dot(B, B)));
	float3 TinvMax = normalize(T * invmax);
	float3 BinvMax = normalize(B * invmax);

	float3x3 TBN = float3x3(float3(TinvMax.x, BinvMax.x, N.x), float3(TinvMax.y, BinvMax.y, N.y), float3(TinvMax.z, BinvMax.z, N.z));

	output = normalize(mul(TBN, localNormal));
}


void fun2_float(
	float2 uv2_SplatTex,
	float4 _SplatTex_TexelSize,
	float _SplatEdgeBumpWidth,

	out float clipDistHard,
	out float clipDistSoft
)
{
	// Use ddx ddy to figure out a max clip amount to keep edge aliasing at bay when viewing from extreme angles or distances
	float splatDDX = length(ddx(uv2_SplatTex * _SplatTex_TexelSize.zw));
	float splatDDY = length(ddy(uv2_SplatTex * _SplatTex_TexelSize.zw));
	float clipDist = sqrt(splatDDX * splatDDX + splatDDY * splatDDY);
	clipDistHard = max(clipDist * 0.01, 0.01);
	clipDistSoft = 0.01 * _SplatEdgeBumpWidth;
}
