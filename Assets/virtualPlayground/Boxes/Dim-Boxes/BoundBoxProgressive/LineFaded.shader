Shader "Dimboxes/Lines/Faded"
{
    Properties
    {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_spread("spread", Range(0, 1)) = 0.5
		_offset("offset", Range(0, 1)) = 0.5
		[Toggle] _inverse("inverse", Float) = 0
		[KeywordEnum(None, Central, Diagonal_centre, Diagonal_corner)] _Animated("Animation mode", Float) = 0
		_Centre("Centre", Vector) = (0, 0, 0, 0)
		_BoxDirX ("BoxDirX", Vector) = (1, 0, 0, 0)
		_BoxDirY ("BoxDirY", Vector) = (0, 1, 0, 0)
		_BoxDirZ ("BoxDirZ", Vector) = (0, 0, 1, 0)
		_BoxExtent ("BoxExtent", Vector) = (1, 1, 1, 0)
		_DiagPlane ("DiagPlane", Vector) = (1, 1, 1, 0)

    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
		Cull Off
		Offset 1, 1

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile __ _ANIMATED_CENTRAL _ANIMATED_DIAGONAL_CENTRE _ANIMATED_DIAGONAL_CORNER
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#include "faded.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
#if _ANIMATED_CENTRAL || _ANIMATED_DIAGONAL_CORNER || _ANIMATED_DIAGONAL_CENTRE
				float3 worldPos : TEXCOORD2;
#endif
            };

            sampler2D _MainTex;
			fixed4 _Color;
            float4 _MainTex_ST;
#if _ANIMATED_CENTRAL || _ANIMATED_DIAGONAL_CORNER || _ANIMATED_DIAGONAL_CENTRE
			fixed _inverse;
#endif
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
				o.color = v.color;
#if _ANIMATED_CENTRAL || _ANIMATED_DIAGONAL_CORNER || _ANIMATED_DIAGONAL_CENTRE
				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
#endif
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color * i.color;
#if _ANIMATED_CENTRAL || _ANIMATED_DIAGONAL_CORNER || _ANIMATED_DIAGONAL_CENTRE
			fixed4 fade = ANIM_FADE(i.worldPos);
			col.a = (col.a)*(_inverse + (1 - 2 * _inverse)*fade.a);
#endif
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
