using System;

namespace Spine
{
	public class DrawOrderTimeline : Timeline
	{
		internal float[] frames;

		private int[][] drawOrders;

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

		public int[][] DrawOrders
		{
			get
			{
				return drawOrders;
			}
			set
			{
				drawOrders = value;
			}
		}

		public int FrameCount => frames.Length;

		public int PropertyId => 134217728;

		public DrawOrderTimeline(int frameCount)
		{
			frames = new float[frameCount];
			drawOrders = new int[frameCount][];
		}

		public void SetFrame(int frameIndex, float time, int[] drawOrder)
		{
			frames[frameIndex] = time;
			drawOrders[frameIndex] = drawOrder;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, bool setupPose, bool mixingOut)
		{
			ExposedList<Slot> drawOrder = skeleton.drawOrder;
			ExposedList<Slot> slots = skeleton.slots;
			if (mixingOut && setupPose)
			{
				Array.Copy(slots.Items, 0, drawOrder.Items, 0, slots.Count);
				return;
			}
			float[] array = frames;
			if (time < array[0])
			{
				if (setupPose)
				{
					Array.Copy(slots.Items, 0, drawOrder.Items, 0, slots.Count);
				}
				return;
			}
			int num = (!(time >= array[array.Length - 1])) ? (Animation.BinarySearch(array, time) - 1) : (array.Length - 1);
			int[] array2 = drawOrders[num];
			if (array2 == null)
			{
				drawOrder.Clear();
				int i = 0;
				for (int count = slots.Count; i < count; i++)
				{
					drawOrder.Add(slots.Items[i]);
				}
				return;
			}
			Slot[] items = drawOrder.Items;
			Slot[] items2 = slots.Items;
			int j = 0;
			for (int num2 = array2.Length; j < num2; j++)
			{
				items[j] = items2[array2[j]];
			}
		}
	}
}
