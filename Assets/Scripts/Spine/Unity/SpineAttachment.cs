using System;

namespace Spine.Unity
{
	public class SpineAttachment : SpineAttributeBase
	{
		public struct Hierarchy
		{
			public string skin;

			public string slot;

			public string name;

			public Hierarchy(string fullPath)
			{
				string[] array = fullPath.Split(new char[1]
				{
					'/'
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 0)
				{
					skin = string.Empty;
					slot = string.Empty;
					name = string.Empty;
					return;
				}
				if (array.Length < 2)
				{
					throw new Exception("Cannot generate Attachment Hierarchy from string! Not enough components! [" + fullPath + "]");
				}
				skin = array[0];
				slot = array[1];
				name = string.Empty;
				for (int i = 2; i < array.Length; i++)
				{
					name += array[i];
				}
			}
		}

		public bool returnAttachmentPath;

		public bool currentSkinOnly;

		public bool placeholdersOnly;

		public string slotField = string.Empty;

		public SpineAttachment(bool currentSkinOnly = true, bool returnAttachmentPath = false, bool placeholdersOnly = false, string slotField = "", string dataField = "")
		{
			this.currentSkinOnly = currentSkinOnly;
			this.returnAttachmentPath = returnAttachmentPath;
			this.placeholdersOnly = placeholdersOnly;
			this.slotField = slotField;
			base.dataField = dataField;
		}

		public static Hierarchy GetHierarchy(string fullPath)
		{
			return new Hierarchy(fullPath);
		}

		public static Attachment GetAttachment(string attachmentPath, SkeletonData skeletonData)
		{
			Hierarchy hierarchy = GetHierarchy(attachmentPath);
			if (hierarchy.name == string.Empty)
			{
				return null;
			}
			return skeletonData.FindSkin(hierarchy.skin).GetAttachment(skeletonData.FindSlotIndex(hierarchy.slot), hierarchy.name);
		}

		public static Attachment GetAttachment(string attachmentPath, SkeletonDataAsset skeletonDataAsset)
		{
			return GetAttachment(attachmentPath, skeletonDataAsset.GetSkeletonData(quiet: true));
		}
	}
}
