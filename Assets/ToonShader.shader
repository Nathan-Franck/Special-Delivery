Shader "Hidden/ToonShader"
{

   // Using these settings in the script: 
    // public struct Settings
    // {
    //     public float LineThickness;
    //     public float DepthThreshold;
    //     public int PaletteQuantization;
    // }
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _LineThickness;
            float _DepthThreshold;
            int _PaletteQuantization;
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float3 RGBtoHSV(float3 rgb) { float cmax = max(rgb.r, max(rgb.g, rgb.b)); float cmin = min(rgb.r, min(rgb.g, rgb.b)); float delta = cmax - cmin; float hue = 0; if (delta > 0) { if (cmax == rgb.r) hue = fmod((rgb.g - rgb.b) / delta, 6); else if (cmax == rgb.g) hue = (rgb.b - rgb.r) / delta + 2; else if (cmax == rgb.b) hue = (rgb.r - rgb.g) / delta + 4; hue *= 60; if (hue < 0) hue += 360; } float saturation = cmax == 0 ? 0 : delta / cmax; float value = cmax; return float3(hue, saturation, value); }

            float3 frag (v2f i) : SV_Target
            {
                const int length = 4;

                float2 uv = i.uv;
                float2 depthOffsets[length] = {
                    float2(0.5, 0),
                    float2(-0.5, 0),
                    float2(0, 0.5),
                    float2(0, -0.5)
                };


                float depthSamples[length] = {0, 0, 0, 0};
                for (int i = 0; i < length; i++)
                {
                    depthSamples[i] = RGBtoHSV(tex2D(_MainTex, uv + depthOffsets[i] / _ScreenParams.xy * _LineThickness).rgb).y;
                }

                const int slopeLength = 2;
                int slopeIndexSamples[slopeLength][2] = {
                    {0, 1},
                    {2, 3},
                };
                float slopes[slopeLength] = {0, 0};

                for (int j = 0; j < slopeLength; j++)
                {
                    slopes[j] = depthSamples[slopeIndexSamples[j][0]] - depthSamples[slopeIndexSamples[j][1]];
                }
                float lineStrength = max(max(abs(slopes[0]) - _DepthThreshold, abs(slopes[1]) - _DepthThreshold), 0);

                float4 unquantizedColor = tex2D(_MainTex, uv);
                float3 quantizedColor = round(unquantizedColor.rgb * _PaletteQuantization) / _PaletteQuantization;
                return quantizedColor - lineStrength;
            }
            ENDCG
        }
    }
}
