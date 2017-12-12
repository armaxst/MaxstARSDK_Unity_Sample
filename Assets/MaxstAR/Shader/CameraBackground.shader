Shader "Custom/CameraBackground" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "black" {}
    }
    SubShader {
        Tags {"Queue"="background" "RenderType"="opaque" }
        Pass {
			Cull Off
			ZWrite Off
			Lighting Off

            // Render the teapot
            SetTexture [_MainTex] {
                combine texture 
            }
        }
    } 
    FallBack "Unlit/Texture"
}
