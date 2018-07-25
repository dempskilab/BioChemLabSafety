Shader "Custom/SpriteTest" {
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _offsetValue ("Offset Value", Range (0,1.0)) = 0.05
        _YMin ("Height min clip Value", Range (0,1.0)) = 0
        _YMax ("Height max clip Value", Range (0,1.0)) = 1.0
        _XMin ("Width min clip Value", Range (0,1.0)) = 0
        _XMax ("Width max clip Value", Range (0,1.0)) = 1.0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
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
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };

            float4 pos;
            fixed4 _Color;
            float1 _offsetValue, _YMax, _YMin, _XMax, _XMin;
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                if((c.r <= _offsetValue && c.g <= _offsetValue && c.b <= _offsetValue) || (IN.texcoord.y > _YMax || IN.texcoord.y < _YMin) || (IN.texcoord.x > _XMax || IN.texcoord.x < _XMin))
                 {
                     c.a = 0;
                 }
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
