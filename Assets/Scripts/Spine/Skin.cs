using System;
using System.Collections.Generic;

namespace Spine
{
	public class Skin
	{
		public struct AttachmentKeyTuple
		{
			public readonly int slotIndex;

			public readonly string name;

			internal readonly int nameHashCode;

			public AttachmentKeyTuple(int slotIndex, string name)
			{
				this.slotIndex = slotIndex;
				this.name = name;
				nameHashCode = this.name.GetHashCode();
			}
		}

		private class AttachmentKeyTupleComparer : IEqualityComparer<AttachmentKeyTuple>
		{
			internal static readonly AttachmentKeyTupleComparer Instance = new AttachmentKeyTupleComparer();

			bool IEqualityComparer<AttachmentKeyTuple>.Equals(AttachmentKeyTuple o1, AttachmentKeyTuple o2)
			{
				return o1.slotIndex == o2.slotIndex && o1.nameHashCode == o2.nameHashCode && o1.name == o2.name;
			}

			int IEqualityComparer<AttachmentKeyTuple>.GetHashCode(AttachmentKeyTuple o)
			{
				return o.slotIndex;
			}
		}

		internal string name;

		private Dictionary<AttachmentKeyTuple, Attachment> attachments = new Dictionary<AttachmentKeyTuple, Attachment>(AttachmentKeyTupleComparer.Instance);

		public string Name => name;

		public Dictionary<AttachmentKeyTuple, Attachment> Attachments => attachments;

		public Skin(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}
			this.name = name;
		}

		public void AddAttachment(int slotIndex, string name, Attachment attachment)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment", "attachment cannot be null.");
			}
			attachments[new AttachmentKeyTuple(slotIndex, name)] = attachment;
		}

		public Attachment GetAttachment(int slotIndex, string name)
		{
			attachments.TryGetValue(new AttachmentKeyTuple(slotIndex, name), out Attachment value);
			return value;
		}

		public void FindNamesForSlot(int slotIndex, List<string> names)
		{
			if (names == null)
			{
				throw new ArgumentNullException("names", "names cannot be null.");
			}
			foreach (AttachmentKeyTuple key in attachments.Keys)
			{
				AttachmentKeyTuple current = key;
				if (current.slotIndex == slotIndex)
				{
					names.Add(current.name);
				}
			}
		}

		public void FindAttachmentsForSlot(int slotIndex, List<Attachment> attachments)
		{
			if (attachments == null)
			{
				throw new ArgumentNullException("attachments", "attachments cannot be null.");
			}
			foreach (KeyValuePair<AttachmentKeyTuple, Attachment> attachment in this.attachments)
			{
				AttachmentKeyTuple key = attachment.Key;
				if (key.slotIndex == slotIndex)
				{
					attachments.Add(attachment.Value);
				}
			}
		}

		public override string ToString()
		{
			return name;
		}

		internal void AttachAll(Skeleton skeleton, Skin oldSkin)
		{
			foreach (KeyValuePair<AttachmentKeyTuple, Attachment> attachment2 in oldSkin.attachments)
			{
				AttachmentKeyTuple key = attachment2.Key;
				int slotIndex = key.slotIndex;
				Slot slot = skeleton.slots.Items[slotIndex];
				if (slot.Attachment == attachment2.Value)
				{
					int slotIndex2 = slotIndex;
					AttachmentKeyTuple key2 = attachment2.Key;
					Attachment attachment = GetAttachment(slotIndex2, key2.name);
					if (attachment != null)
					{
						slot.Attachment = attachment;
					}
				}
			}
		}
	}
}
