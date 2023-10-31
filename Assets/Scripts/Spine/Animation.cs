using System;

namespace Spine
{
	public class Animation
	{
		internal ExposedList<Timeline> timelines;

		internal float duration;

		internal string name;

		public string Name => name;

		public ExposedList<Timeline> Timelines
		{
			get
			{
				return timelines;
			}
			set
			{
				timelines = value;
			}
		}

		public float Duration
		{
			get
			{
				return duration;
			}
			set
			{
				duration = value;
			}
		}

		public Animation(string name, ExposedList<Timeline> timelines, float duration)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}
			if (timelines == null)
			{
				throw new ArgumentNullException("timelines", "timelines cannot be null.");
			}
			this.name = name;
			this.timelines = timelines;
			this.duration = duration;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, bool loop, ExposedList<Event> events, float alpha, bool setupPose, bool mixingOut)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			if (loop && duration != 0f)
			{
				time %= duration;
				if (lastTime > 0f)
				{
					lastTime %= duration;
				}
			}
			ExposedList<Timeline> exposedList = timelines;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				exposedList.Items[i].Apply(skeleton, lastTime, time, events, alpha, setupPose, mixingOut);
			}
		}

		internal static int BinarySearch(float[] values, float target, int step)
		{
			int num = 0;
			int num2 = values.Length / step - 2;
			if (num2 == 0)
			{
				return step;
			}
			int num3 = (int)((uint)num2 >> 1);
			while (true)
			{
				if (values[(num3 + 1) * step] <= target)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3;
				}
				if (num == num2)
				{
					break;
				}
				num3 = (int)((uint)(num + num2) >> 1);
			}
			return (num + 1) * step;
		}

		internal static int BinarySearch(float[] values, float target)
		{
			int num = 0;
			int num2 = values.Length - 2;
			if (num2 == 0)
			{
				return 1;
			}
			int num3 = (int)((uint)num2 >> 1);
			while (true)
			{
				if (values[num3 + 1] <= target)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3;
				}
				if (num == num2)
				{
					break;
				}
				num3 = (int)((uint)(num + num2) >> 1);
			}
			return num + 1;
		}

		internal static int LinearSearch(float[] values, float target, int step)
		{
			int i = 0;
			for (int num = values.Length - step; i <= num; i += step)
			{
				if (values[i] > target)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
