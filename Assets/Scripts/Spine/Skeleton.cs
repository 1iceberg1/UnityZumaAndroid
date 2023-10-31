using System;
using System.Collections.Generic;

namespace Spine
{
	public class Skeleton
	{
		internal SkeletonData data;

		internal ExposedList<Bone> bones;

		internal ExposedList<Slot> slots;

		internal ExposedList<Slot> drawOrder;

		internal ExposedList<IkConstraint> ikConstraints;

		internal ExposedList<TransformConstraint> transformConstraints;

		internal ExposedList<PathConstraint> pathConstraints;

		internal ExposedList<IUpdatable> updateCache = new ExposedList<IUpdatable>();

		internal ExposedList<Bone> updateCacheReset = new ExposedList<Bone>();

		internal Skin skin;

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

		internal float time;

		internal bool flipX;

		internal bool flipY;

		internal float x;

		internal float y;

		public SkeletonData Data => data;

		public ExposedList<Bone> Bones => bones;

		public ExposedList<IUpdatable> UpdateCacheList => updateCache;

		public ExposedList<Slot> Slots => slots;

		public ExposedList<Slot> DrawOrder => drawOrder;

		public ExposedList<IkConstraint> IkConstraints => ikConstraints;

		public ExposedList<PathConstraint> PathConstraints => pathConstraints;

		public ExposedList<TransformConstraint> TransformConstraints => transformConstraints;

		public Skin Skin
		{
			get
			{
				return skin;
			}
			set
			{
				skin = value;
			}
		}

		public float R
		{
			get
			{
				return r;
			}
			set
			{
				r = value;
			}
		}

		public float G
		{
			get
			{
				return g;
			}
			set
			{
				g = value;
			}
		}

		public float B
		{
			get
			{
				return b;
			}
			set
			{
				b = value;
			}
		}

		public float A
		{
			get
			{
				return a;
			}
			set
			{
				a = value;
			}
		}

		public float Time
		{
			get
			{
				return time;
			}
			set
			{
				time = value;
			}
		}

		public float X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		public float Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		public bool FlipX
		{
			get
			{
				return flipX;
			}
			set
			{
				flipX = value;
			}
		}

		public bool FlipY
		{
			get
			{
				return flipY;
			}
			set
			{
				flipY = value;
			}
		}

		public Bone RootBone => (bones.Count != 0) ? bones.Items[0] : null;

		public Skeleton(SkeletonData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}
			this.data = data;
			bones = new ExposedList<Bone>(data.bones.Count);
			foreach (BoneData bone3 in data.bones)
			{
				Bone item;
				if (bone3.parent == null)
				{
					item = new Bone(bone3, this, null);
				}
				else
				{
					Bone bone = bones.Items[bone3.parent.index];
					item = new Bone(bone3, this, bone);
					bone.children.Add(item);
				}
				bones.Add(item);
			}
			slots = new ExposedList<Slot>(data.slots.Count);
			drawOrder = new ExposedList<Slot>(data.slots.Count);
			foreach (SlotData slot in data.slots)
			{
				Bone bone2 = bones.Items[slot.boneData.index];
				Slot item2 = new Slot(slot, bone2);
				slots.Add(item2);
				drawOrder.Add(item2);
			}
			ikConstraints = new ExposedList<IkConstraint>(data.ikConstraints.Count);
			foreach (IkConstraintData ikConstraint in data.ikConstraints)
			{
				ikConstraints.Add(new IkConstraint(ikConstraint, this));
			}
			transformConstraints = new ExposedList<TransformConstraint>(data.transformConstraints.Count);
			foreach (TransformConstraintData transformConstraint in data.transformConstraints)
			{
				transformConstraints.Add(new TransformConstraint(transformConstraint, this));
			}
			pathConstraints = new ExposedList<PathConstraint>(data.pathConstraints.Count);
			foreach (PathConstraintData pathConstraint in data.pathConstraints)
			{
				pathConstraints.Add(new PathConstraint(pathConstraint, this));
			}
			UpdateCache();
			UpdateWorldTransform();
		}

