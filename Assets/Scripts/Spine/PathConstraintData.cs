using System;

namespace Spine
{
	public class PathConstraintData
	{
		internal string name;

		internal int order;

		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();

		internal SlotData target;

		internal PositionMode positionMode;

		internal SpacingMode spacingMode;

		internal RotateMode rotateMode;

		internal float offsetRotation;

		internal float position;

		internal float spacing;

		internal float rotateMix;

		internal float translateMix;

		public string Name => name;

		public int Order
		{
			get
			{
				return order;
			}
			set
			{
				order = value;
			}
		}

		public ExposedList<BoneData> Bones => bones;

		public SlotData Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public PositionMode PositionMode
		{
			get
			{
				return positionMode;
			}
			set
			{
				positionMode = value;
			}
		}

		public SpacingMode SpacingMode
		{
			get
			{
				return spacingMode;
			}
			set
			{
				spacingMode = value;
			}
		}

		public RotateMode RotateMode
		{
			get
			{
				return rotateMode;
			}
			set
			{
				rotateMode = value;
			}
		}

		public float OffsetRotation
		{
			get
			{
				return offsetRotation;
			}
			set
			{
				offsetRotation = value;
			}
		}

		public float Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		public float Spacing
		{
			get
			{
				return spacing;
			}
			set
			{
				spacing = value;
			}
		}

		public float RotateMix
		{
			get
			{
				return rotateMix;
			}
			set
			{
				rotateMix = value;
			}
		}

		public float TranslateMix
		{
			get
			{
				return translateMix;
			}
			set
			{
				translateMix = value;
			}
		}

		public PathConstraintData(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}
	}
}
