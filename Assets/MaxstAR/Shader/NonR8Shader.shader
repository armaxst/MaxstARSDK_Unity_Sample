Shader "Custom/NonR8YUVToRGB"
{
Properties 
{
    _YTex ("Y channel texture", 2D) = "white" {}
    _UVTex ("U channel texture", 2D) = "white" {}
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
        float _range;
        fixed4 _PauseColor;
        float pauseMode;

        float uvScaleWidth;
        float uvScaleHeight;
        float textureWidthSize;
        float textureHeightSize;

        float custom_mod(float x, float y)
        {
            return x - (y * floor(x / y));
        }

        fixed4 frag (v2f i) : SV_Target
        {
            if (pauseMode == 1.0) {
                return _PauseColor;
            }

            float y = tex2D(_YTex, i.uv).a;
            float2 uCoord = i.uv;
            float2 vCoord = i.uv;

            //int localPosition = int(textureWidthSize*(i.uv.x + uvScale));
            int localPositionX = floor(textureWidthSize*i.uv.x);
            int localPositionY = int(textureHeightSize*(i.uv.y));

            float XPosition = textureWidthSize*i.uv.x - 0.5;

            if(localPositionX % 2 == 0) {
                uCoord.x = uCoord.x + uvScaleWidth;
            } else {
                vCoord.x = vCoord.x - uvScaleWidth;
            }

            float v = tex2D(_UVTex, vCoord).a;
            float u = tex2D(_UVTex, uCoord).a;
       
            y = 1.1643 * (y - 0.0625);
            u = u - 0.5;
            v = v - 0.5;    
            float r = y + 1.5958 * v;
            float g = y - 0.39173 * u - 0.81290 * v;
            float b = y + 2.017 * u;

            //float3 yuv = float3(y, u-0.5, v-0.5);
            //float3x3 rgbConvertor = float3x3(1.0,1.0,1.0,   0.0,-.34413,1.772,   1.402,-.71414,0.0);
            //float3x3 rgbConvertor = float3x3(1.0,1.0,1.0,   0.0,-.18732,1.8556,   1.57481,-.46813,0.0);
            //float3 rgb = mul(yuv, rgbConvertor);

            return fixed4(r, g, b, 1.0);

        }
        ENDCG
    }
}
}