namespace Spine
{
	public class AttachmentTimeline : Timeline
	{
		internal int slotIndex;

		internal float[] frames;

		private string[] attachmentNames;

		public int SlotIndex
		{
			get
			{
				return slotIndex;
			}
			set
			{
				slotIndex = value;
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

		public string[] AttachmentNames
		{
			get
			{
				return attachmentNames;
			}
			set
			{
				attachmentNames = value;
			}
		}

		public int FrameCount => frames.Length;

		public int PropertyId => 67108864 + slotIndex;

		public AttachmentTimeline(int frameCount)
		{
			frames = new float[frameCount];
			attachmentNames = new string[frameCount];
		}

		public void SetFrame(int frameIndex, float time, string attachmentName)
		{
			frames[frameIndex] = time;
			attachmentNames[frameIndex] = attachmentName;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, bool setupPose, bool mixingOut)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			if (mixingOut && setupPose)
			{
				string attachmentName = slot.data.attachmentName;
				slot.Attachment = ((attachmentName != null) ? skeleton.GetAttachment(slotIndex, attachmentName) : null);
				return;
			}
			float[] array = frames;
			if (time < array[0])
			{
				if (setupPose)
				{
					string attachmentName = slot.data.attachmentName;
					slot.Attachment = ((attachmentName != null) ? skeleton.GetAttachment(slotIndex, attachmentName) : null);
				}
			}
			else
			{
				int num = (!(time >= array[array.Length - 1])) ? (Animation.BinarySearch(array, time, 1) - 1) : (array.Length - 1);
				string attachmentName = attachmentNames[num];
				slot.Attachment = ((attachmentName != null) ? skeleton.GetAttachment(slotIndex, attachmentName) : null);
			}
		}
	}
}