		public void UpdateCache()
		{
			ExposedList<IUpdatable> exposedList = updateCache;
			exposedList.Clear();
			updateCacheReset.Clear();
			ExposedList<Bone> exposedList2 = bones;
			int i = 0;
			for (int count = exposedList2.Count; i < count; i++)
			{
				exposedList2.Items[i].sorted = false;
			}
			ExposedList<IkConstraint> exposedList3 = ikConstraints;
			ExposedList<TransformConstraint> exposedList4 = transformConstraints;
			ExposedList<PathConstraint> exposedList5 = pathConstraints;
			int count2 = IkConstraints.Count;
			int count3 = exposedList4.Count;
			int count4 = exposedList5.Count;
			int num = count2 + count3 + count4;
			for (int j = 0; j < num; j++)
			{
				int num2 = 0;
				while (true)
				{
					if (num2 < count2)
					{
						IkConstraint ikConstraint = exposedList3.Items[num2];
						if (ikConstraint.data.order == j)
						{
							SortIkConstraint(ikConstraint);
							break;
						}
						num2++;
						continue;
					}
					int num3 = 0;
					while (true)
					{
						if (num3 < count3)
						{
							TransformConstraint transformConstraint = exposedList4.Items[num3];
							if (transformConstraint.data.order == j)
							{
								SortTransformConstraint(transformConstraint);
								break;
							}
							num3++;
							continue;
						}
						for (int k = 0; k < count4; k++)
						{
							PathConstraint pathConstraint = exposedList5.Items[k];
							if (pathConstraint.data.order == j)
							{
								SortPathConstraint(pathConstraint);
								break;
							}
						}
						break;
					}
					break;
				}
			}
			int l = 0;
			for (int count5 = exposedList2.Count; l < count5; l++)
			{
				SortBone(exposedList2.Items[l]);
			}
		}

		private void SortIkConstraint(IkConstraint constraint)
		{
			Bone target = constraint.target;
			SortBone(target);
			ExposedList<Bone> exposedList = constraint.bones;
			Bone bone = exposedList.Items[0];
			SortBone(bone);
			if (exposedList.Count > 1)
			{
				Bone item = exposedList.Items[exposedList.Count - 1];
				if (!updateCache.Contains(item))
				{
					updateCacheReset.Add(item);
				}
			}
			updateCache.Add(constraint);
			SortReset(bone.children);
			exposedList.Items[exposedList.Count - 1].sorted = true;
		}

		private void SortPathConstraint(PathConstraint constraint)
		{
			Slot target = constraint.target;
			int index = target.data.index;
			Bone bone = target.bone;
			if (skin != null)
			{
				SortPathConstraintAttachment(skin, index, bone);
			}
			if (data.defaultSkin != null && data.defaultSkin != skin)
			{
				SortPathConstraintAttachment(data.defaultSkin, index, bone);
			}
			int i = 0;
			for (int count = data.skins.Count; i < count; i++)
			{
				SortPathConstraintAttachment(data.skins.Items[i], index, bone);
			}
			Attachment attachment = target.attachment;
			if (attachment is PathAttachment)
			{
				SortPathConstraintAttachment(attachment, bone);
			}
			ExposedList<Bone> exposedList = constraint.bones;
			int count2 = exposedList.Count;
			for (int j = 0; j < count2; j++)
			{
				SortBone(exposedList.Items[j]);
			}
			updateCache.Add(constraint);
			for (int k = 0; k < count2; k++)
			{
				SortReset(exposedList.Items[k].children);
			}
			for (int l = 0; l < count2; l++)
			{
				exposedList.Items[l].sorted = true;
			}
		}

		private void SortTransformConstraint(TransformConstraint constraint)
		{
			SortBone(constraint.target);
			ExposedList<Bone> exposedList = constraint.bones;
			int count = exposedList.Count;
			for (int i = 0; i < count; i++)
			{
				SortBone(exposedList.Items[i]);
			}
			updateCache.Add(constraint);
			for (int j = 0; j < count; j++)
			{
				SortReset(exposedList.Items[j].children);
			}
			for (int k = 0; k < count; k++)
			{
				exposedList.Items[k].sorted = true;
			}
		}

