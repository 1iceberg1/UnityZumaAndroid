namespace Spine
{
	public class IkConstraintTimeline : CurveTimeline
	{
		public const int ENTRIES = 3;

		private const int PREV_TIME = -3;

		private const int PREV_MIX = -2;

		private const int PREV_BEND_DIRECTION = -1;

		private const int MIX = 1;

		private const int BEND_DIRECTION = 2;

		internal int ikConstraintIndex;

		internal float[] frames;

		public int IkConstraintIndex
		{
			get
			{
				return ikConstraintIndex;
			}
			set
			{
				ikConstraintIndex = value;
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

		public override int PropertyId => 150994944 + ikConstraintIndex;

		public IkConstraintTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 3];
		}

		public void SetFrame(int frameIndex, float time, float mix, int bendDirection)
		{
			frameIndex *= 3;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = mix;
			frames[frameIndex + 2] = bendDirection;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, bool setupPose, bool mixingOut)
		{
			IkConstraint ikConstraint = skeleton.ikConstraints.Items[ikConstraintIndex];
			float[] array = frames;
			if (time < array[0])
			{
				if (setupPose)
				{
					ikConstraint.mix = ikConstraint.data.mix;
					ikConstraint.bendDirection = ikConstraint.data.bendDirection;
				}
				return;
			}
			if (time >= array[array.Length - 3])
			{
				if (setupPose)
				{
					ikConstraint.mix = ikConstraint.data.mix + (array[array.Length + -2] - ikConstraint.data.mix) * alpha;
					ikConstraint.bendDirection = ((!mixingOut) ? ((int)array[array.Length + -1]) : ikConstraint.data.bendDirection);
					return;
				}
				ikConstraint.mix += (array[array.Length + -2] - ikConstraint.mix) * alpha;
				if (!mixingOut)
				{
					ikConstraint.bendDirection = (int)array[array.Length + -1];
				}
				return;
			}
			int num = Animation.BinarySearch(array, time, 3);
			float num2 = array[num + -2];
			float num3 = array[num];
			float curvePercent = GetCurvePercent(num / 3 - 1, 1f - (time - num3) / (array[num + -3] - num3));
			if (setupPose)
			{
				ikConstraint.mix = ikConstraint.data.mix + (num2 + (array[num + 1] - num2) * curvePercent - ikConstraint.data.mix) * alpha;
				ikConstraint.bendDirection = ((!mixingOut) ? ((int)array[num + -1]) : ikConstraint.data.bendDirection);
				return;
			}
			ikConstraint.mix += (num2 + (array[num + 1] - num2) * curvePercent - ikConstraint.mix) * alpha;
			if (!mixingOut)
			{
				ikConstraint.bendDirection = (int)array[num + -1];
			}
		}
	}
}
