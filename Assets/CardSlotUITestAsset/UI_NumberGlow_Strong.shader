Shader "UI/NumberGlow_Strong"
{
    Properties
    {
        [PerRendererData]_MainTex("Sprite", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _GlowColor("Glow Color (HDR)", Color) = (1,1,1,1)
        _GlowWidth("Glow Width (px)", Range(0,32)) = 12
        _GlowSoftness("Glow Softness", Range(0,5)) = 1.8
        _GlowIntensity("Glow Intensity", Range(0,8)) = 3.0
        _AlphaThreshold("Alpha Threshold", Range(0,1)) = 0.0

        [HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
        Stencil{ Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        ColorMask [_ColorMask]
        Cull Off ZWrite Off ZTest [unity_GUIZTestMode]

        // ▶ Pass 1: Additive halo (강제 후광)
        Pass
        {
            Name "HaloAdd"
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex; float4 _MainTex_TexelSize;
            fixed4 _Color; fixed4 _GlowColor;
            float _GlowWidth,_GlowSoftness,_GlowIntensity,_AlphaThreshold;

            struct V{float4 v:POSITION;float2 uv:TEXCOORD0;fixed4 c:COLOR;};
            struct O{float4 p:SV_POSITION;float2 uv:TEXCOORD0;fixed4 c:COLOR;float4 wp:TEXCOORD1;};

            O vert(V i){O o;o.p=UnityObjectToClipPos(i.v);o.uv=i.uv;o.c=i.c*_Color;o.wp=i.v;return o;}

            fixed4 frag(O i):SV_Target
            {
                float centerA = (tex2D(_MainTex,i.uv)*i.c).a;
                #ifdef UNITY_UI_CLIP_RECT
                if(UnityGet2DClipping(i.wp.xy,_ClipRect)<0) discard;
                #endif

                float2 px=_MainTex_TexelSize.xy; float r=_GlowWidth;
                // 더 두텁게 보이도록 방향 16개 + 3링 샘플
                float2 dirs[16] = {
                    float2(1,0),float2(-1,0),float2(0,1),float2(0,-1),
                    float2(1,1),float2(1,-1),float2(-1,1),float2(-1,-1),
                    float2(2,1),float2(2,-1),float2(-2,1),float2(-2,-1),
                    float2(3,1),float2(3,-1),float2(-3,1),float2(-3,-1)
                };

                float acc=0;
                [unroll] for(int k=0;k<16;k++){
                    float2 nd=normalize(dirs[k]);
                    float a1=tex2D(_MainTex,i.uv+nd*px*(r*0.50)).a;
                    float a2=tex2D(_MainTex,i.uv+nd*px*(r*0.85)).a;
                    float a3=tex2D(_MainTex,i.uv+nd*px*(r*1.20)).a;
                    float w1=1.0/(1.0+_GlowSoftness*0.5);
                    float w2=1.0/(1.0+_GlowSoftness*1.0);
                    float w3=1.0/(1.0+_GlowSoftness*1.5);
                    acc += (saturate(a1-centerA)*w1 + saturate(a2-centerA)*w2 + saturate(a3-centerA)*w3);
                }

                float edge = saturate(acc/16.0) * _GlowIntensity;
                if(centerA<=_AlphaThreshold && edge<=0.0001) discard;

                return fixed4(_GlowColor.rgb*edge, edge);
            }
            ENDCG
        }

        // ▶ Pass 2: 원본 스프라이트 (그대로)
        Pass
        {
            Name "Base"
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            sampler2D _MainTex; fixed4 _Color;
            struct V{float4 v:POSITION;float2 uv:TEXCOORD0;fixed4 c:COLOR;};
            struct O{float4 p:SV_POSITION;float2 uv:TEXCOORD0;fixed4 c:COLOR;float4 wp:TEXCOORD1;};
            O vert(V i){O o;o.p=UnityObjectToClipPos(i.v);o.uv=i.uv;o.c=i.c*_Color;o.wp=i.v;return o;}
            fixed4 frag(O i):SV_Target{
                fixed4 col=tex2D(_MainTex,i.uv)*i.c;
                #ifdef UNITY_UI_CLIP_RECT
                if(UnityGet2DClipping(i.wp.xy,_ClipRect)<0) discard;
                #endif
                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}