using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spine.Unity.Modules
{
	public class CustomSkin : MonoBehaviour
	{
		[Serializable]
		public class SkinPair
		{
			[SpineAttachment(false, true, false, "", "skinSource")]
			[FormerlySerializedAs("sourceAttachment")]
			public string sourceAttachmentPath;

			[SpineSlot("", "", false)]
			public string targetSlot;

			[SpineAttachment(true, false, true, "", "")]
			public string targetAttachment;
		}

		public SkeletonDataAsset skinSource;

		[FormerlySerializedAs("skinning")]
		public SkinPair[] skinItems;

		public Skin customSkin;

		private SkeletonRenderer skeletonRenderer;

		private void Start()
		{
			skeletonRenderer = GetComponent<SkeletonRenderer>();
			Skeleton skeleton = skeletonRenderer.skeleton;
			customSkin = new Skin("CustomSkin");
			SkinPair[] array = skinItems;
			foreach (SkinPair skinPair in array)
			{
				Attachment attachment = SpineAttachment.GetAttachment(skinPair.sourceAttachmentPath, skinSource);
				customSkin.AddAttachment(skeleton.FindSlotIndex(skinPair.targetSlot), skinPair.targetAttachment, attachment);
			}
			skeleton.SetSkin(customSkin);
		}
	}
}
