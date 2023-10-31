using System;

namespace Spine
{
	public static class SkeletonExtensions
	{
		public static bool IsWeighted(this VertexAttachment va)
		{
			return va.bones != null && va.bones.Length > 0;
		}

		public static bool InheritsRotation(this TransformMode mode)
		{
			return ((long)mode & 1L) == 0;
		}

		public static bool InheritsScale(this TransformMode mode)
		{
			return ((long)mode & 2L) == 0;
		}

		[Obsolete("Old Animation.Apply method signature. Please use the 8 parameter signature. See summary to learn about the extra arguments.")]
		public static void Apply(this Animation animation, Skeleton skeleton, float lastTime, float time, bool loop, ExposedList<Event> events)
		{
			animation.Apply(skeleton, lastTime, time, loop, events, 1f, setupPose: false, mixingOut: false);
		}

		public static void SetDrawOrderToSetupPose(this Skeleton skeleton)
		{
			Slot[] items = skeleton.slots.Items;
			int count = skeleton.slots.Count;
			ExposedList<Slot> drawOrder = skeleton.drawOrder;
			drawOrder.Clear(clearArray: false);
			drawOrder.GrowIfNeeded(count);
			Array.Copy(items, drawOrder.Items, count);
		}

		public static void SetColorToSetupPose(this Slot slot)
		{
			slot.r = slot.data.r;
			slot.g = slot.data.g;
			slot.b = slot.data.b;
			slot.a = slot.data.a;
		}

		public static void SetAttachmentToSetupPose(this Slot slot)
		{
			SlotData data = slot.data;
			slot.Attachment = slot.bone.skeleton.GetAttachment(data.name, data.attachmentName);
		}

		public static void SetSlotAttachmentToSetupPose(this Skeleton skeleton, int slotIndex)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			string attachmentName = slot.data.attachmentName;
			if (string.IsNullOrEmpty(attachmentName))
			{
				slot.Attachment = null;
			}
			else
			{
				Attachment attachment2 = slot.Attachment = skeleton.GetAttachment(slotIndex, attachmentName);
			}
		}

		public static void PoseWithAnimation(this Skeleton skeleton, string animationName, float time, bool loop)
		{
			skeleton.data.FindAnimation(animationName)?.Apply(skeleton, 0f, time, loop, null, 1f, setupPose: false, mixingOut: false);
		}

		public static void SetKeyedItemsToSetupPose(this Animation animation, Skeleton skeleton)
		{
			animation.Apply(skeleton, 0f, 0f, loop: false, null, 0f, setupPose: true, mixingOut: true);
		}
	}
}
