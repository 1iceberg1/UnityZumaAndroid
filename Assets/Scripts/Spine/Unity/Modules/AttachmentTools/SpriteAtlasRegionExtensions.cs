using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules.AttachmentTools
{
	public static class SpriteAtlasRegionExtensions
	{
		public static AtlasPage ToSpineAtlasPage(this Material m)
		{
			AtlasPage atlasPage = new AtlasPage();
			atlasPage.rendererObject = m;
			atlasPage.name = m.name;
			AtlasPage atlasPage2 = atlasPage;
			Texture mainTexture = m.mainTexture;
			if (mainTexture != null)
			{
				atlasPage2.width = mainTexture.width;
				atlasPage2.height = mainTexture.height;
			}
			return atlasPage2;
		}

		public static AtlasRegion ToAtlasRegion(this Sprite s, AtlasPage page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page", "page cannot be null. AtlasPage determines which texture region belongs and how it should be rendered. You can use material.ToSpineAtlasPage() to get a shareable AtlasPage from a Material, or use the sprite.ToAtlasRegion(material) overload.");
			}
			AtlasRegion atlasRegion = s.ToAtlasRegion();
			atlasRegion.page = page;
			return atlasRegion;
		}

		public static AtlasRegion ToAtlasRegion(this Sprite s, Material material)
		{
			AtlasRegion atlasRegion = s.ToAtlasRegion();
			atlasRegion.page = material.ToSpineAtlasPage();
			return atlasRegion;
		}

		public static AtlasRegion ToAtlasRegionPMAClone(this Sprite s, Shader shader)
		{
			Material material = new Material(shader);
			Texture2D texture2D = s.ToTexture(applyImmediately: false);
			texture2D.ApplyPMA();
			texture2D.name = s.name + "-pma-";
			material.name = texture2D.name + shader.name;
			material.mainTexture = texture2D;
			AtlasPage page = material.ToSpineAtlasPage();
			AtlasRegion atlasRegion = s.ToAtlasRegion(isolatedTexture: true);
			atlasRegion.page = page;
			return atlasRegion;
		}

		private static AtlasRegion ToAtlasRegion(this Sprite s, bool isolatedTexture = false)
		{
			AtlasRegion atlasRegion = new AtlasRegion();
			atlasRegion.name = s.name;
			atlasRegion.index = -1;
			atlasRegion.rotate = (s.packed && s.packingRotation != SpritePackingRotation.None);
			Bounds bounds = s.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.max;
			Rect rect = s.rect.SpineUnityFlipRect(s.texture.height);
			atlasRegion.width = (int)rect.width;
			atlasRegion.originalWidth = (int)rect.width;
			atlasRegion.height = (int)rect.height;
			atlasRegion.originalHeight = (int)rect.height;
			atlasRegion.offsetX = rect.width * (0.5f - InverseLerp(vector.x, vector2.x, 0f));
			atlasRegion.offsetY = rect.height * (0.5f - InverseLerp(vector.y, vector2.y, 0f));
			if (isolatedTexture)
			{
				atlasRegion.u = 0f;
				atlasRegion.v = 1f;
				atlasRegion.u2 = 1f;
				atlasRegion.v2 = 0f;
				atlasRegion.x = 0;
				atlasRegion.y = 0;
			}
			else
			{
				Texture2D texture = s.texture;
				Rect rect2 = TextureRectToUVRect(s.textureRect, texture.width, texture.height);
				atlasRegion.u = rect2.xMin;
				atlasRegion.v = rect2.yMax;
				atlasRegion.u2 = rect2.xMax;
				atlasRegion.v2 = rect2.yMin;
				atlasRegion.x = (int)rect.x;
				atlasRegion.y = (int)rect.y;
			}
			return atlasRegion;
		}

		public static Skin GetRepackedSkin(this Skin o, string newName, Shader shader, out Material m, out Texture2D t, int maxAtlasSize = 1024, int padding = 2)
		{
			Dictionary<Skin.AttachmentKeyTuple, Attachment> attachments = o.Attachments;
			Skin skin = new Skin(newName);
			List<Attachment> list = new List<Attachment>();
			List<Texture2D> list2 = new List<Texture2D>();
			foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> item in attachments)
			{
				Attachment clone = item.Value.GetClone(cloneMeshesAsLinked: true);
				if (IsRenderable(clone))
				{
					list2.Add(clone.GetAtlasRegion().ToTexture());
					list.Add(clone);
				}
				Skin.AttachmentKeyTuple key = item.Key;
				skin.AddAttachment(key.slotIndex, key.name, clone);
			}
			Texture2D texture2D = new Texture2D(maxAtlasSize, maxAtlasSize);
			texture2D.name = newName;
			Rect[] array = texture2D.PackTextures(list2.ToArray(), padding, maxAtlasSize);
			Material material = new Material(shader);
			material.name = newName;
			material.mainTexture = texture2D;
			AtlasPage atlasPage = material.ToSpineAtlasPage();
			atlasPage.name = newName;
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				Attachment attachment = list[i];
				Rect uvRect = array[i];
				AtlasRegion atlasRegion = attachment.GetAtlasRegion();
				AtlasRegion region = UVRectToAtlasRegion(uvRect, atlasRegion.name, atlasPage, atlasRegion.offsetX, atlasRegion.offsetY, atlasRegion.rotate);
				attachment.SetRegion(region);
			}
			t = texture2D;
			m = material;
			return skin;
		}

		public static Sprite ToSprite(this AtlasRegion ar, float pixelsPerUnit = 100f)
		{
			return Sprite.Create(ar.GetMainTexture(), ar.GetUnityRect(), new Vector2(0.5f, 0.5f), pixelsPerUnit);
		}

		internal static Texture2D ToTexture(this AtlasRegion ar, bool applyImmediately = true)
		{
			Texture2D mainTexture = ar.GetMainTexture();
			Rect unityRect = ar.GetUnityRect(mainTexture.height);
			Texture2D texture2D = new Texture2D((int)unityRect.width, (int)unityRect.height);
			texture2D.name = ar.name;
			Color[] pixels = mainTexture.GetPixels((int)unityRect.x, (int)unityRect.y, (int)unityRect.width, (int)unityRect.height);
			texture2D.SetPixels(pixels);
			if (applyImmediately)
			{
				texture2D.Apply();
			}
			return texture2D;
		}

		private static Texture2D ToTexture(this Sprite s, bool applyImmediately = true)
		{
			Texture2D texture = s.texture;
			Rect textureRect = s.textureRect;
			Color[] pixels = texture.GetPixels((int)textureRect.x, (int)textureRect.y, (int)textureRect.width, (int)textureRect.height);
			Texture2D texture2D = new Texture2D((int)textureRect.width, (int)textureRect.height);
			texture2D.SetPixels(pixels);
			if (applyImmediately)
			{
				texture2D.Apply();
			}
			return texture2D;
		}

		private static bool IsRenderable(Attachment a)
		{
			return a is RegionAttachment || a is MeshAttachment;
		}

		private static Rect SpineUnityFlipRect(this Rect rect, int textureHeight)
		{
			rect.y = (float)textureHeight - rect.y - rect.height;
			return rect;
		}

		private static Rect GetUnityRect(this AtlasRegion region)
		{
			return region.GetSpineAtlasRect().SpineUnityFlipRect(region.page.height);
		}

		private static Rect GetUnityRect(this AtlasRegion region, int textureHeight)
		{
			return region.GetSpineAtlasRect().SpineUnityFlipRect(textureHeight);
		}

		private static Rect GetSpineAtlasRect(this AtlasRegion region, bool includeRotate = true)
		{
			if (includeRotate && region.rotate)
			{
				return new Rect(region.x, region.y, region.height, region.width);
			}
			return new Rect(region.x, region.y, region.width, region.height);
		}

		private static Rect UVRectToTextureRect(Rect uvRect, int texWidth, int texHeight)
		{
			uvRect.x *= texWidth;
			uvRect.width *= texWidth;
			uvRect.y *= texHeight;
			uvRect.height *= texHeight;
			return uvRect;
		}

		private static Rect TextureRectToUVRect(Rect textureRect, int texWidth, int texHeight)
		{
			textureRect.x = Mathf.InverseLerp(0f, texWidth, textureRect.x);
			textureRect.y = Mathf.InverseLerp(0f, texHeight, textureRect.y);
			textureRect.width = Mathf.InverseLerp(0f, texWidth, textureRect.width);
			textureRect.height = Mathf.InverseLerp(0f, texHeight, textureRect.height);
			return textureRect;
		}

		private static AtlasRegion UVRectToAtlasRegion(Rect uvRect, string name, AtlasPage page, float offsetX, float offsetY, bool rotate)
		{
			Rect rect = UVRectToTextureRect(uvRect, page.width, page.height);
			Rect rect2 = rect.SpineUnityFlipRect(page.height);
			int num = (int)rect2.width;
			int num2 = (int)rect2.height;
			int x = (int)rect2.x;
			int y = (int)rect2.y;
			AtlasRegion atlasRegion = new AtlasRegion();
			atlasRegion.page = page;
			atlasRegion.name = name;
			atlasRegion.u = uvRect.xMin;
			atlasRegion.u2 = uvRect.xMax;
			atlasRegion.v = uvRect.yMax;
			atlasRegion.v2 = uvRect.yMin;
			atlasRegion.index = -1;
			atlasRegion.width = num;
			atlasRegion.originalWidth = num;
			atlasRegion.height = num2;
			atlasRegion.originalHeight = num2;
			atlasRegion.offsetX = offsetX;
			atlasRegion.offsetY = offsetY;
			atlasRegion.x = x;
			atlasRegion.y = y;
			atlasRegion.rotate = rotate;
			return atlasRegion;
		}

		private static AtlasRegion GetAtlasRegion(this Attachment a)
		{
			RegionAttachment regionAttachment = a as RegionAttachment;
			if (regionAttachment != null)
			{
				return regionAttachment.RendererObject as AtlasRegion;
			}
			MeshAttachment meshAttachment = a as MeshAttachment;
			if (meshAttachment != null)
			{
				return meshAttachment.RendererObject as AtlasRegion;
			}
			return null;
		}

		private static Texture2D GetMainTexture(this AtlasRegion region)
		{
			Material material = region.page.rendererObject as Material;
			return material.mainTexture as Texture2D;
		}

		private static void ApplyPMA(this Texture2D texture, bool applyImmediately = true)
		{
			Color[] pixels = texture.GetPixels();
			int i = 0;
			for (int num = pixels.Length; i < num; i++)
			{
				Color color = pixels[i];
				float a = color.a;
				color.r *= a;
				color.g *= a;
				color.b *= a;
				pixels[i] = color;
			}
			texture.SetPixels(pixels);
			if (applyImmediately)
			{
				texture.Apply();
			}
		}

		private static float InverseLerp(float a, float b, float value)
		{
			return (value - a) / (b - a);
		}
	}
}
