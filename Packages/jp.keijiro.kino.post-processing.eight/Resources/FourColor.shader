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

    static const uint bayer2x2[] = {0, 170, 255, 85};

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

        uint dither = bayer2x2[(positionSS.y & 1) * 2 + (positionSS.x & 1)];
        c += (dither / 255.0 - 0.5) * _Dithering;

        float4 rgb_d = float4(_Color1, distance(c.rgb, _Color1));

        float4 rgb_d2 = float4(_Color2, distance(c.rgb, _Color2));
        rgb_d = lerp(rgb_d, rgb_d2, rgb_d.a > rgb_d2.a);

        float4 rgb_d3 = float4(_Color3, distance(c.rgb, _Color3));
        rgb_d = lerp(rgb_d, rgb_d3, rgb_d.a > rgb_d3.a);

        float4 rgb_d4 = float4(_Color4, distance(c.rgb, _Color4));
        rgb_d = lerp(rgb_d, rgb_d4, rgb_d.a > rgb_d4.a);

        return float4(SRGBToLinear(rgb_d.rgb), c.a);
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
