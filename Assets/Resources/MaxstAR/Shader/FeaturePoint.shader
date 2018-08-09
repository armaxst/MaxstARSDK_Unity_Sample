// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MaxstAR/FeaturePoint"
{
    Properties {
        _BlueTex ("Blue (RGBA)", 2D) = "black" {}
        _RedTex ("Red (RGBA)", 2D) = "black" {}
        [Toggle(USE_TEXTURE)] _Type("Use Texture", Float) = 0
    }
    SubShader 
    {
        Tags { "Queue" = "Transparent" }
        Pass 
        {
        	ZWrite Off
            Cull Off
            Lighting Off

            Blend SrcAlpha OneMinusSrcAlpha

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

            float _FeatureSize;
            float4x4 projectionMatrix;
            //float4x4 poseMatrix;
            
            v2f vert (appdata v, uint vid : SV_VertexID)
            {
                v2f o;
                uint divide = vid % 4;

                float4 tempVertex = v.vertex;
//                float4 convertedVertex = mul(poseMatrix, tempVertex);
                float4 projectedVertex = v.vertex;

//                if(convertedVertex.z > 0.0) {
//                    projectedVertex = float4(convertedVertex.x / convertedVertex.z, convertedVertex.y / convertedVertex.z, 1.0, 1.0);
//                } else {
//                    projectedVertex = float4(convertedVertex.x / convertedVertex.z, convertedVertex.y / convertedVertex.z, 0.0, 1.0);
//                }

                if(divide == 0) {
                    projectedVertex = projectedVertex + float4(-_FeatureSize,-_FeatureSize,0,0);
                } else if(divide == 1) {
                    projectedVertex = projectedVertex + float4(_FeatureSize,-_FeatureSize,0,0);
                } else if(divide == 2) {
                    projectedVertex = projectedVertex + float4(-_FeatureSize,_FeatureSize,0,0);
                } else if(divide == 3) {
                    projectedVertex = projectedVertex + float4(_FeatureSize,_FeatureSize,0,0);
                }

                //projectedVertex = float4(projectedVertex.x*4500.0, projectedVertex.y*4500.0, projectedVertex.z*4500.0, projectedVertex.w);

                o.vertex = mul(UNITY_MATRIX_MV, projectedVertex);
                o.vertex = mul(projectionMatrix, o.vertex);
                o.uv = v.uv; 
                return o;
            }

            sampler2D _BlueTex;
            sampler2D _RedTex;
            float _Type;

            fixed4 frag (v2f i) : SV_Target
            {
                if(_Type == 1.0) {
                    return tex2D(_BlueTex, i.uv);
                } else {
                    return tex2D(_RedTex, i.uv);
                }

            }
            ENDCG
        }
    }
}