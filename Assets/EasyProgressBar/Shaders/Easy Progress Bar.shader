Shader "Unlit/Easy Progress Bar"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _StartColor ("Start Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _EndColor ("End Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _BackColor ("Back Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _Gradient ("Gradient", Range(0, 1)) = 0
        [HideInInspector] _Roundness ("Roundness", Range(0, 1)) = 0.5
        [HideInInspector] _BorderSize ("Border Size", Range(0, 1)) = 0.2
        [HideInInspector] _FillAmount ("Fill Amount", Range(0, 1)) = 0
        [HideInInspector] _Size ("Size", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainColor;
            float4 _StartColor;
            float4 _EndColor;
            float4 _BackColor;
            fixed _Gradient;
            fixed _Roundness;
            fixed _BorderSize;
            fixed _FillAmount;
            float4 _Size;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed CalculateSDF(float2 position, float roundness, float2 halfSize)
            {
                float2 distanceToEdge = abs(position) - (halfSize - roundness);
                float outsideDistance = length(max(distanceToEdge, 0));
                float insideDistance = min(max(distanceToEdge.x, distanceToEdge.y), 0);

                return outsideDistance + insideDistance - roundness;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed fillAmount;
                float mainAxisHalfSize;

                if (_Size.x < _Size.y)
                {
                    fillAmount = i.uv.y;
                    mainAxisHalfSize = _Size.x * 0.5;
                }
                else
                {
                    fillAmount = i.uv.x;
                    mainAxisHalfSize = _Size.y * 0.5;
                }

                fixed sdf = CalculateSDF((i.uv - 0.5) * _Size, _Roundness * mainAxisHalfSize, _Size * 0.5);
                clip(-sdf);

                fixed borderSdf = sdf + _BorderSize * mainAxisHalfSize;
                fixed borderMask = step(0, -borderSdf);
                fixed fillMask = _FillAmount > fillAmount;
                fixed totalMask = borderMask * fillMask;

                if (totalMask < 1)
                {
                    return _BackColor * _MainColor;
                }

                fixed4 color = tex2D(_MainTex, float2(i.uv.x, i.uv.y)) * _MainColor;
                fixed4 fill = lerp(_StartColor, _EndColor, fillAmount * _Gradient + _FillAmount * (1 - _Gradient));

                return color * fill;
            }
            ENDCG
        }
    }
}