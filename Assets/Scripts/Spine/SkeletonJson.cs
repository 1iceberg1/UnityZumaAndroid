using System;
using System.Collections.Generic;
using System.IO;

namespace Spine
{
	public class SkeletonJson
	{
		internal class LinkedMesh
		{
			internal string parent;

			internal string skin;

			internal int slotIndex;

			internal MeshAttachment mesh;

			public LinkedMesh(MeshAttachment mesh, string skin, int slotIndex, string parent)
			{
				this.mesh = mesh;
				this.skin = skin;
				this.slotIndex = slotIndex;
				this.parent = parent;
			}
		}

		private AttachmentLoader attachmentLoader;

		private List<LinkedMesh> linkedMeshes = new List<LinkedMesh>();

		public float Scale
		{
			get;
			set;
		}

		public SkeletonJson(params Atlas[] atlasArray)
			: this(new AtlasAttachmentLoader(atlasArray))
		{
		}

		public SkeletonJson(AttachmentLoader attachmentLoader)
		{
			if (attachmentLoader == null)
			{
				throw new ArgumentNullException("attachmentLoader", "attachmentLoader cannot be null.");
			}
			this.attachmentLoader = attachmentLoader;
			Scale = 1f;
		}

		public SkeletonData ReadSkeletonData(string path)
		{
			using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
			{
				SkeletonData skeletonData = ReadSkeletonData(reader);
				skeletonData.name = Path.GetFileNameWithoutExtension(path);
				return skeletonData;
			}
		}

