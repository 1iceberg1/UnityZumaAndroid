using System;
using UnityEngine;

namespace Spine.Unity.Modules.AttachmentTools
{
	public static class AttachmentRegionExtensions
	{
		public static AtlasRegion GetRegion(this Attachment attachment)
		{
			RegionAttachment regionAttachment = attachment as RegionAttachment;
			if (regionAttachment != null)
			{
				return regionAttachment.RendererObject as AtlasRegion;
			}
			MeshAttachment meshAttachment = attachment as MeshAttachment;
			if (meshAttachment != null)
			{
				return meshAttachment.RendererObject as AtlasRegion;
			}
			return null;
		}

		public static AtlasRegion GetRegion(this RegionAttachment regionAttachment)
		{
			return regionAttachment.RendererObject as AtlasRegion;
		}

		public static AtlasRegion GetRegion(this MeshAttachment meshAttachment)
		{
			return meshAttachment.RendererObject as AtlasRegion;
		}

		public static void SetRegion(this Attachment attachment, AtlasRegion region, bool updateOffset = true)
		{
			(attachment as RegionAttachment)?.SetRegion(region, updateOffset);
			(attachment as MeshAttachment)?.SetRegion(region, updateOffset);
		}

		public static void SetRegion(this RegionAttachment attachment, AtlasRegion region, bool updateOffset = true)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			attachment.RendererObject = region;
			attachment.SetUVs(region.u, region.v, region.u2, region.v2, region.rotate);
			attachment.regionOffsetX = region.offsetX;
			attachment.regionOffsetY = region.offsetY;
			attachment.regionWidth = region.width;
			attachment.regionHeight = region.height;
			attachment.regionOriginalWidth = region.originalWidth;
			attachment.regionOriginalHeight = region.originalHeight;
			if (updateOffset)
			{
				attachment.UpdateOffset();
			}
		}

		public static void SetRegion(this MeshAttachment attachment, AtlasRegion region, bool updateUVs = true)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			attachment.RendererObject = region;
			attachment.RegionU = region.u;
			attachment.RegionV = region.v;
			attachment.RegionU2 = region.u2;
			attachment.RegionV2 = region.v2;
			attachment.RegionRotate = region.rotate;
			attachment.regionOffsetX = region.offsetX;
			attachment.regionOffsetY = region.offsetY;
			attachment.regionWidth = region.width;
			attachment.regionHeight = region.height;
			attachment.regionOriginalWidth = region.originalWidth;
			attachment.regionOriginalHeight = region.originalHeight;
			if (updateUVs)
			{
				attachment.UpdateUVs();
			}
		}

		public static RegionAttachment ToRegionAttachment(this Sprite sprite, Material material)
		{
			return sprite.ToRegionAttachment(material.ToSpineAtlasPage());
		}

		public static RegionAttachment ToRegionAttachment(this Sprite sprite, AtlasPage page)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			AtlasRegion region = sprite.ToAtlasRegion(page);
			float scale = 1f / sprite.pixelsPerUnit;
			return region.ToRegionAttachment(sprite.name, scale);
		}

		public static RegionAttachment ToRegionAttachmentPMAClone(this Sprite sprite, Shader shader)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException("sprite");
			}
			if (shader == null)
			{
				throw new ArgumentNullException("shader");
			}
			AtlasRegion region = sprite.ToAtlasRegionPMAClone(shader);
			float scale = 1f / sprite.pixelsPerUnit;
			return region.ToRegionAttachment(sprite.name, scale);
		}

		public static RegionAttachment ToRegionAttachment(this AtlasRegion region, string attachmentName, float scale = 0.01f)
		{
			if (string.IsNullOrEmpty(attachmentName))
			{
				throw new ArgumentException("attachmentName can't be null or empty.", "attachmentName");
			}
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}
			RegionAttachment regionAttachment = new RegionAttachment(attachmentName);
			regionAttachment.RendererObject = region;
			regionAttachment.SetUVs(region.u, region.v, region.u2, region.v2, region.rotate);
			regionAttachment.regionOffsetX = region.offsetX;
			regionAttachment.regionOffsetY = region.offsetY;
			regionAttachment.regionWidth = region.width;
			regionAttachment.regionHeight = region.height;
			regionAttachment.regionOriginalWidth = region.originalWidth;
			regionAttachment.regionOriginalHeight = region.originalHeight;
			regionAttachment.Path = region.name;
			regionAttachment.scaleX = 1f;
			regionAttachment.scaleY = 1f;
			regionAttachment.rotation = 0f;
			regionAttachment.width = regionAttachment.regionOriginalWidth * scale;
			regionAttachment.height = regionAttachment.regionOriginalHeight * scale;
			regionAttachment.SetColor(Color.white);
			regionAttachment.UpdateOffset();
			return regionAttachment;
		}

		public static void SetScale(this RegionAttachment regionAttachment, Vector2 scale)
		{
			regionAttachment.scaleX = scale.x;
			regionAttachment.scaleY = scale.y;
		}

		public static void SetScale(this RegionAttachment regionAttachment, float x, float y)
		{
			regionAttachment.scaleX = x;
			regionAttachment.scaleY = y;
		}

		public static void SetPositionOffset(this RegionAttachment regionAttachment, Vector2 offset)
		{
			regionAttachment.x = offset.x;
			regionAttachment.y = offset.y;
		}

		public static void SetPositionOffset(this RegionAttachment regionAttachment, float x, float y)
		{
			regionAttachment.x = x;
			regionAttachment.y = y;
		}

		public static void SetRotation(this RegionAttachment regionAttachment, float rotation)
		{
			regionAttachment.rotation = rotation;
		}
	}
}
