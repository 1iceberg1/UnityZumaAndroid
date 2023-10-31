using System;

namespace Spine
{
	public class TransformConstraint : IConstraint, IUpdatable
	{
		internal TransformConstraintData data;

		internal ExposedList<Bone> bones;

		internal Bone target;

		internal float rotateMix;

		internal float translateMix;

		internal float scaleMix;

		internal float shearMix;

		public TransformConstraintData Data => data;

		public int Order => data.order;

		public ExposedList<Bone> Bones => bones;

		public Bone Target
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

		public TransformConstraint(TransformConstraintData data, Skeleton skeleton)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			this.data = data;
			rotateMix = data.rotateMix;
			translateMix = data.translateMix;
			scaleMix = data.scaleMix;
			shearMix = data.shearMix;
			bones = new ExposedList<Bone>();
			foreach (BoneData bone in data.bones)
			{
				bones.Add(skeleton.FindBone(bone.name));
			}
			target = skeleton.FindBone(data.target.name);
		}

		public void Apply()
		{
			Update();
		}

		public void Update()
		{
			float num = rotateMix;
			float num2 = translateMix;
			float num3 = scaleMix;
			float num4 = shearMix;
			Bone bone = target;
			float a = bone.a;
			float b = bone.b;
			float c = bone.c;
			float d = bone.d;
			float num5 = (!(a * d - b * c > 0f)) ? (-(float)Math.PI / 180f) : ((float)Math.PI / 180f);
			float num6 = data.offsetRotation * num5;
			float num7 = data.offsetShearY * num5;
			ExposedList<Bone> exposedList = bones;
			Bone[] items = exposedList.Items;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Bone bone2 = items[i];
				bool flag = false;
				if (num != 0f)
				{
					float a2 = bone2.a;
					float b2 = bone2.b;
					float c2 = bone2.c;
					float d2 = bone2.d;
					float num8 = MathUtils.Atan2(c, a) - MathUtils.Atan2(c2, a2) + num6;
					if (num8 > (float)Math.PI)
					{
						num8 -= (float)Math.PI * 2f;
					}
					else if (num8 < -(float)Math.PI)
					{
						num8 += (float)Math.PI * 2f;
					}
					num8 *= num;
					float num9 = MathUtils.Cos(num8);
					float num10 = MathUtils.Sin(num8);
					bone2.a = num9 * a2 - num10 * c2;
					bone2.b = num9 * b2 - num10 * d2;
					bone2.c = num10 * a2 + num9 * c2;
					bone2.d = num10 * b2 + num9 * d2;
					flag = true;
				}
				if (num2 != 0f)
				{
					bone.LocalToWorld(data.offsetX, data.offsetY, out float worldX, out float worldY);
					bone2.worldX += (worldX - bone2.worldX) * num2;
					bone2.worldY += (worldY - bone2.worldY) * num2;
					flag = true;
				}
				if (num3 > 0f)
				{
					float num11 = (float)Math.Sqrt(bone2.a * bone2.a + bone2.c * bone2.c);
					float num12 = (float)Math.Sqrt(a * a + c * c);
					if (num11 > 1E-05f)
					{
						num11 = (num11 + (num12 - num11 + data.offsetScaleX) * num3) / num11;
					}
					bone2.a *= num11;
					bone2.c *= num11;
					num11 = (float)Math.Sqrt(bone2.b * bone2.b + bone2.d * bone2.d);
					num12 = (float)Math.Sqrt(b * b + d * d);
					if (num11 > 1E-05f)
					{
						num11 = (num11 + (num12 - num11 + data.offsetScaleY) * num3) / num11;
					}
					bone2.b *= num11;
					bone2.d *= num11;
					flag = true;
				}
				if (num4 > 0f)
				{
					float b3 = bone2.b;
					float d3 = bone2.d;
					float num13 = MathUtils.Atan2(d3, b3);
					float num14 = MathUtils.Atan2(d, b) - MathUtils.Atan2(c, a) - (num13 - MathUtils.Atan2(bone2.c, bone2.a));
					if (num14 > (float)Math.PI)
					{
						num14 -= (float)Math.PI * 2f;
					}
					else if (num14 < -(float)Math.PI)
					{
						num14 += (float)Math.PI * 2f;
					}
					num14 = num13 + (num14 + num7) * num4;
					float num15 = (float)Math.Sqrt(b3 * b3 + d3 * d3);
					bone2.b = MathUtils.Cos(num14) * num15;
					bone2.d = MathUtils.Sin(num14) * num15;
					flag = true;
				}
				if (flag)
				{
					bone2.appliedValid = false;
				}
			}
		}

		public override string ToString()
		{
			return data.name;
		}
	}
}
