using System;
using UnityEngine;

namespace Spine.Unity
{
	public static class SkeletonExtensions
	{
		private const float ByteToFloat = 0.003921569f;

		public static Color GetColor(this Skeleton s)
		{
			return new Color(s.r, s.g, s.b, s.a);
		}

		public static Color GetColor(this RegionAttachment a)
		{
			return new Color(a.r, a.g, a.b, a.a);
		}

		public static Color GetColor(this MeshAttachment a)
		{
			return new Color(a.r, a.g, a.b, a.a);
		}

		public static void SetColor(this Skeleton skeleton, Color color)
		{
			skeleton.A = color.a;
			skeleton.R = color.r;
			skeleton.G = color.g;
			skeleton.B = color.b;
		}

		public static void SetColor(this Skeleton skeleton, Color32 color)
		{
			skeleton.A = (float)(int)color.a * 0.003921569f;
			skeleton.R = (float)(int)color.r * 0.003921569f;
			skeleton.G = (float)(int)color.g * 0.003921569f;
			skeleton.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this Slot slot, Color color)
		{
			slot.A = color.a;
			slot.R = color.r;
			slot.G = color.g;
			slot.B = color.b;
		}

		public static void SetColor(this Slot slot, Color32 color)
		{
			slot.A = (float)(int)color.a * 0.003921569f;
			slot.R = (float)(int)color.r * 0.003921569f;
			slot.G = (float)(int)color.g * 0.003921569f;
			slot.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this RegionAttachment attachment, Color color)
		{
			attachment.A = color.a;
			attachment.R = color.r;
			attachment.G = color.g;
			attachment.B = color.b;
		}

		public static void SetColor(this RegionAttachment attachment, Color32 color)
		{
			attachment.A = (float)(int)color.a * 0.003921569f;
			attachment.R = (float)(int)color.r * 0.003921569f;
			attachment.G = (float)(int)color.g * 0.003921569f;
			attachment.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetColor(this MeshAttachment attachment, Color color)
		{
			attachment.A = color.a;
			attachment.R = color.r;
			attachment.G = color.g;
			attachment.B = color.b;
		}

		public static void SetColor(this MeshAttachment attachment, Color32 color)
		{
			attachment.A = (float)(int)color.a * 0.003921569f;
			attachment.R = (float)(int)color.r * 0.003921569f;
			attachment.G = (float)(int)color.g * 0.003921569f;
			attachment.B = (float)(int)color.b * 0.003921569f;
		}

		public static void SetPosition(this Bone bone, Vector2 position)
		{
			bone.X = position.x;
			bone.Y = position.y;
		}

		public static void SetPosition(this Bone bone, Vector3 position)
		{
			bone.X = position.x;
			bone.Y = position.y;
		}

		public static Vector2 GetLocalPosition(this Bone bone)
		{
			return new Vector2(bone.x, bone.y);
		}

		public static Vector2 GetSkeletonSpacePosition(this Bone bone)
		{
			return new Vector2(bone.worldX, bone.worldY);
		}

		public static Vector2 GetSkeletonSpacePosition(this Bone bone, Vector2 boneLocal)
		{
			Vector2 result = default(Vector2);
			bone.LocalToWorld(boneLocal.x, boneLocal.y, out result.x, out result.y);
			return result;
		}

		public static Vector3 GetWorldPosition(this Bone bone, Transform parentTransform)
		{
			return parentTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY));
		}

		public static Matrix4x4 GetMatrix4x4(this Bone bone)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.m00 = bone.a;
			result.m01 = bone.b;
			result.m03 = bone.worldX;
			result.m10 = bone.c;
			result.m11 = bone.d;
			result.m13 = bone.worldY;
			result.m33 = 1f;
			return result;
		}

		public static void GetWorldToLocalMatrix(this Bone bone, out float ia, out float ib, out float ic, out float id)
		{
			float a = bone.a;
			float b = bone.b;
			float c = bone.c;
			float d = bone.d;
			float num = 1f / (a * d - b * c);
			ia = num * d;
			ib = num * (0f - b);
			ic = num * (0f - c);
			id = num * a;
		}

		public static Vector2 WorldToLocal(this Bone bone, Vector2 worldPosition)
		{
			Vector2 result = default(Vector2);
			bone.WorldToLocal(worldPosition.x, worldPosition.y, out result.x, out result.y);
			return result;
		}

		public static Material GetMaterial(this Attachment a)
		{
			object obj = null;
			RegionAttachment regionAttachment = a as RegionAttachment;
			if (regionAttachment != null)
			{
				obj = regionAttachment.RendererObject;
			}
			MeshAttachment meshAttachment = a as MeshAttachment;
			if (meshAttachment != null)
			{
				obj = meshAttachment.RendererObject;
			}
			if (obj == null)
			{
				return null;
			}
			return (Material)((AtlasRegion)obj).page.rendererObject;
		}

		public static Vector2[] GetLocalVertices(this VertexAttachment va, Slot slot, Vector2[] buffer)
		{
			int worldVerticesLength = va.worldVerticesLength;
			int num = worldVerticesLength >> 1;
			buffer = (buffer ?? new Vector2[num]);
			if (buffer.Length < num)
			{
				throw new ArgumentException($"Vector2 buffer too small. {va.Name} requires an array of size {worldVerticesLength}. Use the attachment's .WorldVerticesLength to get the correct size.", "buffer");
			}
			if (va.bones == null)
			{
				float[] vertices = va.vertices;
				for (int i = 0; i < num; i++)
				{
					int num2 = i * 2;
					buffer[i] = new Vector2(vertices[num2], vertices[num2 + 1]);
				}
			}
			else
			{
				float[] array = new float[worldVerticesLength];
				va.ComputeWorldVertices(slot, array);
				Bone bone = slot.bone;
				float worldX = bone.worldX;
				float worldY = bone.worldY;
				bone.GetWorldToLocalMatrix(out float ia, out float ib, out float ic, out float id);
				for (int j = 0; j < num; j++)
				{
					int num3 = j * 2;
					float num4 = array[num3] - worldX;
					float num5 = array[num3 + 1] - worldY;
					buffer[j] = new Vector2(num4 * ia + num5 * ib, num4 * ic + num5 * id);
				}
			}
			return buffer;
		}

		public static Vector2[] GetWorldVertices(this VertexAttachment a, Slot slot, Vector2[] buffer)
		{
			int worldVerticesLength = a.worldVerticesLength;
			int num = worldVerticesLength >> 1;
			buffer = (buffer ?? new Vector2[num]);
			if (buffer.Length < num)
			{
				throw new ArgumentException($"Vector2 buffer too small. {a.Name} requires an array of size {worldVerticesLength}. Use the attachment's .WorldVerticesLength to get the correct size.", "buffer");
			}
			float[] array = new float[worldVerticesLength];
			a.ComputeWorldVertices(slot, array);
			int i = 0;
			for (int num2 = worldVerticesLength >> 1; i < num2; i++)
			{
				int num3 = i * 2;
				buffer[i] = new Vector2(array[num3], array[num3 + 1]);
			}
			return buffer;
		}
	}
}
