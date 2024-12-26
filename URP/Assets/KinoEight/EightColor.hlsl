// 2x2 Bayer matrix for dithering
static const float bayer2x2[] = {-0.5, 0.16666666, 0.5, -0.16666666};

void EightColorCore_float(float4 Source,
                          float2 RawUV,
                          float4x4 Palette1,
                          float4x4 Palette2,
                          float Dither,
                          float Downsample,
                          out float4 Output)
{
    // Linear -> sRGB
    float3 col = LinearToSRGB(Source.rgb);

    // Dithering (2x2 bayer)
    const uint2 pss = RawUV / Downsample;
    const float dither = bayer2x2[(pss.y & 1) * 2 + (pss.x & 1)];
    col.rgb += dither * Dither;

    // Alias for each color
    const float3 c1 = Palette1[0].rgb;
    const float3 c2 = Palette1[1].rgb;
    const float3 c3 = Palette1[2].rgb;
    const float3 c4 = Palette1[3].rgb;
    const float3 c5 = Palette2[0].rgb;
    const float3 c6 = Palette2[1].rgb;
    const float3 c7 = Palette2[2].rgb;
    const float3 c8 = Palette2[3].rgb;

    // Euclidean distance
    const float d1 = distance(c1, col);
    const float d2 = distance(c2, col);
    const float d3 = distance(c3, col);
    const float d4 = distance(c4, col);
    const float d5 = distance(c5, col);
    const float d6 = distance(c6, col);
    const float d7 = distance(c7, col);
    const float d8 = distance(c8, col);

    // Best fit search
    float4 rgb_d = float4(c1, d1);
    rgb_d = rgb_d.a < d2 ? rgb_d : float4(c2, d2);
    rgb_d = rgb_d.a < d3 ? rgb_d : float4(c3, d3);
    rgb_d = rgb_d.a < d4 ? rgb_d : float4(c4, d4);
    rgb_d = rgb_d.a < d5 ? rgb_d : float4(c5, d5);
    rgb_d = rgb_d.a < d6 ? rgb_d : float4(c6, d6);
    rgb_d = rgb_d.a < d7 ? rgb_d : float4(c7, d7);
    rgb_d = rgb_d.a < d8 ? rgb_d : float4(c8, d8);

    // sRGB -> Linear
    Output = float4(SRGBToLinear(rgb_d.rgb), Source.a);
}
