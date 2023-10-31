using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spine
{
	public class SkeletonBinary
	{
		internal class Vertices
		{
			public int[] bones;

			public float[] vertices;
		}

		public const int BONE_ROTATE = 0;

		public const int BONE_TRANSLATE = 1;

		public const int BONE_SCALE = 2;

		public const int BONE_SHEAR = 3;

		public const int SLOT_ATTACHMENT = 0;

		public const int SLOT_COLOR = 1;

		public const int PATH_POSITION = 0;

		public const int PATH_SPACING = 1;

		public const int PATH_MIX = 2;

		public const int CURVE_LINEAR = 0;

		public const int CURVE_STEPPED = 1;

		public const int CURVE_BEZIER = 2;

		private AttachmentLoader attachmentLoader;

		private byte[] buffer = new byte[32];

		private List<SkeletonJson.LinkedMesh> linkedMeshes = new List<SkeletonJson.LinkedMesh>();

		public static readonly TransformMode[] TransformModeValues = new TransformMode[5]
		{
			TransformMode.Normal,
			TransformMode.OnlyTranslation,
			TransformMode.NoRotationOrReflection,
			TransformMode.NoScale,
			TransformMode.NoScaleOrReflection
		};

		public float Scale
		{
			get;
			set;
		}

		public SkeletonBinary(params Atlas[] atlasArray)
			: this(new AtlasAttachmentLoader(atlasArray))
		{
		}

		public SkeletonBinary(AttachmentLoader attachmentLoader)
		{
			if (attachmentLoader == null)
			{
				throw new ArgumentNullException("attachmentLoader");
			}
			this.attachmentLoader = attachmentLoader;
			Scale = 1f;
		}

		public SkeletonData ReadSkeletonData(string path)
		{
			using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				SkeletonData skeletonData = ReadSkeletonData(input);
				skeletonData.name = Path.GetFileNameWithoutExtension(path);
				return skeletonData;
			}
		}

		public static string GetVersionString(Stream input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			try
			{
				int num = ReadVarint(input, optimizePositive: true);
				if (num > 1)
				{
					input.Position += num - 1;
				}
				num = ReadVarint(input, optimizePositive: true);
				if (num <= 1)
				{
					throw new ArgumentException("Stream does not contain a valid binary Skeleton Data.", "input");
				}
				num--;
				byte[] bytes = new byte[num];
				ReadFully(input, bytes, 0, num);
				return Encoding.UTF8.GetString(bytes, 0, num);
			}
			catch (Exception arg)
			{
				throw new ArgumentException("Stream does not contain a valid binary Skeleton Data.\n" + arg, "input");
			}
		}

		public SkeletonData ReadSkeletonData(Stream input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			float scale = Scale;
			SkeletonData skeletonData = new SkeletonData();
			skeletonData.hash = ReadString(input);
			if (skeletonData.hash.Length == 0)
			{
				skeletonData.hash = null;
			}
			skeletonData.version = ReadString(input);
			if (skeletonData.version.Length == 0)
			{
				skeletonData.version = null;
			}
			skeletonData.width = ReadFloat(input);
			skeletonData.height = ReadFloat(input);
			bool flag = ReadBoolean(input);
			if (flag)
			{
				skeletonData.fps = ReadFloat(input);
				skeletonData.imagesPath = ReadString(input);
				if (skeletonData.imagesPath.Length == 0)
				{
					skeletonData.imagesPath = null;
				}
			}
			int i = 0;
			for (int num = ReadVarint(input, optimizePositive: true); i < num; i++)
			{
				string name = ReadString(input);
				BoneData parent = (i != 0) ? skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)] : null;
				BoneData boneData = new BoneData(i, name, parent);
				boneData.rotation = ReadFloat(input);
				boneData.x = ReadFloat(input) * scale;
				boneData.y = ReadFloat(input) * scale;
				boneData.scaleX = ReadFloat(input);
				boneData.scaleY = ReadFloat(input);
				boneData.shearX = ReadFloat(input);
				boneData.shearY = ReadFloat(input);
				boneData.length = ReadFloat(input) * scale;
				boneData.transformMode = TransformModeValues[ReadVarint(input, optimizePositive: true)];
				if (flag)
				{
					ReadInt(input);
				}
				skeletonData.bones.Add(boneData);
			}
			int j = 0;
			for (int num2 = ReadVarint(input, optimizePositive: true); j < num2; j++)
			{
				string name2 = ReadString(input);
				BoneData boneData2 = skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)];
				SlotData slotData = new SlotData(j, name2, boneData2);
				int num3 = ReadInt(input);
				slotData.r = (float)((num3 & 4278190080u) >> 24) / 255f;
				slotData.g = (float)((num3 & 0xFF0000) >> 16) / 255f;
				slotData.b = (float)((num3 & 0xFF00) >> 8) / 255f;
				slotData.a = (float)(num3 & 0xFF) / 255f;
				slotData.attachmentName = ReadString(input);
				slotData.blendMode = (BlendMode)ReadVarint(input, optimizePositive: true);
				skeletonData.slots.Add(slotData);
			}
			int k = 0;
			for (int num4 = ReadVarint(input, optimizePositive: true); k < num4; k++)
			{
				IkConstraintData ikConstraintData = new IkConstraintData(ReadString(input));
				ikConstraintData.order = ReadVarint(input, optimizePositive: true);
				int l = 0;
				for (int num5 = ReadVarint(input, optimizePositive: true); l < num5; l++)
				{
					ikConstraintData.bones.Add(skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)]);
				}
				ikConstraintData.target = skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)];
				ikConstraintData.mix = ReadFloat(input);
				ikConstraintData.bendDirection = ReadSByte(input);
				skeletonData.ikConstraints.Add(ikConstraintData);
			}
			int m = 0;
			for (int num6 = ReadVarint(input, optimizePositive: true); m < num6; m++)
			{
				TransformConstraintData transformConstraintData = new TransformConstraintData(ReadString(input));
				transformConstraintData.order = ReadVarint(input, optimizePositive: true);
				int n = 0;
				for (int num7 = ReadVarint(input, optimizePositive: true); n < num7; n++)
				{
					transformConstraintData.bones.Add(skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)]);
				}
				transformConstraintData.target = skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)];
				transformConstraintData.offsetRotation = ReadFloat(input);
				transformConstraintData.offsetX = ReadFloat(input) * scale;
				transformConstraintData.offsetY = ReadFloat(input) * scale;
				transformConstraintData.offsetScaleX = ReadFloat(input);
				transformConstraintData.offsetScaleY = ReadFloat(input);
				transformConstraintData.offsetShearY = ReadFloat(input);
				transformConstraintData.rotateMix = ReadFloat(input);
				transformConstraintData.translateMix = ReadFloat(input);
				transformConstraintData.scaleMix = ReadFloat(input);
				transformConstraintData.shearMix = ReadFloat(input);
				skeletonData.transformConstraints.Add(transformConstraintData);
			}
			int num8 = 0;
			for (int num9 = ReadVarint(input, optimizePositive: true); num8 < num9; num8++)
			{
				PathConstraintData pathConstraintData = new PathConstraintData(ReadString(input));
				pathConstraintData.order = ReadVarint(input, optimizePositive: true);
				int num10 = 0;
				for (int num11 = ReadVarint(input, optimizePositive: true); num10 < num11; num10++)
				{
					pathConstraintData.bones.Add(skeletonData.bones.Items[ReadVarint(input, optimizePositive: true)]);
				}
				pathConstraintData.target = skeletonData.slots.Items[ReadVarint(input, optimizePositive: true)];
				pathConstraintData.positionMode = (PositionMode)Enum.GetValues(typeof(PositionMode)).GetValue(ReadVarint(input, optimizePositive: true));
				pathConstraintData.spacingMode = (SpacingMode)Enum.GetValues(typeof(SpacingMode)).GetValue(ReadVarint(input, optimizePositive: true));
				pathConstraintData.rotateMode = (RotateMode)Enum.GetValues(typeof(RotateMode)).GetValue(ReadVarint(input, optimizePositive: true));
				pathConstraintData.offsetRotation = ReadFloat(input);
				pathConstraintData.position = ReadFloat(input);
				if (pathConstraintData.positionMode == PositionMode.Fixed)
				{
					pathConstraintData.position *= scale;
				}
				pathConstraintData.spacing = ReadFloat(input);
				if (pathConstraintData.spacingMode == SpacingMode.Length || pathConstraintData.spacingMode == SpacingMode.Fixed)
				{
					pathConstraintData.spacing *= scale;
				}
				pathConstraintData.rotateMix = ReadFloat(input);
				pathConstraintData.translateMix = ReadFloat(input);
				skeletonData.pathConstraints.Add(pathConstraintData);
			}
			Skin skin = ReadSkin(input, "default", flag);
			if (skin != null)
			{
				skeletonData.defaultSkin = skin;
				skeletonData.skins.Add(skin);
			}
			int num12 = 0;
			for (int num13 = ReadVarint(input, optimizePositive: true); num12 < num13; num12++)
			{
				skeletonData.skins.Add(ReadSkin(input, ReadString(input), flag));
			}
			int num14 = 0;
			for (int count = linkedMeshes.Count; num14 < count; num14++)
			{
				SkeletonJson.LinkedMesh linkedMesh = linkedMeshes[num14];
				Skin skin2 = (linkedMesh.skin != null) ? skeletonData.FindSkin(linkedMesh.skin) : skeletonData.DefaultSkin;
				if (skin2 == null)
				{
					throw new Exception("Skin not found: " + linkedMesh.skin);
				}
				Attachment attachment = skin2.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
				if (attachment == null)
				{
					throw new Exception("Parent mesh not found: " + linkedMesh.parent);
				}
				linkedMesh.mesh.ParentMesh = (MeshAttachment)attachment;
				linkedMesh.mesh.UpdateUVs();
			}
			linkedMeshes.Clear();
			int num15 = 0;
			for (int num16 = ReadVarint(input, optimizePositive: true); num15 < num16; num15++)
			{
				EventData eventData = new EventData(ReadString(input));
				eventData.Int = ReadVarint(input, optimizePositive: false);
				eventData.Float = ReadFloat(input);
				eventData.String = ReadString(input);
				skeletonData.events.Add(eventData);
			}
			int num17 = 0;
			for (int num18 = ReadVarint(input, optimizePositive: true); num17 < num18; num17++)
			{
				ReadAnimation(ReadString(input), input, skeletonData);
			}
			skeletonData.bones.TrimExcess();
			skeletonData.slots.TrimExcess();
			skeletonData.skins.TrimExcess();
			skeletonData.events.TrimExcess();
			skeletonData.animations.TrimExcess();
			skeletonData.ikConstraints.TrimExcess();
			skeletonData.pathConstraints.TrimExcess();
			return skeletonData;
		}

		private Skin ReadSkin(Stream input, string skinName, bool nonessential)
		{
			int num = ReadVarint(input, optimizePositive: true);
			if (num == 0)
			{
				return null;
			}
			Skin skin = new Skin(skinName);
			for (int i = 0; i < num; i++)
			{
				int slotIndex = ReadVarint(input, optimizePositive: true);
				int j = 0;
				for (int num2 = ReadVarint(input, optimizePositive: true); j < num2; j++)
				{
					string text = ReadString(input);
					Attachment attachment = ReadAttachment(input, skin, slotIndex, text, nonessential);
					if (attachment != null)
					{
						skin.AddAttachment(slotIndex, text, attachment);
					}
				}
			}
			return skin;
		}

		private Attachment ReadAttachment(Stream input, Skin skin, int slotIndex, string attachmentName, bool nonessential)
		{
			float scale = Scale;
			string text = ReadString(input);
			if (text == null)
			{
				text = attachmentName;
			}
			switch (input.ReadByte())
			{
			case 0:
			{
				string text3 = ReadString(input);
				float rotation = ReadFloat(input);
				float num9 = ReadFloat(input);
				float num10 = ReadFloat(input);
				float scaleX = ReadFloat(input);
				float scaleY = ReadFloat(input);
				float num11 = ReadFloat(input);
				float num12 = ReadFloat(input);
				int num13 = ReadInt(input);
				if (text3 == null)
				{
					text3 = text;
				}
				RegionAttachment regionAttachment = attachmentLoader.NewRegionAttachment(skin, text, text3);
				if (regionAttachment == null)
				{
					return null;
				}
				regionAttachment.Path = text3;
				regionAttachment.x = num9 * scale;
				regionAttachment.y = num10 * scale;
				regionAttachment.scaleX = scaleX;
				regionAttachment.scaleY = scaleY;
				regionAttachment.rotation = rotation;
				regionAttachment.width = num11 * scale;
				regionAttachment.height = num12 * scale;
				regionAttachment.r = (float)((num13 & 4278190080u) >> 24) / 255f;
				regionAttachment.g = (float)((num13 & 0xFF0000) >> 16) / 255f;
				regionAttachment.b = (float)((num13 & 0xFF00) >> 8) / 255f;
				regionAttachment.a = (float)(num13 & 0xFF) / 255f;
				regionAttachment.UpdateOffset();
				return regionAttachment;
			}
			case 1:
			{
				int num8 = ReadVarint(input, optimizePositive: true);
				Vertices vertices3 = ReadVertices(input, num8);
				if (nonessential)
				{
					ReadInt(input);
				}
				BoundingBoxAttachment boundingBoxAttachment = attachmentLoader.NewBoundingBoxAttachment(skin, text);
				if (boundingBoxAttachment == null)
				{
					return null;
				}
				boundingBoxAttachment.worldVerticesLength = num8 << 1;
				boundingBoxAttachment.vertices = vertices3.vertices;
				boundingBoxAttachment.bones = vertices3.bones;
				return boundingBoxAttachment;
			}
			case 2:
			{
				string text2 = ReadString(input);
				int num3 = ReadInt(input);
				int num4 = ReadVarint(input, optimizePositive: true);
				float[] regionUVs = ReadFloatArray(input, num4 << 1, 1f);
				int[] triangles = ReadShortArray(input);
				Vertices vertices2 = ReadVertices(input, num4);
				int num5 = ReadVarint(input, optimizePositive: true);
				int[] edges = null;
				float num6 = 0f;
				float num7 = 0f;
				if (nonessential)
				{
					edges = ReadShortArray(input);
					num6 = ReadFloat(input);
					num7 = ReadFloat(input);
				}
				if (text2 == null)
				{
					text2 = text;
				}
				MeshAttachment meshAttachment = attachmentLoader.NewMeshAttachment(skin, text, text2);
				if (meshAttachment == null)
				{
					return null;
				}
				meshAttachment.Path = text2;
				meshAttachment.r = (float)((num3 & 4278190080u) >> 24) / 255f;
				meshAttachment.g = (float)((num3 & 0xFF0000) >> 16) / 255f;
				meshAttachment.b = (float)((num3 & 0xFF00) >> 8) / 255f;
				meshAttachment.a = (float)(num3 & 0xFF) / 255f;
				meshAttachment.bones = vertices2.bones;
				meshAttachment.vertices = vertices2.vertices;
				meshAttachment.WorldVerticesLength = num4 << 1;
				meshAttachment.triangles = triangles;
				meshAttachment.regionUVs = regionUVs;
				meshAttachment.UpdateUVs();
				meshAttachment.HullLength = num5 << 1;
				if (nonessential)
				{
					meshAttachment.Edges = edges;
					meshAttachment.Width = num6 * scale;
					meshAttachment.Height = num7 * scale;
				}
				return meshAttachment;
			}
			case 3:
			{
				string text4 = ReadString(input);
				int num14 = ReadInt(input);
				string skin2 = ReadString(input);
				string parent = ReadString(input);
				bool inheritDeform = ReadBoolean(input);
				float num15 = 0f;
				float num16 = 0f;
				if (nonessential)
				{
					num15 = ReadFloat(input);
					num16 = ReadFloat(input);
				}
				if (text4 == null)
				{
					text4 = text;
				}
				MeshAttachment meshAttachment2 = attachmentLoader.NewMeshAttachment(skin, text, text4);
				if (meshAttachment2 == null)
				{
					return null;
				}
				meshAttachment2.Path = text4;
				meshAttachment2.r = (float)((num14 & 4278190080u) >> 24) / 255f;
				meshAttachment2.g = (float)((num14 & 0xFF0000) >> 16) / 255f;
				meshAttachment2.b = (float)((num14 & 0xFF00) >> 8) / 255f;
				meshAttachment2.a = (float)(num14 & 0xFF) / 255f;
				meshAttachment2.inheritDeform = inheritDeform;
				if (nonessential)
				{
					meshAttachment2.Width = num15 * scale;
					meshAttachment2.Height = num16 * scale;
				}
				linkedMeshes.Add(new SkeletonJson.LinkedMesh(meshAttachment2, skin2, slotIndex, parent));
				return meshAttachment2;
			}
			case 4:
			{
				bool closed = ReadBoolean(input);
				bool constantSpeed = ReadBoolean(input);
				int num = ReadVarint(input, optimizePositive: true);
				Vertices vertices = ReadVertices(input, num);
				float[] array = new float[num / 3];
				int i = 0;
				for (int num2 = array.Length; i < num2; i++)
				{
					array[i] = ReadFloat(input) * scale;
				}
				if (nonessential)
				{
					ReadInt(input);
				}
				PathAttachment pathAttachment = attachmentLoader.NewPathAttachment(skin, text);
				if (pathAttachment == null)
				{
					return null;
				}
				pathAttachment.closed = closed;
				pathAttachment.constantSpeed = constantSpeed;
				pathAttachment.worldVerticesLength = num << 1;
				pathAttachment.vertices = vertices.vertices;
				pathAttachment.bones = vertices.bones;
				pathAttachment.lengths = array;
				return pathAttachment;
			}
			default:
				return null;
			}
		}

		private Vertices ReadVertices(Stream input, int vertexCount)
		{
			float scale = Scale;
			int num = vertexCount << 1;
			Vertices vertices = new Vertices();
			if (!ReadBoolean(input))
			{
				vertices.vertices = ReadFloatArray(input, num, scale);
				return vertices;
			}
			ExposedList<float> exposedList = new ExposedList<float>(num * 3 * 3);
			ExposedList<int> exposedList2 = new ExposedList<int>(num * 3);
			for (int i = 0; i < vertexCount; i++)
			{
				int num2 = ReadVarint(input, optimizePositive: true);
				exposedList2.Add(num2);
				for (int j = 0; j < num2; j++)
				{
					exposedList2.Add(ReadVarint(input, optimizePositive: true));
					exposedList.Add(ReadFloat(input) * scale);
					exposedList.Add(ReadFloat(input) * scale);
					exposedList.Add(ReadFloat(input));
				}
			}
			vertices.vertices = exposedList.ToArray();
			vertices.bones = exposedList2.ToArray();
			return vertices;
		}

		private float[] ReadFloatArray(Stream input, int n, float scale)
		{
			float[] array = new float[n];
			if (scale == 1f)
			{
				for (int i = 0; i < n; i++)
				{
					array[i] = ReadFloat(input);
				}
			}
			else
			{
				for (int j = 0; j < n; j++)
				{
					array[j] = ReadFloat(input) * scale;
				}
			}
			return array;
		}

		private int[] ReadShortArray(Stream input)
		{
			int num = ReadVarint(input, optimizePositive: true);
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = ((input.ReadByte() << 8) | input.ReadByte());
			}
			return array;
		}

		private void ReadAnimation(string name, Stream input, SkeletonData skeletonData)
		{
			ExposedList<Timeline> exposedList = new ExposedList<Timeline>();
			float scale = Scale;
			float num = 0f;
			int i = 0;
			for (int num2 = ReadVarint(input, optimizePositive: true); i < num2; i++)
			{
				int slotIndex = ReadVarint(input, optimizePositive: true);
				int j = 0;
				for (int num3 = ReadVarint(input, optimizePositive: true); j < num3; j++)
				{
					int num4 = input.ReadByte();
					int num5 = ReadVarint(input, optimizePositive: true);
					switch (num4)
					{
					case 1:
					{
						ColorTimeline colorTimeline = new ColorTimeline(num5);
						colorTimeline.slotIndex = slotIndex;
						for (int l = 0; l < num5; l++)
						{
							float time = ReadFloat(input);
							int num6 = ReadInt(input);
							float r = (float)((num6 & 4278190080u) >> 24) / 255f;
							float g = (float)((num6 & 0xFF0000) >> 16) / 255f;
							float b = (float)((num6 & 0xFF00) >> 8) / 255f;
							float a = (float)(num6 & 0xFF) / 255f;
							colorTimeline.SetFrame(l, time, r, g, b, a);
							if (l < num5 - 1)
							{
								ReadCurve(input, l, colorTimeline);
							}
						}
						exposedList.Add(colorTimeline);
						num = Math.Max(num, colorTimeline.frames[(colorTimeline.FrameCount - 1) * 5]);
						break;
					}
					case 0:
					{
						AttachmentTimeline attachmentTimeline = new AttachmentTimeline(num5);
						attachmentTimeline.slotIndex = slotIndex;
						for (int k = 0; k < num5; k++)
						{
							attachmentTimeline.SetFrame(k, ReadFloat(input), ReadString(input));
						}
						exposedList.Add(attachmentTimeline);
						num = Math.Max(num, attachmentTimeline.frames[num5 - 1]);
						break;
					}
					}
				}
			}
			int m = 0;
			for (int num7 = ReadVarint(input, optimizePositive: true); m < num7; m++)
			{
				int boneIndex = ReadVarint(input, optimizePositive: true);
				int n = 0;
				for (int num8 = ReadVarint(input, optimizePositive: true); n < num8; n++)
				{
					int num9 = input.ReadByte();
					int num10 = ReadVarint(input, optimizePositive: true);
					switch (num9)
					{
					case 0:
					{
						RotateTimeline rotateTimeline = new RotateTimeline(num10);
						rotateTimeline.boneIndex = boneIndex;
						for (int num13 = 0; num13 < num10; num13++)
						{
							rotateTimeline.SetFrame(num13, ReadFloat(input), ReadFloat(input));
							if (num13 < num10 - 1)
							{
								ReadCurve(input, num13, rotateTimeline);
							}
						}
						exposedList.Add(rotateTimeline);
						num = Math.Max(num, rotateTimeline.frames[(num10 - 1) * 2]);
						break;
					}
					case 1:
					case 2:
					case 3:
					{
						float num11 = 1f;
						TranslateTimeline translateTimeline;
						switch (num9)
						{
						case 2:
							translateTimeline = new ScaleTimeline(num10);
							break;
						case 3:
							translateTimeline = new ShearTimeline(num10);
							break;
						default:
							translateTimeline = new TranslateTimeline(num10);
							num11 = scale;
							break;
						}
						translateTimeline.boneIndex = boneIndex;
						for (int num12 = 0; num12 < num10; num12++)
						{
							translateTimeline.SetFrame(num12, ReadFloat(input), ReadFloat(input) * num11, ReadFloat(input) * num11);
							if (num12 < num10 - 1)
							{
								ReadCurve(input, num12, translateTimeline);
							}
						}
						exposedList.Add(translateTimeline);
						num = Math.Max(num, translateTimeline.frames[(num10 - 1) * 3]);
						break;
					}
					}
				}
			}
			int num14 = 0;
			for (int num15 = ReadVarint(input, optimizePositive: true); num14 < num15; num14++)
			{
				int ikConstraintIndex = ReadVarint(input, optimizePositive: true);
				int num16 = ReadVarint(input, optimizePositive: true);
				IkConstraintTimeline ikConstraintTimeline = new IkConstraintTimeline(num16);
				ikConstraintTimeline.ikConstraintIndex = ikConstraintIndex;
				for (int num17 = 0; num17 < num16; num17++)
				{
					ikConstraintTimeline.SetFrame(num17, ReadFloat(input), ReadFloat(input), ReadSByte(input));
					if (num17 < num16 - 1)
					{
						ReadCurve(input, num17, ikConstraintTimeline);
					}
				}
				exposedList.Add(ikConstraintTimeline);
				num = Math.Max(num, ikConstraintTimeline.frames[(num16 - 1) * 3]);
			}
			int num18 = 0;
			for (int num19 = ReadVarint(input, optimizePositive: true); num18 < num19; num18++)
			{
				int transformConstraintIndex = ReadVarint(input, optimizePositive: true);
				int num20 = ReadVarint(input, optimizePositive: true);
				TransformConstraintTimeline transformConstraintTimeline = new TransformConstraintTimeline(num20);
				transformConstraintTimeline.transformConstraintIndex = transformConstraintIndex;
				for (int num21 = 0; num21 < num20; num21++)
				{
					transformConstraintTimeline.SetFrame(num21, ReadFloat(input), ReadFloat(input), ReadFloat(input), ReadFloat(input), ReadFloat(input));
					if (num21 < num20 - 1)
					{
						ReadCurve(input, num21, transformConstraintTimeline);
					}
				}
				exposedList.Add(transformConstraintTimeline);
				num = Math.Max(num, transformConstraintTimeline.frames[(num20 - 1) * 5]);
			}
			int num22 = 0;
			for (int num23 = ReadVarint(input, optimizePositive: true); num22 < num23; num22++)
			{
				int num24 = ReadVarint(input, optimizePositive: true);
				PathConstraintData pathConstraintData = skeletonData.pathConstraints.Items[num24];
				int num25 = 0;
				for (int num26 = ReadVarint(input, optimizePositive: true); num25 < num26; num25++)
				{
					int num27 = ReadSByte(input);
					int num28 = ReadVarint(input, optimizePositive: true);
					switch (num27)
					{
					case 0:
					case 1:
					{
						float num30 = 1f;
						PathConstraintPositionTimeline pathConstraintPositionTimeline;
						if (num27 == 1)
						{
							pathConstraintPositionTimeline = new PathConstraintSpacingTimeline(num28);
							if (pathConstraintData.spacingMode == SpacingMode.Length || pathConstraintData.spacingMode == SpacingMode.Fixed)
							{
								num30 = scale;
							}
						}
						else
						{
							pathConstraintPositionTimeline = new PathConstraintPositionTimeline(num28);
							if (pathConstraintData.positionMode == PositionMode.Fixed)
							{
								num30 = scale;
							}
						}
						pathConstraintPositionTimeline.pathConstraintIndex = num24;
						for (int num31 = 0; num31 < num28; num31++)
						{
							pathConstraintPositionTimeline.SetFrame(num31, ReadFloat(input), ReadFloat(input) * num30);
							if (num31 < num28 - 1)
							{
								ReadCurve(input, num31, pathConstraintPositionTimeline);
							}
						}
						exposedList.Add(pathConstraintPositionTimeline);
						num = Math.Max(num, pathConstraintPositionTimeline.frames[(num28 - 1) * 2]);
						break;
					}
					case 2:
					{
						PathConstraintMixTimeline pathConstraintMixTimeline = new PathConstraintMixTimeline(num28);
						pathConstraintMixTimeline.pathConstraintIndex = num24;
						for (int num29 = 0; num29 < num28; num29++)
						{
							pathConstraintMixTimeline.SetFrame(num29, ReadFloat(input), ReadFloat(input), ReadFloat(input));
							if (num29 < num28 - 1)
							{
								ReadCurve(input, num29, pathConstraintMixTimeline);
							}
						}
						exposedList.Add(pathConstraintMixTimeline);
						num = Math.Max(num, pathConstraintMixTimeline.frames[(num28 - 1) * 3]);
						break;
					}
					}
				}
			}
			int num32 = 0;
			for (int num33 = ReadVarint(input, optimizePositive: true); num32 < num33; num32++)
			{
				Skin skin = skeletonData.skins.Items[ReadVarint(input, optimizePositive: true)];
				int num34 = 0;
				for (int num35 = ReadVarint(input, optimizePositive: true); num34 < num35; num34++)
				{
					int slotIndex2 = ReadVarint(input, optimizePositive: true);
					int num36 = 0;
					for (int num37 = ReadVarint(input, optimizePositive: true); num36 < num37; num36++)
					{
						VertexAttachment vertexAttachment = (VertexAttachment)skin.GetAttachment(slotIndex2, ReadString(input));
						bool flag = vertexAttachment.bones != null;
						float[] vertices = vertexAttachment.vertices;
						int num38 = (!flag) ? vertices.Length : (vertices.Length / 3 * 2);
						int num39 = ReadVarint(input, optimizePositive: true);
						DeformTimeline deformTimeline = new DeformTimeline(num39);
						deformTimeline.slotIndex = slotIndex2;
						deformTimeline.attachment = vertexAttachment;
						for (int num40 = 0; num40 < num39; num40++)
						{
							float time2 = ReadFloat(input);
							int num41 = ReadVarint(input, optimizePositive: true);
							float[] array;
							if (num41 == 0)
							{
								array = ((!flag) ? vertices : new float[num38]);
							}
							else
							{
								array = new float[num38];
								int num42 = ReadVarint(input, optimizePositive: true);
								num41 += num42;
								if (scale == 1f)
								{
									for (int num43 = num42; num43 < num41; num43++)
									{
										array[num43] = ReadFloat(input);
									}
								}
								else
								{
									for (int num44 = num42; num44 < num41; num44++)
									{
										array[num44] = ReadFloat(input) * scale;
									}
								}
								if (!flag)
								{
									int num45 = 0;
									for (int num46 = array.Length; num45 < num46; num45++)
									{
										array[num45] += vertices[num45];
									}
								}
							}
							deformTimeline.SetFrame(num40, time2, array);
							if (num40 < num39 - 1)
							{
								ReadCurve(input, num40, deformTimeline);
							}
						}
						exposedList.Add(deformTimeline);
						num = Math.Max(num, deformTimeline.frames[num39 - 1]);
					}
				}
			}
			int num47 = ReadVarint(input, optimizePositive: true);
			if (num47 > 0)
			{
				DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(num47);
				int count = skeletonData.slots.Count;
				for (int num48 = 0; num48 < num47; num48++)
				{
					float time3 = ReadFloat(input);
					int num49 = ReadVarint(input, optimizePositive: true);
					int[] array2 = new int[count];
					for (int num50 = count - 1; num50 >= 0; num50--)
					{
						array2[num50] = -1;
					}
					int[] array3 = new int[count - num49];
					int num51 = 0;
					int num52 = 0;
					for (int num53 = 0; num53 < num49; num53++)
					{
						int num54 = ReadVarint(input, optimizePositive: true);
						while (num51 != num54)
						{
							array3[num52++] = num51++;
						}
						array2[num51 + ReadVarint(input, optimizePositive: true)] = num51++;
					}
					while (num51 < count)
					{
						array3[num52++] = num51++;
					}
					for (int num60 = count - 1; num60 >= 0; num60--)
					{
						if (array2[num60] == -1)
						{
							array2[num60] = array3[--num52];
						}
					}
					drawOrderTimeline.SetFrame(num48, time3, array2);
				}
				exposedList.Add(drawOrderTimeline);
				num = Math.Max(num, drawOrderTimeline.frames[num47 - 1]);
			}
			int num61 = ReadVarint(input, optimizePositive: true);
			if (num61 > 0)
			{
				EventTimeline eventTimeline = new EventTimeline(num61);
				for (int num62 = 0; num62 < num61; num62++)
				{
					float time4 = ReadFloat(input);
					EventData eventData = skeletonData.events.Items[ReadVarint(input, optimizePositive: true)];
					Event @event = new Event(time4, eventData);
					@event.Int = ReadVarint(input, optimizePositive: false);
					@event.Float = ReadFloat(input);
					@event.String = ((!ReadBoolean(input)) ? eventData.String : ReadString(input));
					eventTimeline.SetFrame(num62, @event);
				}
				exposedList.Add(eventTimeline);
				num = Math.Max(num, eventTimeline.frames[num61 - 1]);
			}
			exposedList.TrimExcess();
			skeletonData.animations.Add(new Animation(name, exposedList, num));
		}

		private void ReadCurve(Stream input, int frameIndex, CurveTimeline timeline)
		{
			switch (input.ReadByte())
			{
			case 1:
				timeline.SetStepped(frameIndex);
				break;
			case 2:
				timeline.SetCurve(frameIndex, ReadFloat(input), ReadFloat(input), ReadFloat(input), ReadFloat(input));
				break;
			}
		}

		private static sbyte ReadSByte(Stream input)
		{
			int num = input.ReadByte();
			if (num == -1)
			{
				throw new EndOfStreamException();
			}
			return (sbyte)num;
		}

		private static bool ReadBoolean(Stream input)
		{
			return input.ReadByte() != 0;
		}

		private float ReadFloat(Stream input)
		{
			buffer[3] = (byte)input.ReadByte();
			buffer[2] = (byte)input.ReadByte();
			buffer[1] = (byte)input.ReadByte();
			buffer[0] = (byte)input.ReadByte();
			return BitConverter.ToSingle(buffer, 0);
		}

		private static int ReadInt(Stream input)
		{
			return (input.ReadByte() << 24) + (input.ReadByte() << 16) + (input.ReadByte() << 8) + input.ReadByte();
		}

		private static int ReadVarint(Stream input, bool optimizePositive)
		{
			int num = input.ReadByte();
			int num2 = num & 0x7F;
			if ((num & 0x80) != 0)
			{
				num = input.ReadByte();
				num2 |= (num & 0x7F) << 7;
				if ((num & 0x80) != 0)
				{
					num = input.ReadByte();
					num2 |= (num & 0x7F) << 14;
					if ((num & 0x80) != 0)
					{
						num = input.ReadByte();
						num2 |= (num & 0x7F) << 21;
						if ((num & 0x80) != 0)
						{
							num2 |= (input.ReadByte() & 0x7F) << 28;
						}
					}
				}
			}
			return (!optimizePositive) ? ((num2 >> 1) ^ -(num2 & 1)) : num2;
		}

		private string ReadString(Stream input)
		{
			int num = ReadVarint(input, optimizePositive: true);
			switch (num)
			{
			case 0:
				return null;
			case 1:
				return string.Empty;
			default:
			{
				num--;
				byte[] array = buffer;
				if (array.Length < num)
				{
					array = new byte[num];
				}
				ReadFully(input, array, 0, num);
				return Encoding.UTF8.GetString(array, 0, num);
			}
			}
		}

		private static void ReadFully(Stream input, byte[] buffer, int offset, int length)
		{
			while (true)
			{
				if (length > 0)
				{
					int num = input.Read(buffer, offset, length);
					if (num <= 0)
					{
						break;
					}
					offset += num;
					length -= num;
					continue;
				}
				return;
			}
			throw new EndOfStreamException();
		}
	}
}
