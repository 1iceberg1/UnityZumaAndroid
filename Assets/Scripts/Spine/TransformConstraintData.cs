using System;

namespace Spine
{
	public class TransformConstraintData
	{
		internal string name;

		internal int order;

		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();

		internal BoneData target;

		internal float rotateMix;

		internal float translateMix;

		internal float scaleMix;

		internal float shearMix;

		internal float offsetRotation;

		internal float offsetX;

		internal float offsetY;

		internal float offsetScaleX;

		internal float offsetScaleY;

		internal float offsetShearY;

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

		public BoneData Target
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

		public float ScaleMix
		{
			get
			{
				return scaleMix;
			}
			set
			{
				scaleMix = value;
			}
		}

		public float ShearMix
		{
			get
			{
				return shearMix;
			}
			set
			{
				shearMix = value;
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

		public float OffsetX
		{
			get
			{
				return offsetX;
			}
			set
			{
				offsetX = value;
			}
		}

		public float OffsetY
		{
			get
			{
				return offsetY;
			}
			set
			{
				offsetY = value;
			}
		}

		public float OffsetScaleX
		{
			get
			{
				return offsetScaleX;
			}
			set
			{
				offsetScaleX = value;
			}
		}

		public float OffsetScaleY
		{
			get
			{
				return offsetScaleY;
			}
			set
			{
				offsetScaleY = value;
			}
		}

		public float OffsetShearY
		{
			get
			{
				return offsetShearY;
			}
			set
			{
				offsetShearY = value;
			}
		}

		public TransformConstraintData(string name)
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
