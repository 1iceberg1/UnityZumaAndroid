using System;

namespace Spine
{
	public class SkeletonData
	{
		internal string name;

		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();

		internal ExposedList<SlotData> slots = new ExposedList<SlotData>();

		internal ExposedList<Skin> skins = new ExposedList<Skin>();

		internal Skin defaultSkin;

		internal ExposedList<EventData> events = new ExposedList<EventData>();

		internal ExposedList<Animation> animations = new ExposedList<Animation>();

		internal ExposedList<IkConstraintData> ikConstraints = new ExposedList<IkConstraintData>();

		internal ExposedList<TransformConstraintData> transformConstraints = new ExposedList<TransformConstraintData>();

		internal ExposedList<PathConstraintData> pathConstraints = new ExposedList<PathConstraintData>();

		internal float width;

		internal float height;

		internal string version;

		internal string hash;

		internal float fps;

		internal string imagesPath;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public ExposedList<BoneData> Bones => bones;

		public ExposedList<SlotData> Slots => slots;

		public ExposedList<Skin> Skins
		{
			get
			{
				return skins;
			}
			set
			{
				skins = value;
			}
		}

		public Skin DefaultSkin
		{
			get
			{
				return defaultSkin;
			}
			set
			{
				defaultSkin = value;
			}
		}

		public ExposedList<EventData> Events
		{
			get
			{
				return events;
			}
			set
			{
				events = value;
			}
		}

		public ExposedList<Animation> Animations
		{
			get
			{
				return animations;
			}
			set
			{
				animations = value;
			}
		}

		public ExposedList<IkConstraintData> IkConstraints
		{
			get
			{
				return ikConstraints;
			}
			set
			{
				ikConstraints = value;
			}
		}

		public ExposedList<TransformConstraintData> TransformConstraints
		{
			get
			{
				return transformConstraints;
			}
			set
			{
				transformConstraints = value;
			}
		}

		public ExposedList<PathConstraintData> PathConstraints
		{
			get
			{
				return pathConstraints;
			}
			set
			{
				pathConstraints = value;
			}
		}

		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		public float Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public string Hash
		{
			get
			{
				return hash;
			}
			set
			{
				hash = value;
			}
		}

		public string ImagesPath
		{
			get
			{
				return imagesPath;
			}
			set
			{
				imagesPath = value;
			}
		}

		public float Fps
		{
			get
			{
				return fps;
			}
			set
			{
				fps = value;
			}
		}

		public BoneData FindBone(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName", "boneName cannot be null.");
			}
			ExposedList<BoneData> exposedList = bones;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				BoneData boneData = exposedList.Items[i];
				if (boneData.name == boneName)
				{
					return boneData;
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
			ExposedList<BoneData> exposedList = bones;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				if (exposedList.Items[i].name == boneName)
				{
					return i;
				}
			}
			return -1;
		}

		public SlotData FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			}
			ExposedList<SlotData> exposedList = slots;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				SlotData slotData = exposedList.Items[i];
				if (slotData.name == slotName)
				{
					return slotData;
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
			ExposedList<SlotData> exposedList = slots;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				if (exposedList.Items[i].name == slotName)
				{
					return i;
				}
			}
			return -1;
		}

		public Skin FindSkin(string skinName)
		{
			if (skinName == null)
			{
				throw new ArgumentNullException("skinName", "skinName cannot be null.");
			}
			foreach (Skin skin in skins)
			{
				if (skin.name == skinName)
				{
					return skin;
				}
			}
			return null;
		}

		public EventData FindEvent(string eventDataName)
		{
			if (eventDataName == null)
			{
				throw new ArgumentNullException("eventDataName", "eventDataName cannot be null.");
			}
			foreach (EventData @event in events)
			{
				if (@event.name == eventDataName)
				{
					return @event;
				}
			}
			return null;
		}

		public Animation FindAnimation(string animationName)
		{
			if (animationName == null)
			{
				throw new ArgumentNullException("animationName", "animationName cannot be null.");
			}
			ExposedList<Animation> exposedList = animations;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Animation animation = exposedList.Items[i];
				if (animation.name == animationName)
				{
					return animation;
				}
			}
			return null;
		}

		public IkConstraintData FindIkConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<IkConstraintData> exposedList = ikConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				IkConstraintData ikConstraintData = exposedList.Items[i];
				if (ikConstraintData.name == constraintName)
				{
					return ikConstraintData;
				}
			}
			return null;
		}

		public TransformConstraintData FindTransformConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<TransformConstraintData> exposedList = transformConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				TransformConstraintData transformConstraintData = exposedList.Items[i];
				if (transformConstraintData.name == constraintName)
				{
					return transformConstraintData;
				}
			}
			return null;
		}

		public PathConstraintData FindPathConstraint(string constraintName)
		{
			if (constraintName == null)
			{
				throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
			}
			ExposedList<PathConstraintData> exposedList = pathConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				PathConstraintData pathConstraintData = exposedList.Items[i];
				if (pathConstraintData.name.Equals(constraintName))
				{
					return pathConstraintData;
				}
			}
			return null;
		}

		public int FindPathConstraintIndex(string pathConstraintName)
		{
			if (pathConstraintName == null)
			{
				throw new ArgumentNullException("pathConstraintName", "pathConstraintName cannot be null.");
			}
			ExposedList<PathConstraintData> exposedList = pathConstraints;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				if (exposedList.Items[i].name.Equals(pathConstraintName))
				{
					return i;
				}
			}
			return -1;
		}

		public override string ToString()
		{
			return name ?? base.ToString();
		}
	}
}
