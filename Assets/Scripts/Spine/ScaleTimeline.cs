using System;

namespace Spine
{
	public class ScaleTimeline : TranslateTimeline
	{
		public override int PropertyId => 33554432 + boneIndex;

		public ScaleTimeline(int frameCount)
			: base(frameCount)
		{
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, bool setupPose, bool mixingOut)
		{
			Bone bone = skeleton.bones.Items[boneIndex];
			float[] frames = base.frames;
			if (time < frames[0])
			{
				if (setupPose)
				{
					bone.scaleX = bone.data.scaleX;
					bone.scaleY = bone.data.scaleY;
				}
				return;
			}
			float num;
			float num2;
			if (time >= frames[frames.Length - 3])
			{
				num = frames[frames.Length + -2] * bone.data.scaleX;
				num2 = frames[frames.Length + -1] * bone.data.scaleY;
			}
			else
			{
				int num3 = Animation.BinarySearch(frames, time, 3);
				num = frames[num3 + -2];
				num2 = frames[num3 + -1];
				float num4 = frames[num3];
				float curvePercent = GetCurvePercent(num3 / 3 - 1, 1f - (time - num4) / (frames[num3 + -3] - num4));
				num = (num + (frames[num3 + 1] - num) * curvePercent) * bone.data.scaleX;
				num2 = (num2 + (frames[num3 + 2] - num2) * curvePercent) * bone.data.scaleY;
			}
			if (alpha == 1f)
			{
				bone.scaleX = num;
				bone.scaleY = num2;
				return;
			}
			float num5;
			float num6;
			if (setupPose)
			{
				num5 = bone.data.scaleX;
				num6 = bone.data.scaleY;
			}
			else
			{
				num5 = bone.scaleX;
				num6 = bone.scaleY;
			}
			if (mixingOut)
			{
				num = Math.Abs(num) * (float)Math.Sign(num5);
				num2 = Math.Abs(num2) * (float)Math.Sign(num6);
			}
			else
			{
				num5 = Math.Abs(num5) * (float)Math.Sign(num);
				num6 = Math.Abs(num6) * (float)Math.Sign(num2);
			}
			bone.scaleX = num5 + (num - num5) * alpha;
			bone.scaleY = num6 + (num2 - num6) * alpha;
		}
	}
}