		public SkeletonData ReadSkeletonData(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader", "reader cannot be null.");
			}
			float scale = Scale;
			SkeletonData skeletonData = new SkeletonData();
			Dictionary<string, object> dictionary = Json.Deserialize(reader) as Dictionary<string, object>;
			if (dictionary == null)
			{
				throw new Exception("Invalid JSON.");
			}
			if (dictionary.ContainsKey("skeleton"))
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["skeleton"];
				skeletonData.hash = (string)dictionary2["hash"];
				skeletonData.version = (string)dictionary2["spine"];
				skeletonData.width = GetFloat(dictionary2, "width", 0f);
				skeletonData.height = GetFloat(dictionary2, "height", 0f);
				skeletonData.fps = GetFloat(dictionary2, "fps", 0f);
				skeletonData.imagesPath = GetString(dictionary2, "images", null);
			}
			foreach (Dictionary<string, object> item in (List<object>)dictionary["bones"])
			{
				BoneData boneData = null;
				if (item.ContainsKey("parent"))
				{
					boneData = skeletonData.FindBone((string)item["parent"]);
					if (boneData == null)
					{
						throw new Exception("Parent bone not found: " + item["parent"]);
					}
				}
				BoneData boneData2 = new BoneData(skeletonData.Bones.Count, (string)item["name"], boneData);
				boneData2.length = GetFloat(item, "length", 0f) * scale;
				boneData2.x = GetFloat(item, "x", 0f) * scale;
				boneData2.y = GetFloat(item, "y", 0f) * scale;
				boneData2.rotation = GetFloat(item, "rotation", 0f);
				boneData2.scaleX = GetFloat(item, "scaleX", 1f);
				boneData2.scaleY = GetFloat(item, "scaleY", 1f);
				boneData2.shearX = GetFloat(item, "shearX", 0f);
				boneData2.shearY = GetFloat(item, "shearY", 0f);
				string @string = GetString(item, "transform", TransformMode.Normal.ToString());
				boneData2.transformMode = (TransformMode)Enum.Parse(typeof(TransformMode), @string, ignoreCase: true);
				skeletonData.bones.Add(boneData2);
			}
			if (dictionary.ContainsKey("slots"))
			{
				foreach (Dictionary<string, object> item2 in (List<object>)dictionary["slots"])
				{
					string name = (string)item2["name"];
					string text = (string)item2["bone"];
					BoneData boneData3 = skeletonData.FindBone(text);
					if (boneData3 == null)
					{
						throw new Exception("Slot bone not found: " + text);
					}
					SlotData slotData = new SlotData(skeletonData.Slots.Count, name, boneData3);
					if (item2.ContainsKey("color"))
					{
						string hexString = (string)item2["color"];
						slotData.r = ToColor(hexString, 0);
						slotData.g = ToColor(hexString, 1);
						slotData.b = ToColor(hexString, 2);
						slotData.a = ToColor(hexString, 3);
					}
					slotData.attachmentName = GetString(item2, "attachment", null);
					if (item2.ContainsKey("blend"))
					{
						slotData.blendMode = (BlendMode)Enum.Parse(typeof(BlendMode), (string)item2["blend"], ignoreCase: false);
					}
					else
					{
						slotData.blendMode = BlendMode.normal;
					}
					skeletonData.slots.Add(slotData);
				}
			}
			if (dictionary.ContainsKey("ik"))
			{
				foreach (Dictionary<string, object> item3 in (List<object>)dictionary["ik"])
				{
					IkConstraintData ikConstraintData = new IkConstraintData((string)item3["name"]);
					ikConstraintData.order = GetInt(item3, "order", 0);
					foreach (string item4 in (List<object>)item3["bones"])
					{
						BoneData boneData4 = skeletonData.FindBone(item4);
						if (boneData4 == null)
						{
							throw new Exception("IK constraint bone not found: " + item4);
						}
						ikConstraintData.bones.Add(boneData4);
					}
					string text3 = (string)item3["target"];
					ikConstraintData.target = skeletonData.FindBone(text3);
					if (ikConstraintData.target == null)
					{
						throw new Exception("Target bone not found: " + text3);
					}
					ikConstraintData.bendDirection = (GetBoolean(item3, "bendPositive", defaultValue: true) ? 1 : (-1));
					ikConstraintData.mix = GetFloat(item3, "mix", 1f);
					skeletonData.ikConstraints.Add(ikConstraintData);
				}
			}
			if (dictionary.ContainsKey("transform"))
			{
				foreach (Dictionary<string, object> item5 in (List<object>)dictionary["transform"])
				{
					TransformConstraintData transformConstraintData = new TransformConstraintData((string)item5["name"]);
					transformConstraintData.order = GetInt(item5, "order", 0);
					foreach (string item6 in (List<object>)item5["bones"])
					{
						BoneData boneData5 = skeletonData.FindBone(item6);
						if (boneData5 == null)
						{
							throw new Exception("Transform constraint bone not found: " + item6);
						}
						transformConstraintData.bones.Add(boneData5);
					}
					string text5 = (string)item5["target"];
					transformConstraintData.target = skeletonData.FindBone(text5);
					if (transformConstraintData.target == null)
					{
						throw new Exception("Target bone not found: " + text5);
					}
					transformConstraintData.offsetRotation = GetFloat(item5, "rotation", 0f);
					transformConstraintData.offsetX = GetFloat(item5, "x", 0f) * scale;
					transformConstraintData.offsetY = GetFloat(item5, "y", 0f) * scale;
					transformConstraintData.offsetScaleX = GetFloat(item5, "scaleX", 0f);
					transformConstraintData.offsetScaleY = GetFloat(item5, "scaleY", 0f);
					transformConstraintData.offsetShearY = GetFloat(item5, "shearY", 0f);
					transformConstraintData.rotateMix = GetFloat(item5, "rotateMix", 1f);
					transformConstraintData.translateMix = GetFloat(item5, "translateMix", 1f);
					transformConstraintData.scaleMix = GetFloat(item5, "scaleMix", 1f);
					transformConstraintData.shearMix = GetFloat(item5, "shearMix", 1f);
					skeletonData.transformConstraints.Add(transformConstraintData);
				}
			}
			if (dictionary.ContainsKey("path"))
			{
				foreach (Dictionary<string, object> item7 in (List<object>)dictionary["path"])
				{
					PathConstraintData pathConstraintData = new PathConstraintData((string)item7["name"]);
					pathConstraintData.order = GetInt(item7, "order", 0);
					foreach (string item8 in (List<object>)item7["bones"])
					{
						BoneData boneData6 = skeletonData.FindBone(item8);
						if (boneData6 == null)
						{
							throw new Exception("Path bone not found: " + item8);
						}
						pathConstraintData.bones.Add(boneData6);
					}
					string text7 = (string)item7["target"];
					pathConstraintData.target = skeletonData.FindSlot(text7);
					if (pathConstraintData.target == null)
					{
						throw new Exception("Target slot not found: " + text7);
					}
					pathConstraintData.positionMode = (PositionMode)Enum.Parse(typeof(PositionMode), GetString(item7, "positionMode", "percent"), ignoreCase: true);
					pathConstraintData.spacingMode = (SpacingMode)Enum.Parse(typeof(SpacingMode), GetString(item7, "spacingMode", "length"), ignoreCase: true);
					pathConstraintData.rotateMode = (RotateMode)Enum.Parse(typeof(RotateMode), GetString(item7, "rotateMode", "tangent"), ignoreCase: true);
					pathConstraintData.offsetRotation = GetFloat(item7, "rotation", 0f);
					pathConstraintData.position = GetFloat(item7, "position", 0f);
					if (pathConstraintData.positionMode == PositionMode.Fixed)
					{
						pathConstraintData.position *= scale;
					}
					pathConstraintData.spacing = GetFloat(item7, "spacing", 0f);
					if (pathConstraintData.spacingMode == SpacingMode.Length || pathConstraintData.spacingMode == SpacingMode.Fixed)
					{
						pathConstraintData.spacing *= scale;
					}
					pathConstraintData.rotateMix = GetFloat(item7, "rotateMix", 1f);
					pathConstraintData.translateMix = GetFloat(item7, "translateMix", 1f);
					skeletonData.pathConstraints.Add(pathConstraintData);
				}
			}
			if (dictionary.ContainsKey("skins"))
			{
				foreach (KeyValuePair<string, object> item9 in (Dictionary<string, object>)dictionary["skins"])
				{
					Skin skin = new Skin(item9.Key);
					foreach (KeyValuePair<string, object> item10 in (Dictionary<string, object>)item9.Value)
					{
						int slotIndex = skeletonData.FindSlotIndex(item10.Key);
						foreach (KeyValuePair<string, object> item11 in (Dictionary<string, object>)item10.Value)
						{
							try
							{
								Attachment attachment = ReadAttachment((Dictionary<string, object>)item11.Value, skin, slotIndex, item11.Key);
								if (attachment != null)
								{
									skin.AddAttachment(slotIndex, item11.Key, attachment);
								}
							}
							catch (Exception innerException)
							{
								throw new Exception("Error reading attachment: " + item11.Key + ", skin: " + skin, innerException);
							}
						}
					}
					skeletonData.skins.Add(skin);
					if (skin.name == "default")
					{
						skeletonData.defaultSkin = skin;
					}
				}
			}
			int i = 0;
			for (int count = linkedMeshes.Count; i < count; i++)
			{
				LinkedMesh linkedMesh = linkedMeshes[i];
				Skin skin2 = (linkedMesh.skin != null) ? skeletonData.FindSkin(linkedMesh.skin) : skeletonData.defaultSkin;
				if (skin2 == null)
				{
					throw new Exception("Slot not found: " + linkedMesh.skin);
				}
				Attachment attachment2 = skin2.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
				if (attachment2 == null)
				{
					throw new Exception("Parent mesh not found: " + linkedMesh.parent);
				}
				linkedMesh.mesh.ParentMesh = (MeshAttachment)attachment2;
				linkedMesh.mesh.UpdateUVs();
			}
			linkedMeshes.Clear();
			if (dictionary.ContainsKey("events"))
			{
				foreach (KeyValuePair<string, object> item12 in (Dictionary<string, object>)dictionary["events"])
				{
					Dictionary<string, object> map = (Dictionary<string, object>)item12.Value;
					EventData eventData = new EventData(item12.Key);
					eventData.Int = GetInt(map, "int", 0);
					eventData.Float = GetFloat(map, "float", 0f);
					eventData.String = GetString(map, "string", string.Empty);
					skeletonData.events.Add(eventData);
				}
			}
			if (dictionary.ContainsKey("animations"))
			{
				foreach (KeyValuePair<string, object> item13 in (Dictionary<string, object>)dictionary["animations"])
				{
					try
					{
						ReadAnimation((Dictionary<string, object>)item13.Value, item13.Key, skeletonData);
					}
					catch (Exception innerException2)
					{
						throw new Exception("Error reading animation: " + item13.Key, innerException2);
					}
				}
			}
			skeletonData.bones.TrimExcess();
			skeletonData.slots.TrimExcess();
			skeletonData.skins.TrimExcess();
			skeletonData.events.TrimExcess();
			skeletonData.animations.TrimExcess();
			skeletonData.ikConstraints.TrimExcess();
			return skeletonData;
		}

		private Attachment ReadAttachment(Dictionary<string, object> map, Skin skin, int slotIndex, string name)
		{
			float scale = Scale;
			name = GetString(map, "name", name);
			string text = GetString(map, "type", "region");
			if (text == "skinnedmesh")
			{
				text = "weightedmesh";
			}
			if (text == "weightedmesh")
			{
				text = "mesh";
			}
			if (text == "weightedlinkedmesh")
			{
				text = "linkedmesh";
			}
			AttachmentType attachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), text, ignoreCase: true);
			string @string = GetString(map, "path", name);
			switch (attachmentType)
			{
			case AttachmentType.Region:
			{
				RegionAttachment regionAttachment = attachmentLoader.NewRegionAttachment(skin, name, @string);
				if (regionAttachment == null)
				{
					return null;
				}
				regionAttachment.Path = @string;
				regionAttachment.x = GetFloat(map, "x", 0f) * scale;
				regionAttachment.y = GetFloat(map, "y", 0f) * scale;
				regionAttachment.scaleX = GetFloat(map, "scaleX", 1f);
				regionAttachment.scaleY = GetFloat(map, "scaleY", 1f);
				regionAttachment.rotation = GetFloat(map, "rotation", 0f);
				regionAttachment.width = GetFloat(map, "width", 32f) * scale;
				regionAttachment.height = GetFloat(map, "height", 32f) * scale;
				regionAttachment.UpdateOffset();
				if (map.ContainsKey("color"))
				{
					string hexString = (string)map["color"];
					regionAttachment.r = ToColor(hexString, 0);
					regionAttachment.g = ToColor(hexString, 1);
					regionAttachment.b = ToColor(hexString, 2);
					regionAttachment.a = ToColor(hexString, 3);
				}
				regionAttachment.UpdateOffset();
				return regionAttachment;
			}
			case AttachmentType.Boundingbox:
			{
				BoundingBoxAttachment boundingBoxAttachment = attachmentLoader.NewBoundingBoxAttachment(skin, name);
				if (boundingBoxAttachment == null)
				{
					return null;
				}
				ReadVertices(map, boundingBoxAttachment, GetInt(map, "vertexCount", 0) << 1);
				return boundingBoxAttachment;
			}
			case AttachmentType.Mesh:
			case AttachmentType.Linkedmesh:
			{
				MeshAttachment meshAttachment = attachmentLoader.NewMeshAttachment(skin, name, @string);
				if (meshAttachment == null)
				{
					return null;
				}
				meshAttachment.Path = @string;
				if (map.ContainsKey("color"))
				{
					string hexString2 = (string)map["color"];
					meshAttachment.r = ToColor(hexString2, 0);
					meshAttachment.g = ToColor(hexString2, 1);
					meshAttachment.b = ToColor(hexString2, 2);
					meshAttachment.a = ToColor(hexString2, 3);
				}
				meshAttachment.Width = GetFloat(map, "width", 0f) * scale;
				meshAttachment.Height = GetFloat(map, "height", 0f) * scale;
				string string2 = GetString(map, "parent", null);
				if (string2 != null)
				{
					meshAttachment.InheritDeform = GetBoolean(map, "deform", defaultValue: true);
					linkedMeshes.Add(new LinkedMesh(meshAttachment, GetString(map, "skin", null), slotIndex, string2));
					return meshAttachment;
				}
				float[] floatArray = GetFloatArray(map, "uvs", 1f);
				ReadVertices(map, meshAttachment, floatArray.Length);
				meshAttachment.triangles = GetIntArray(map, "triangles");
				meshAttachment.regionUVs = floatArray;
				meshAttachment.UpdateUVs();
				if (map.ContainsKey("hull"))
				{
					meshAttachment.HullLength = GetInt(map, "hull", 0) * 2;
				}
				if (map.ContainsKey("edges"))
				{
					meshAttachment.Edges = GetIntArray(map, "edges");
				}
				return meshAttachment;
			}
			case AttachmentType.Path:
			{
				PathAttachment pathAttachment = attachmentLoader.NewPathAttachment(skin, name);
				if (pathAttachment == null)
				{
					return null;
				}
				pathAttachment.closed = GetBoolean(map, "closed", defaultValue: false);
				pathAttachment.constantSpeed = GetBoolean(map, "constantSpeed", defaultValue: true);
				int @int = GetInt(map, "vertexCount", 0);
				ReadVertices(map, pathAttachment, @int << 1);
				pathAttachment.lengths = GetFloatArray(map, "lengths", scale);
				return pathAttachment;
			}
			default:
				return null;
			}
		}

		private void ReadVertices(Dictionary<string, object> map, VertexAttachment attachment, int verticesLength)
		{
			attachment.WorldVerticesLength = verticesLength;
			float[] floatArray = GetFloatArray(map, "vertices", 1f);
			float scale = Scale;
			if (verticesLength == floatArray.Length)
			{
				if (scale != 1f)
				{
					for (int i = 0; i < floatArray.Length; i++)
					{
						floatArray[i] *= scale;
					}
				}
				attachment.vertices = floatArray;
				return;
			}
			ExposedList<float> exposedList = new ExposedList<float>(verticesLength * 3 * 3);
			ExposedList<int> exposedList2 = new ExposedList<int>(verticesLength * 3);
			int j = 0;
			int num = floatArray.Length;
			while (j < num)
			{
				int num3 = (int)floatArray[j++];
				exposedList2.Add(num3);
				for (int num4 = j + num3 * 4; j < num4; j += 4)
				{
					exposedList2.Add((int)floatArray[j]);
					exposedList.Add(floatArray[j + 1] * Scale);
					exposedList.Add(floatArray[j + 2] * Scale);
					exposedList.Add(floatArray[j + 3]);
				}
			}
			attachment.bones = exposedList2.ToArray();
			attachment.vertices = exposedList.ToArray();
		}

		private void ReadAnimation(Dictionary<string, object> map, string name, SkeletonData skeletonData)
		{
			float scale = Scale;
			ExposedList<Timeline> exposedList = new ExposedList<Timeline>();
			float num = 0f;
			if (map.ContainsKey("slots"))
			{
				foreach (KeyValuePair<string, object> item3 in (Dictionary<string, object>)map["slots"])
				{
					string key = item3.Key;
					int slotIndex = skeletonData.FindSlotIndex(key);
					Dictionary<string, object> dictionary = (Dictionary<string, object>)item3.Value;
					foreach (KeyValuePair<string, object> item4 in dictionary)
					{
						List<object> list = (List<object>)item4.Value;
						string key2 = item4.Key;
						if (key2 == "color")
						{
							ColorTimeline colorTimeline = new ColorTimeline(list.Count);
							colorTimeline.slotIndex = slotIndex;
							int num2 = 0;
							foreach (Dictionary<string, object> item5 in list)
							{
								float time = (float)item5["time"];
								string hexString = (string)item5["color"];
								colorTimeline.SetFrame(num2, time, ToColor(hexString, 0), ToColor(hexString, 1), ToColor(hexString, 2), ToColor(hexString, 3));
								ReadCurve(item5, colorTimeline, num2);
								num2++;
							}
							exposedList.Add(colorTimeline);
							num = Math.Max(num, colorTimeline.frames[(colorTimeline.FrameCount - 1) * 5]);
						}
						else
						{
							if (!(key2 == "attachment"))
							{
								throw new Exception("Invalid timeline type for a slot: " + key2 + " (" + key + ")");
							}
							AttachmentTimeline attachmentTimeline = new AttachmentTimeline(list.Count);
							attachmentTimeline.slotIndex = slotIndex;
							int num3 = 0;
							foreach (Dictionary<string, object> item6 in list)
							{
								float time2 = (float)item6["time"];
								attachmentTimeline.SetFrame(num3++, time2, (string)item6["name"]);
							}
							exposedList.Add(attachmentTimeline);
							num = Math.Max(num, attachmentTimeline.frames[attachmentTimeline.FrameCount - 1]);
						}
					}
				}
			}
			if (map.ContainsKey("bones"))
			{
				foreach (KeyValuePair<string, object> item7 in (Dictionary<string, object>)map["bones"])
				{
					string key3 = item7.Key;
					int num5 = skeletonData.FindBoneIndex(key3);
					if (num5 == -1)
					{
						throw new Exception("Bone not found: " + key3);
					}
					Dictionary<string, object> dictionary4 = (Dictionary<string, object>)item7.Value;
					foreach (KeyValuePair<string, object> item8 in dictionary4)
					{
						List<object> list2 = (List<object>)item8.Value;
						string key4 = item8.Key;
						if (key4 == "rotate")
						{
							RotateTimeline rotateTimeline = new RotateTimeline(list2.Count);
							rotateTimeline.boneIndex = num5;
							int num6 = 0;
							foreach (Dictionary<string, object> item9 in list2)
							{
								rotateTimeline.SetFrame(num6, (float)item9["time"], (float)item9["angle"]);
								ReadCurve(item9, rotateTimeline, num6);
								num6++;
							}
							exposedList.Add(rotateTimeline);
							num = Math.Max(num, rotateTimeline.frames[(rotateTimeline.FrameCount - 1) * 2]);
						}
						else
						{
							if (!(key4 == "translate") && !(key4 == "scale") && !(key4 == "shear"))
							{
								throw new Exception("Invalid timeline type for a bone: " + key4 + " (" + key3 + ")");
							}
							float num7 = 1f;
							TranslateTimeline translateTimeline;
							if (key4 == "scale")
							{
								translateTimeline = new ScaleTimeline(list2.Count);
							}
							else if (key4 == "shear")
							{
								translateTimeline = new ShearTimeline(list2.Count);
							}
							else
							{
								translateTimeline = new TranslateTimeline(list2.Count);
								num7 = scale;
							}
							translateTimeline.boneIndex = num5;
							int num8 = 0;
							foreach (Dictionary<string, object> item10 in list2)
							{
								float time3 = (float)item10["time"];
								float @float = GetFloat(item10, "x", 0f);
								float float2 = GetFloat(item10, "y", 0f);
								translateTimeline.SetFrame(num8, time3, @float * num7, float2 * num7);
								ReadCurve(item10, translateTimeline, num8);
								num8++;
							}
							exposedList.Add(translateTimeline);
							num = Math.Max(num, translateTimeline.frames[(translateTimeline.FrameCount - 1) * 3]);
						}
					}
				}
			}
			if (map.ContainsKey("ik"))
			{
				foreach (KeyValuePair<string, object> item11 in (Dictionary<string, object>)map["ik"])
				{
					IkConstraintData item = skeletonData.FindIkConstraint(item11.Key);
					List<object> list3 = (List<object>)item11.Value;
					IkConstraintTimeline ikConstraintTimeline = new IkConstraintTimeline(list3.Count);
					ikConstraintTimeline.ikConstraintIndex = skeletonData.ikConstraints.IndexOf(item);
					int num9 = 0;
					foreach (Dictionary<string, object> item12 in list3)
					{
						float time4 = (float)item12["time"];
						float float3 = GetFloat(item12, "mix", 1f);
						bool boolean = GetBoolean(item12, "bendPositive", defaultValue: true);
						ikConstraintTimeline.SetFrame(num9, time4, float3, boolean ? 1 : (-1));
						ReadCurve(item12, ikConstraintTimeline, num9);
						num9++;
					}
					exposedList.Add(ikConstraintTimeline);
					num = Math.Max(num, ikConstraintTimeline.frames[(ikConstraintTimeline.FrameCount - 1) * 3]);
				}
			}
			if (map.ContainsKey("transform"))
			{
				foreach (KeyValuePair<string, object> item13 in (Dictionary<string, object>)map["transform"])
				{
					TransformConstraintData item2 = skeletonData.FindTransformConstraint(item13.Key);
					List<object> list4 = (List<object>)item13.Value;
					TransformConstraintTimeline transformConstraintTimeline = new TransformConstraintTimeline(list4.Count);
					transformConstraintTimeline.transformConstraintIndex = skeletonData.transformConstraints.IndexOf(item2);
					int num10 = 0;
					foreach (Dictionary<string, object> item14 in list4)
					{
						float time5 = (float)item14["time"];
						float float4 = GetFloat(item14, "rotateMix", 1f);
						float float5 = GetFloat(item14, "translateMix", 1f);
						float float6 = GetFloat(item14, "scaleMix", 1f);
						float float7 = GetFloat(item14, "shearMix", 1f);
						transformConstraintTimeline.SetFrame(num10, time5, float4, float5, float6, float7);
						ReadCurve(item14, transformConstraintTimeline, num10);
						num10++;
					}
					exposedList.Add(transformConstraintTimeline);
					num = Math.Max(num, transformConstraintTimeline.frames[(transformConstraintTimeline.FrameCount - 1) * 5]);
				}
			}
			if (map.ContainsKey("paths"))
			{
				foreach (KeyValuePair<string, object> item15 in (Dictionary<string, object>)map["paths"])
				{
					int num11 = skeletonData.FindPathConstraintIndex(item15.Key);
					if (num11 == -1)
					{
						throw new Exception("Path constraint not found: " + item15.Key);
					}
					PathConstraintData pathConstraintData = skeletonData.pathConstraints.Items[num11];
					Dictionary<string, object> dictionary9 = (Dictionary<string, object>)item15.Value;
					foreach (KeyValuePair<string, object> item16 in dictionary9)
					{
						List<object> list5 = (List<object>)item16.Value;
						string key5 = item16.Key;
						switch (key5)
						{
						case "position":
						case "spacing":
						{
							float num13 = 1f;
							PathConstraintPositionTimeline pathConstraintPositionTimeline;
							if (key5 == "spacing")
							{
								pathConstraintPositionTimeline = new PathConstraintSpacingTimeline(list5.Count);
								if (pathConstraintData.spacingMode == SpacingMode.Length || pathConstraintData.spacingMode == SpacingMode.Fixed)
								{
									num13 = scale;
								}
							}
							else
							{
								pathConstraintPositionTimeline = new PathConstraintPositionTimeline(list5.Count);
								if (pathConstraintData.positionMode == PositionMode.Fixed)
								{
									num13 = scale;
								}
							}
							pathConstraintPositionTimeline.pathConstraintIndex = num11;
							int num14 = 0;
							foreach (Dictionary<string, object> item17 in list5)
							{
								pathConstraintPositionTimeline.SetFrame(num14, (float)item17["time"], GetFloat(item17, key5, 0f) * num13);
								ReadCurve(item17, pathConstraintPositionTimeline, num14);
								num14++;
							}
							exposedList.Add(pathConstraintPositionTimeline);
							num = Math.Max(num, pathConstraintPositionTimeline.frames[(pathConstraintPositionTimeline.FrameCount - 1) * 2]);
							break;
						}
						case "mix":
						{
							PathConstraintMixTimeline pathConstraintMixTimeline = new PathConstraintMixTimeline(list5.Count);
							pathConstraintMixTimeline.pathConstraintIndex = num11;
							int num12 = 0;
							foreach (Dictionary<string, object> item18 in list5)
							{
								pathConstraintMixTimeline.SetFrame(num12, (float)item18["time"], GetFloat(item18, "rotateMix", 1f), GetFloat(item18, "translateMix", 1f));
								ReadCurve(item18, pathConstraintMixTimeline, num12);
								num12++;
							}
							exposedList.Add(pathConstraintMixTimeline);
							num = Math.Max(num, pathConstraintMixTimeline.frames[(pathConstraintMixTimeline.FrameCount - 1) * 3]);
							break;
						}
						}
					}
				}
			}
			if (map.ContainsKey("deform"))
			{
				foreach (KeyValuePair<string, object> item19 in (Dictionary<string, object>)map["deform"])
				{
					Skin skin = skeletonData.FindSkin(item19.Key);
					foreach (KeyValuePair<string, object> item20 in (Dictionary<string, object>)item19.Value)
					{
						int num15 = skeletonData.FindSlotIndex(item20.Key);
						if (num15 == -1)
						{
							throw new Exception("Slot not found: " + item20.Key);
						}
						foreach (KeyValuePair<string, object> item21 in (Dictionary<string, object>)item20.Value)
						{
							List<object> list6 = (List<object>)item21.Value;
							VertexAttachment vertexAttachment = (VertexAttachment)skin.GetAttachment(num15, item21.Key);
							if (vertexAttachment == null)
							{
								throw new Exception("Deform attachment not found: " + item21.Key);
							}
							bool flag = vertexAttachment.bones != null;
							float[] vertices = vertexAttachment.vertices;
							int num16 = (!flag) ? vertices.Length : (vertices.Length / 3 * 2);
							DeformTimeline deformTimeline = new DeformTimeline(list6.Count);
							deformTimeline.slotIndex = num15;
							deformTimeline.attachment = vertexAttachment;
							int num17 = 0;
							foreach (Dictionary<string, object> item22 in list6)
							{
								float[] array;
								if (!item22.ContainsKey("vertices"))
								{
									array = ((!flag) ? vertices : new float[num16]);
								}
								else
								{
									array = new float[num16];
									int @int = GetInt(item22, "offset", 0);
									float[] floatArray = GetFloatArray(item22, "vertices", 1f);
									Array.Copy(floatArray, 0, array, @int, floatArray.Length);
									if (scale != 1f)
									{
										int i = @int;
										for (int num18 = i + floatArray.Length; i < num18; i++)
										{
											array[i] *= scale;
										}
									}
									if (!flag)
									{
										for (int j = 0; j < num16; j++)
										{
											array[j] += vertices[j];
										}
									}
								}
								deformTimeline.SetFrame(num17, (float)item22["time"], array);
								ReadCurve(item22, deformTimeline, num17);
								num17++;
							}
							exposedList.Add(deformTimeline);
							num = Math.Max(num, deformTimeline.frames[deformTimeline.FrameCount - 1]);
						}
					}
				}
			}
			if (map.ContainsKey("drawOrder") || map.ContainsKey("draworder"))
			{
				List<object> list7 = (List<object>)map[(!map.ContainsKey("drawOrder")) ? "draworder" : "drawOrder"];
				DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(list7.Count);
				int count = skeletonData.slots.Count;
				int num19 = 0;
				foreach (Dictionary<string, object> item23 in list7)
				{
					int[] array2 = null;
					if (item23.ContainsKey("offsets"))
					{
						array2 = new int[count];
						for (int num20 = count - 1; num20 >= 0; num20--)
						{
							array2[num20] = -1;
						}
						List<object> list8 = (List<object>)item23["offsets"];
						int[] array3 = new int[count - list8.Count];
						int num21 = 0;
						int num22 = 0;
						foreach (Dictionary<string, object> item24 in list8)
						{
							int num23 = skeletonData.FindSlotIndex((string)item24["slot"]);
							if (num23 == -1)
							{
								throw new Exception("Slot not found: " + item24["slot"]);
							}
							while (num21 != num23)
							{
								array3[num22++] = num21++;
							}
							int num26 = num21 + (int)(float)item24["offset"];
							array2[num26] = num21++;
						}
						while (num21 < count)
						{
							array3[num22++] = num21++;
						}
						for (int num30 = count - 1; num30 >= 0; num30--)
						{
							if (array2[num30] == -1)
							{
								array2[num30] = array3[--num22];
							}
						}
					}
					drawOrderTimeline.SetFrame(num19++, (float)item23["time"], array2);
				}
				exposedList.Add(drawOrderTimeline);
				num = Math.Max(num, drawOrderTimeline.frames[drawOrderTimeline.FrameCount - 1]);
			}
			if (map.ContainsKey("events"))
			{
				List<object> list9 = (List<object>)map["events"];
				EventTimeline eventTimeline = new EventTimeline(list9.Count);
				int num32 = 0;
				foreach (Dictionary<string, object> item25 in list9)
				{
					EventData eventData = skeletonData.FindEvent((string)item25["name"]);
					if (eventData == null)
					{
						throw new Exception("Event not found: " + item25["name"]);
					}
					Event @event = new Event((float)item25["time"], eventData);
					@event.Int = GetInt(item25, "int", eventData.Int);
					@event.Float = GetFloat(item25, "float", eventData.Float);
					@event.String = GetString(item25, "string", eventData.String);
					eventTimeline.SetFrame(num32++, @event);
				}
				exposedList.Add(eventTimeline);
				num = Math.Max(num, eventTimeline.frames[eventTimeline.FrameCount - 1]);
			}
			exposedList.TrimExcess();
			skeletonData.animations.Add(new Animation(name, exposedList, num));
		}

		private static void ReadCurve(Dictionary<string, object> valueMap, CurveTimeline timeline, int frameIndex)
		{
			if (!valueMap.ContainsKey("curve"))
			{
				return;
			}
			object obj = valueMap["curve"];
			if (obj.Equals("stepped"))
			{
				timeline.SetStepped(frameIndex);
				return;
			}
			List<object> list = obj as List<object>;
			if (list != null)
			{
				timeline.SetCurve(frameIndex, (float)list[0], (float)list[1], (float)list[2], (float)list[3]);
			}
		}

		private static float[] GetFloatArray(Dictionary<string, object> map, string name, float scale)
		{
			List<object> list = (List<object>)map[name];
			float[] array = new float[list.Count];
			if (scale == 1f)
			{
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					array[i] = (float)list[i];
				}
			}
			else
			{
				int j = 0;
				for (int count2 = list.Count; j < count2; j++)
				{
					array[j] = (float)list[j] * scale;
				}
			}
			return array;
		}

		private static int[] GetIntArray(Dictionary<string, object> map, string name)
		{
			List<object> list = (List<object>)map[name];
			int[] array = new int[list.Count];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				array[i] = (int)(float)list[i];
			}
			return array;
		}

		private static float GetFloat(Dictionary<string, object> map, string name, float defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (float)map[name];
		}

		private static int GetInt(Dictionary<string, object> map, string name, int defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (int)(float)map[name];
		}

		private static bool GetBoolean(Dictionary<string, object> map, string name, bool defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (bool)map[name];
		}

		private static string GetString(Dictionary<string, object> map, string name, string defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (string)map[name];
		}

		private static float ToColor(string hexString, int colorIndex)
		{
			if (hexString.Length != 8)
			{
				throw new ArgumentException("Color hexidecimal length must be 8, recieved: " + hexString, "hexString");
			}
			return (float)Convert.ToInt32(hexString.Substring(colorIndex * 2, 2), 16) / 255f;
		}
	}
}
