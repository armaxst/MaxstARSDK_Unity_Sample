Shader "Custom/YUVToRGB"
{
    Properties 
    {
        _YTex ("Y channel texture", 2D) = "white" {}
        _UVTex ("UV channel texture", 2D) = "white" {}
    }
    SubShader 
    {
        Pass 
        {
            Cull Off
            ZWrite Off
            Lighting Off

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _YTex;
            sampler2D _UVTex;
            fixed4 _PauseColor;
            float pauseMode;
            
            fixed4 frag (v2f i) : SV_Target
            {
                if (pauseMode == 1.0) {
                    return _PauseColor;
                }
                float y = tex2D(_YTex, i.uv).r;
                float u = tex2D(_UVTex, i.uv).r;
                float v = tex2D(_UVTex, i.uv).g;

                y = 1.1643 * (y - 0.0625);
                u = u - 0.5;
                v = v - 0.5;    
                float r = y + 1.5958 * v;
                float g = y - 0.39173 * u - 0.81290 * v;
                float b = y + 2.017 * u;

                return fixed4(r, g, b, 1.0);
            }
            ENDCG
        }
    }
}