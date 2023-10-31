namespace Spine
{
	public class TransformConstraintTimeline : CurveTimeline
	{
		public const int ENTRIES = 5;

		private const int PREV_TIME = -5;

		private const int PREV_ROTATE = -4;

		private const int PREV_TRANSLATE = -3;

		private const int PREV_SCALE = -2;

		private const int PREV_SHEAR = -1;

		private const int ROTATE = 1;

		private const int TRANSLATE = 2;

		private const int SCALE = 3;

		private const int SHEAR = 4;

		internal int transformConstraintIndex;

		internal float[] frames;

		public int TransformConstraintIndex
		{
			get
			{
				return transformConstraintIndex;
			}
			set
			{
				transformConstraintIndex = value;
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

		public override int PropertyId => 167772160 + transformConstraintIndex;

		public TransformConstraintTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount * 5];
		}

		public void SetFrame(int frameIndex, float time, float rotateMix, float translateMix, float scaleMix, float shearMix)
		{
			frameIndex *= 5;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = rotateMix;
			frames[frameIndex + 2] = translateMix;
			frames[frameIndex + 3] = scaleMix;
			frames[frameIndex + 4] = shearMix;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, bool setupPose, bool mixingOut)
		{
			TransformConstraint transformConstraint = skeleton.transformConstraints.Items[transformConstraintIndex];
			float[] array = frames;
			if (time < array[0])
			{
				if (setupPose)
				{
					TransformConstraintData data = transformConstraint.data;
					transformConstraint.rotateMix = data.rotateMix;
					transformConstraint.translateMix = data.translateMix;
					transformConstraint.scaleMix = data.scaleMix;
					transformConstraint.shearMix = data.shearMix;
				}
				return;
			}
			float num2;
			float num3;
			float num4;
			float num5;
			if (time >= array[array.Length - 5])
			{
				int num = array.Length;
				num2 = array[num + -4];
				num3 = array[num + -3];
				num4 = array[num + -2];
				num5 = array[num + -1];
			}
			else
			{
				int num6 = Animation.BinarySearch(array, time, 5);
				num2 = array[num6 + -4];
				num3 = array[num6 + -3];
				num4 = array[num6 + -2];
				num5 = array[num6 + -1];
				float num7 = array[num6];
				float curvePercent = GetCurvePercent(num6 / 5 - 1, 1f - (time - num7) / (array[num6 + -5] - num7));
				num2 += (array[num6 + 1] - num2) * curvePercent;
				num3 += (array[num6 + 2] - num3) * curvePercent;
				num4 += (array[num6 + 3] - num4) * curvePercent;
				num5 += (array[num6 + 4] - num5) * curvePercent;
			}
			if (setupPose)
			{
				TransformConstraintData data2 = transformConstraint.data;
				transformConstraint.rotateMix = data2.rotateMix + (num2 - data2.rotateMix) * alpha;
				transformConstraint.translateMix = data2.translateMix + (num3 - data2.translateMix) * alpha;
				transformConstraint.scaleMix = data2.scaleMix + (num4 - data2.scaleMix) * alpha;
				transformConstraint.shearMix = data2.shearMix + (num5 - data2.shearMix) * alpha;
			}
			else
			{
				transformConstraint.rotateMix += (num2 - transformConstraint.rotateMix) * alpha;
				transformConstraint.translateMix += (num3 - transformConstraint.translateMix) * alpha;
				transformConstraint.scaleMix += (num4 - transformConstraint.scaleMix) * alpha;
				transformConstraint.shearMix += (num5 - transformConstraint.shearMix) * alpha;
			}
		}
	}
}
