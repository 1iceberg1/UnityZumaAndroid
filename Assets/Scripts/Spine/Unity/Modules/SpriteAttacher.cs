using Spine.Unity.Modules.AttachmentTools;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public class SpriteAttacher : MonoBehaviour
	{
		public const string DefaultPMAShader = "Spine/Skeleton";

		public const string DefaultStraightAlphaShader = "Sprites/Default";

		public bool attachOnStart = true;

		public Sprite sprite;

		[SpineSlot("", "", false)]
		public string slot;

		private RegionAttachment attachment;

		private bool applyPMA;

		private static Dictionary<Texture, AtlasPage> atlasPageCache;

		private static AtlasPage GetPageFor(Texture texture, Shader shader)
		{
			if (atlasPageCache == null)
			{
				atlasPageCache = new Dictionary<Texture, AtlasPage>();
			}
			atlasPageCache.TryGetValue(texture, out AtlasPage value);
			if (value == null)
			{
				Material m = new Material(shader);
				value = m.ToSpineAtlasPage();
				atlasPageCache[texture] = value;
			}
			return value;
		}

		private void Start()
		{
			if (attachOnStart)
			{
				Attach();
			}
		}

		public void Attach()
		{
			ISkeletonComponent component = GetComponent<ISkeletonComponent>();
			SkeletonRenderer skeletonRenderer = component as SkeletonRenderer;
			if (skeletonRenderer != null)
			{
				applyPMA = skeletonRenderer.pmaVertexColors;
			}
			else
			{
				SkeletonGraphic skeletonGraphic = component as SkeletonGraphic;
				if (skeletonGraphic != null)
				{
					applyPMA = skeletonGraphic.SpineMeshGenerator.PremultiplyVertexColors;
				}
			}
			Shader shader = (!applyPMA) ? Shader.Find("Sprites/Default") : Shader.Find("Spine/Skeleton");
			attachment = ((!applyPMA) ? sprite.ToRegionAttachment(GetPageFor(sprite.texture, shader)) : sprite.ToRegionAttachmentPMAClone(shader));
			component.Skeleton.FindSlot(slot).Attachment = attachment;
		}
	}
}
