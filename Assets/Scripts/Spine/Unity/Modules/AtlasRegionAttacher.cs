using Spine.Unity.Modules.AttachmentTools;
using System;
using System.Collections;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public class AtlasRegionAttacher : MonoBehaviour
	{
		[Serializable]
		public class SlotRegionPair
		{
			[SpineSlot("", "", false)]
			public string slot;

			[SpineAtlasRegion("")]
			public string region;
		}

		public AtlasAsset atlasAsset;

		public SlotRegionPair[] attachments;

		private Atlas atlas;

		private void Awake()
		{
			SkeletonRenderer component = GetComponent<SkeletonRenderer>();
			component.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Combine(component.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(Apply));
		}

		private void Apply(SkeletonRenderer skeletonRenderer)
		{
			atlas = atlasAsset.GetAtlas();
			float scale = skeletonRenderer.skeletonDataAsset.scale;
			IEnumerator enumerator = attachments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SlotRegionPair slotRegionPair = (SlotRegionPair)enumerator.Current;
				Slot slot = skeletonRenderer.skeleton.FindSlot(slotRegionPair.slot);
				AtlasRegion region = atlas.FindRegion(slotRegionPair.region);
				slot.Attachment = region.ToRegionAttachment(slotRegionPair.region, scale);
			}
		}
	}
}
