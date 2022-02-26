// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PageShader" {

    Properties {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _ScaleX("Scale X", Float) = 1
        _ScaleY("Scale Y", Float) = 1
        _RotationY("Rotation Y", Float) = 0
            
    }

    SubShader {
        Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float3 rotate(float3 vec, float radians) {
                float sina, cosa;
                sincos(radians, sina, cosa);
                float2x2 rotationMatrix = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(rotationMatrix, vec.xy), vec.z).xyz;
            };

            float3 stretch(float3 vec, float x, float y) {
                float2x2 stretchMatrix = float2x2(x, 0, 0, y);
                return float3(mul(stretchMatrix, vec.xy), vec.z).xyz;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _ScaleX;
            float _ScaleY;
            float _RotationY;

            v2f vert (appdata v) {
                
                // stretch the matrix
                float3 vertex = stretch(v.vertex, _ScaleX, _ScaleY);

                // transform to clip space
                float4 clipVertex = UnityObjectToClipPos(vertex);

                // pass the data to the fragment shader
                v2f o;
                o.vertex = clipVertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); 


                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
