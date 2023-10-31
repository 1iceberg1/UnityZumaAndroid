using System;

namespace Spine
{
	public static class MathUtils
	{
		public const float PI = (float)Math.PI;

		public const float PI2 = (float)Math.PI * 2f;

		public const float RadDeg = 180f / (float)Math.PI;

		public const float DegRad = (float)Math.PI / 180f;

		private const int SIN_BITS = 14;

		private const int SIN_MASK = 16383;

		private const int SIN_COUNT = 16384;

		private const float RadFull = (float)Math.PI * 2f;

		private const float DegFull = 360f;

		private const float RadToIndex = 8192f / (float)Math.PI;

		private const float DegToIndex = 45.5111122f;

		private static float[] sin;

		static MathUtils()
		{
			sin = new float[16384];
			for (int i = 0; i < 16384; i++)
			{
				sin[i] = (float)Math.Sin(((float)i + 0.5f) / 16384f * ((float)Math.PI * 2f));
			}
			for (int j = 0; j < 360; j += 90)
			{
				sin[(int)((float)j * 45.5111122f) & 0x3FFF] = (float)Math.Sin((float)j * ((float)Math.PI / 180f));
			}
		}

		public static float Sin(float radians)
		{
			return sin[(int)(radians * (8192f / (float)Math.PI)) & 0x3FFF];
		}

		public static float Cos(float radians)
		{
			return sin[(int)((radians + (float)Math.PI / 2f) * (8192f / (float)Math.PI)) & 0x3FFF];
		}

		public static float SinDeg(float degrees)
		{
			return sin[(int)(degrees * 45.5111122f) & 0x3FFF];
		}

		public static float CosDeg(float degrees)
		{
			return sin[(int)((degrees + 90f) * 45.5111122f) & 0x3FFF];
		}

		public static float Atan2(float y, float x)
		{
			if (x == 0f)
			{
				if (y > 0f)
				{
					return (float)Math.PI / 2f;
				}
				if (y == 0f)
				{
					return 0f;
				}
				return -(float)Math.PI / 2f;
			}
			float num = y / x;
			float num2;
			if (Math.Abs(num) < 1f)
			{
				num2 = num / (1f + 0.28f * num * num);
				if (x < 0f)
				{
					return num2 + ((!(y < 0f)) ? ((float)Math.PI) : (-(float)Math.PI));
				}
				return num2;
			}
			num2 = (float)Math.PI / 2f - num / (num * num + 0.28f);
			return (!(y < 0f)) ? num2 : (num2 - (float)Math.PI);
		}

		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}
	}
}
