namespace Spine
{
	public class RotateTimeline : CurveTimeline
	{
		public const int ENTRIES = 2;

		internal const int PREV_TIME = -2;

		internal const int PREV_ROTATION = -1;

		internal const int ROTATION = 1;

		internal int boneIndex;

		internal float[] frames;

		public int BoneIndex
		{
			get
			{
				return boneIndex;
			}
			set
			{
				boneIndex = value;
			}
		}

		public float[] Frames
		{
			get
			{
				return frames;
			}
			set
			{
				frames = value;
			}
		}

		public override int PropertyId => boneIndex;

		public RotateTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount << 1];
		}

		public void SetFrame(int frameIndex, float time, float degrees)
		{
			frameIndex <<= 1;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = degrees;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, bool setupPose, bool mixingOut)
		{
			Bone bone = skeleton.bones.Items[boneIndex];
			float[] array = frames;
			if (time < array[0])
			{
				if (setupPose)
				{
					bone.rotation = bone.data.rotation;
				}
				return;
			}
			if (time >= array[array.Length - 2])
			{
				if (setupPose)
				{
					bone.rotation = bone.data.rotation + array[array.Length + -1] * alpha;
					return;
				}
				float num = bone.data.rotation + array[array.Length + -1] - bone.rotation;
				num -= (float)((16384 - (int)(16384.499999999996 - (double)(num / 360f))) * 360);
				bone.rotation += num * alpha;
				return;
			}
			int num2 = Animation.BinarySearch(array, time, 2);
			float num3 = array[num2 + -1];
			float num4 = array[num2];
			float curvePercent = GetCurvePercent((num2 >> 1) - 1, 1f - (time - num4) / (array[num2 + -2] - num4));
			float num5 = array[num2 + 1] - num3;
			num5 -= (float)((16384 - (int)(16384.499999999996 - (double)(num5 / 360f))) * 360);
			num5 = num3 + num5 * curvePercent;
			if (setupPose)
			{
				num5 -= (float)((16384 - (int)(16384.499999999996 - (double)(num5 / 360f))) * 360);
				bone.rotation = bone.data.rotation + num5 * alpha;
			}
			else
			{
				num5 = bone.data.rotation + num5 - bone.rotation;
				num5 -= (float)((16384 - (int)(16384.499999999996 - (double)(num5 / 360f))) * 360);
				bone.rotation += num5 * alpha;
			}
		}
	}
}