		private void SortPathConstraintAttachment(Skin skin, int slotIndex, Bone slotBone)
		{
			foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> attachment in skin.Attachments)
			{
				Skin.AttachmentKeyTuple key = attachment.Key;
				if (key.slotIndex == slotIndex)
				{
					SortPathConstraintAttachment(attachment.Value, slotBone);
				}
			}
		}

		private void SortPathConstraintAttachment(Attachment attachment, Bone slotBone)
		{
			if (!(attachment is PathAttachment))
			{
				return;
			}
			int[] array = ((PathAttachment)attachment).bones;
			if (array == null)
			{
				SortBone(slotBone);
				return;
			}
			ExposedList<Bone> exposedList = bones;
			int num = 0;
			int num2 = array.Length;
			while (num < num2)
			{
				int num4 = array[num++];
				num4 += num;
				while (num < num4)
				{
					SortBone(exposedList.Items[array[num++]]);
				}
			}
		}

		private void SortBone(Bone bone)
		{
			if (!bone.sorted)
			{
				Bone parent = bone.parent;
				if (parent != null)
				{
					SortBone(parent);
				}
				bone.sorted = true;
				updateCache.Add(bone);
			}
		}

		private void SortReset(ExposedList<Bone> bones)
		{
			Bone[] items = bones.Items;
			int i = 0;
			for (int count = bones.Count; i < count; i++)
			{
				Bone bone = items[i];
				if (bone.sorted)
				{
					SortReset(bone.children);
				}
				bone.sorted = false;
			}
		}

		public void UpdateWorldTransform()
		{
			ExposedList<Bone> exposedList = updateCacheReset;
			Bone[] items = exposedList.Items;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Bone bone = items[i];
				bone.ax = bone.x;
				bone.ay = bone.y;
				bone.arotation = bone.rotation;
				bone.ascaleX = bone.scaleX;
				bone.ascaleY = bone.scaleY;
				bone.ashearX = bone.shearX;
				bone.ashearY = bone.shearY;
				bone.appliedValid = true;
			}
			IUpdatable[] items2 = updateCache.Items;
			int j = 0;
			for (int count2 = updateCache.Count; j < count2; j++)
			{
				items2[j].Update();
			}
		}

		public void SetToSetupPose()
		{
			SetBonesToSetupPose();
			SetSlotsToSetupPose();
		}

		public void SetBonesToSetupPose()
		{
			Bone[] items = bones.Items;
			int i = 0;
			for (int count = bones.Count; i < count; i++)
			{
				items[i].SetToSetupPose();
			}
			IkConstraint[] items2 = ikConstraints.Items;
			int j = 0;
			for (int count2 = ikConstraints.Count; j < count2; j++)
			{
				IkConstraint ikConstraint = items2[j];
				ikConstraint.bendDirection = ikConstraint.data.bendDirection;
				ikConstraint.mix = ikConstraint.data.mix;
			}
			TransformConstraint[] items3 = transformConstraints.Items;
			int k = 0;
			for (int count3 = transformConstraints.Count; k < count3; k++)
			{
				TransformConstraint transformConstraint = items3[k];
				TransformConstraintData transformConstraintData = transformConstraint.data;
				transformConstraint.rotateMix = transformConstraintData.rotateMix;
				transformConstraint.translateMix = transformConstraintData.translateMix;
				transformConstraint.scaleMix = transformConstraintData.scaleMix;
				transformConstraint.shearMix = transformConstraintData.shearMix;
			}
			PathConstraint[] items4 = pathConstraints.Items;
			int l = 0;
			for (int count4 = pathConstraints.Count; l < count4; l++)
			{
				PathConstraint pathConstraint = items4[l];
				PathConstraintData pathConstraintData = pathConstraint.data;
				pathConstraint.position = pathConstraintData.position;
				pathConstraint.spacing = pathConstraintData.spacing;
				pathConstraint.rotateMix = pathConstraintData.rotateMix;
				pathConstraint.translateMix = pathConstraintData.translateMix;
			}
		}

		public void SetSlotsToSetupPose()
		{
			ExposedList<Slot> exposedList = slots;
			Slot[] items = exposedList.Items;
			drawOrder.Clear();
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				drawOrder.Add(items[i]);
			}
			int j = 0;
			for (int count2 = exposedList.Count; j < count2; j++)
			{
				items[j].SetToSetupPose();
			}
		}

		public Bone FindBone(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName", "boneName cannot be null.");
			}
			ExposedList<Bone> exposedList = bones;
			Bone[] items = exposedList.Items;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Bone bone = items[i];
				if (bone.data.name == boneName)
				{
					return bone;
				}
			}
			return null;
		}

		public int FindBoneIndex(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName", "boneName cannot be null.");
			}
			ExposedList<Bone> exposedList = bones;
			Bone[] items = exposedList.Items;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				if (items[i].data.name == boneName)
				{
					return i;
				}
			}
			return -1;
		}

		public Slot FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			}
			ExposedList<Slot> exposedList = slots;
			Slot[] items = exposedList.Items;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Slot slot = items[i];
				if (slot.data.name == slotName)
				{
					return slot;
				}
			}
			return null;
		}

		public int FindSlotIndex(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			}
			ExposedList<Slot> exposedList = slots;
			Slot[] items = exposedList.Items;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				if (items[i].data.name.Equals(slotName))
				{
					return i;
				}
			}
			return -1;
		}

		public void SetSkin(string skinName)
		{
			Skin skin = data.FindSkin(skinName);
			if (skin == null)
			{
				throw new ArgumentException("Skin not found: " + skinName, "skinName");
			}
			SetSkin(skin);
		}

		public void SetSkin(Skin newSkin)
		{
			if (newSkin != null)
			{
				if (skin != null)
				{
					newSkin.AttachAll(this, skin);
				}
				else
				{
					ExposedList<Slot> exposedList = slots;
					int i = 0;
					for (int count = exposedList.Count; i < count; i++)
					{
						Slot slot = exposedList.Items[i];
						string attachmentName = slot.data.attachmentName;
						if (attachmentName != null)
						{
							Attachment attachment = newSkin.GetAttachment(i, attachmentName);
							if (attachment != null)
							{
								slot.Attachment = attachment;
							}
						}
					}
				}
			}
			skin = newSkin;
		}

		public Attachment GetAttachment(string slotName, string attachmentName)
		{
			return GetAttachment(data.FindSlotIndex(slotName), attachmentName);
		}

		public Attachment GetAttachment(int slotIndex, string attachmentName)
		{
			if (attachmentName == null)
			{
				throw new ArgumentNullException("attachmentName", "attachmentName cannot be null.");
			}
			if (skin != null)
			{
				Attachment attachment = skin.GetAttachment(slotIndex, attachmentName);
				if (attachment != null)
				{
					return attachment;
				}
			}
			if (data.defaultSkin != null)
			{
				return data.defaultSkin.GetAttachment(slotIndex, attachmentName);
			}
			return null;
		}

		public void SetAttachment(string slotName, string attachmentName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			}
			ExposedList<Slot> exposedList = slots;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Slot slot = exposedList.Items[i];
				if (!(slot.data.name == slotName))
				{
					continue;
				}
				Attachment attachment = null;
				if (attachmentName != null)
				{
					attachment = GetAttachment(i, attachmentName);
					if (attachment == null)
					{
						throw new Exception("Attachment not found: " + attachmentName + ", for slot: " + slotName);
					}
				}
				slot.Attachment = attachment;
				return;
			}
			throw new Exception("Slot not found: " + slotName);
		}

		public IkConstraint FindIkConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<IkConstraint> exposedList = ikConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				IkConstraint ikConstraint = exposedList.Items[i];
				if (ikConstraint.data.name == constraintName)
				{
					return ikConstraint;
				}
			}
			return null;
		}

		public TransformConstraint FindTransformConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<TransformConstraint> exposedList = transformConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				TransformConstraint transformConstraint = exposedList.Items[i];
				if (transformConstraint.data.name == constraintName)
				{
					return transformConstraint;
				}
			}
			return null;
		}

		public PathConstraint FindPathConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<PathConstraint> exposedList = pathConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				PathConstraint pathConstraint = exposedList.Items[i];
				if (pathConstraint.data.name.Equals(constraintName))
				{
					return pathConstraint;
				}
			}
			return null;
		}

		public void Update(float delta)
		{
			time += delta;
		}
	}
}
