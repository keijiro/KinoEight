Shader "Hidden/Kino/PostProcess/FourColor"
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

    float3 _Color1, _Color2, _Color3, _Color4;
    float _Dithering;
    float _Opacity;

    TEXTURE2D_X(_InputTexture);

    float4 Fragment(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _ScreenSize.xy;
        float4 c = LOAD_TEXTURE2D_X(_InputTexture, positionSS);

        c.rgb = LinearToSRGB(c.rgb);

        float dither = bayer2x2[(positionSS.y & 1) * 2 + (positionSS.x & 1)];
        c.rgb += dither * _Dithering;

        float4 rgb_d, temp;

        // Color 1
        rgb_d = float4(_Color1, distance(_Color1, c.rgb));

        // Color 2
        temp = float4(_Color2, distance(_Color2, c.rgb));
        rgb_d = lerp(rgb_d, temp, rgb_d.a > temp.a);

        // Color 3
        temp = float4(_Color3, distance(_Color3, c.rgb));
        rgb_d = lerp(rgb_d, temp, rgb_d.a > temp.a);

        // Color 4
        temp = float4(_Color4, distance(_Color4, c.rgb));
        rgb_d = lerp(rgb_d, temp, rgb_d.a > temp.a);

        // Opacity
        c.rgb = lerp(c.rgb, rgb_d.rgb, _Opacity);

        return float4(SRGBToLinear(c.rgb), c.a);
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
