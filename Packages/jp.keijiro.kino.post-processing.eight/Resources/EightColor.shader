//
// Vertex/Fragment shader pair for the Eight Color effect
//

Shader "Hidden/Kino/PostProcess/Eight/EightColor"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vertex(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    static const float bayer2x2[] = {-0.5, 0.16666666, 0.5, -0.16666666};

    float4 _Palette[8];
    float _Dithering;
    uint _Downsampling;
    float _Opacity;

    TEXTURE2D_X(_InputTexture);

    float4 Fragment(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        // Input sample
        const uint2 pss = (uint2)(input.texcoord * _ScreenSize.xy) / _Downsampling;
        float4 col = LOAD_TEXTURE2D_X(_InputTexture, pss * _Downsampling);

        // Linear -> sRGB
        col.rgb = LinearToSRGB(col.rgb);

        // Dithering (2x2 bayer)
        const float dither = bayer2x2[(pss.y & 1) * 2 + (pss.x & 1)];
        col.rgb += dither * _Dithering;

        // Alias for each color
        const float3 c1 = _Palette[0].rgb;
        const float3 c2 = _Palette[1].rgb;
        const float3 c3 = _Palette[2].rgb;
        const float3 c4 = _Palette[3].rgb;
        const float3 c5 = _Palette[4].rgb;
        const float3 c6 = _Palette[5].rgb;
        const float3 c7 = _Palette[6].rgb;
        const float3 c8 = _Palette[7].rgb;

        // Euclidean distance
        const float d1 = distance(c1, col.rgb);
        const float d2 = distance(c2, col.rgb);
        const float d3 = distance(c3, col.rgb);
        const float d4 = distance(c4, col.rgb);
        const float d5 = distance(c5, col.rgb);
        const float d6 = distance(c6, col.rgb);
        const float d7 = distance(c7, col.rgb);
        const float d8 = distance(c8, col.rgb);

        // Best fit search
        float4 rgb_d = float4(c1, d1);
        rgb_d = rgb_d.a < d2 ? rgb_d : float4(c2, d2);
        rgb_d = rgb_d.a < d3 ? rgb_d : float4(c3, d3);
        rgb_d = rgb_d.a < d4 ? rgb_d : float4(c4, d4);
        rgb_d = rgb_d.a < d5 ? rgb_d : float4(c5, d5);
        rgb_d = rgb_d.a < d6 ? rgb_d : float4(c6, d6);
        rgb_d = rgb_d.a < d7 ? rgb_d : float4(c7, d7);
        rgb_d = rgb_d.a < d8 ? rgb_d : float4(c8, d8);

        // Opacity
        col.rgb = lerp(col.rgb, rgb_d.rgb, _Opacity);

        // sRGB -> Linear
        col.rgb = SRGBToLinear(col.rgb);

        return col;
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDHLSL
        }
    }
    Fallback Off
}
