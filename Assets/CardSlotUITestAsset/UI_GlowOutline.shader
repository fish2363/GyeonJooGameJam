Shader "UI/GlowOutline"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite", 2D) = "white" {}
        _Color   ("Tint", Color) = (1,1,1,1)

        // Glow controls
        _GlowColor    ("Glow Color", Color) = (0.7,0.9,1,1)
        _GlowWidth    ("Glow Width (px)", Range(0,12)) = 4
        _GlowSoftness ("Glow Softness", Range(0,5)) = 1.25
        _GlowIntensity("Glow Intensity", Range(0,5)) = 1.3

        // Alpha clip(경계 노이즈 제거용)
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.001

        // ===== UI/Default와 동일한 스텐실/컬러마스크 =====
        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest [unity_GUIZTestMode]

        Pass
        {
            Name "UIGlowOutline"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; // (1/width, 1/height, width, height)
            fixed4 _Color;

            fixed4 _GlowColor;
            float  _GlowWidth;      // 픽셀 기준
            float  _GlowSoftness;   // 퍼짐 부드러움
            float  _GlowIntensity;  // 밝기
            float  _AlphaThreshold; // 경계 노이즈 컷

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                fixed4 color : COLOR;
                float4 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.uv    = v.uv;
                o.color = v.color * _Color;
                o.worldPos = v.vertex;
                return o;
            }

            // 경계 검출 + 부드러운 테두리
            fixed4 frag (v2f i) : SV_Target
            {
                // 기본 스프라이트
                fixed4 baseCol = tex2D(_MainTex, i.uv) * i.color;
                float  centerA = baseCol.a;

                // 8방향 샘플(반경 r 픽셀)
                float2 px = _MainTex_TexelSize.xy;
                float  r  = _GlowWidth;

                float aL = tex2D(_MainTex, i.uv + float2(-r*px.x, 0)).a;
                float aR = tex2D(_MainTex, i.uv + float2( r*px.x, 0)).a;
                float aU = tex2D(_MainTex, i.uv + float2(0,  r*px.y)).a;
                float aD = tex2D(_MainTex, i.uv + float2(0, -r*px.y)).a;

                float aUL = tex2D(_MainTex, i.uv + float2(-r*px.x,  r*px.y)).a;
                float aUR = tex2D(_MainTex, i.uv + float2( r*px.x,  r*px.y)).a;
                float aDL = tex2D(_MainTex, i.uv + float2(-r*px.x, -r*px.y)).a;
                float aDR = tex2D(_MainTex, i.uv + float2( r*px.x, -r*px.y)).a;

                // 주변 최대 알파
                float maxA = max(max(max(aL,aR), max(aU,aD)), max(max(aUL,aUR), max(aDL,aDR)));

                // 소프트니스 가중
                float softness = max(_GlowSoftness, 0.001);
                float edge = saturate( (maxA - centerA) * (1.0 + _GlowIntensity) / (1.0 + softness) );

                #ifdef UNITY_UI_CLIP_RECT
                if (UnityGet2DClipping(i.worldPos.xy, _ClipRect) < 0)
                    discard;
                #endif

                if (centerA < _AlphaThreshold && edge <= 0.0001)
                    discard;

                fixed3 glowRGB = _GlowColor.rgb * (edge * _GlowIntensity);
                fixed4 col = baseCol;
                col.rgb += glowRGB;

                return col;
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}
