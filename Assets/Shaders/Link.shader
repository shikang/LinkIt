Shader "Trail/Link"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_MainTexture("Sprite", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			fixed4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTexture;
			sampler2D _AlphaTex;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTexture, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D(_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				//standard sprite shader, only this part is different.
				//takes the sprite in
				fixed4 s = SampleSpriteTexture(i.uv);

				//makes a colored version of the sprite
				fixed4 c = s * i.color;

				//makes the grayscale version
				fixed n = (s.g + s.r + s.b) / 3;

				//So, I've scrapped the previous calculation in favor of this I'll leave the previous one in too, just for reference
				c = (c * 3 + n*c.a)*c.a;
				//Adds the grayscale version on top of the colored version
				//The alpha multiplications give the neon effect feeling
				//c.g = (c.g + n * c.a) * c.a;
				//c.r = (c.r + n * c.a) * c.a;
				//c.b = (c.b + n * c.a) * c.a;
				// You can add c.a multiplications of colors 
				//(i.e. turn  c.g to c.g*c.a) for less color in your effect
				//this saturates the insides a bit too much for my liking

				return c;
			}
			ENDCG
		}
	}
}
