namespace Spine
{
	public class VertexAttachment : Attachment
	{
		internal int[] bones;

		internal float[] vertices;

		internal int worldVerticesLength;

		public int[] Bones
		{
			get
			{
				return bones;
			}
			set
			{
				bones = value;
			}
		}

		public float[] Vertices
		{
			get
			{
				return vertices;
			}
			set
			{
				vertices = value;
			}
		}

		public int WorldVerticesLength
		{
			get
			{
				return worldVerticesLength;
			}
			set
			{
				worldVerticesLength = value;
			}
		}

		public VertexAttachment(string name)
			: base(name)
		{
		}

		public void ComputeWorldVertices(Slot slot, float[] worldVertices)
		{
			ComputeWorldVertices(slot, 0, worldVerticesLength, worldVertices, 0);
		}

		public void ComputeWorldVertices(Slot slot, int start, int count, float[] worldVertices, int offset)
		{
			count += offset;
			Skeleton skeleton = slot.Skeleton;
			ExposedList<float> attachmentVertices = slot.attachmentVertices;
			float[] items = vertices;
			int[] array = bones;
			if (array == null)
			{
				if (attachmentVertices.Count > 0)
				{
					items = attachmentVertices.Items;
				}
				Bone bone = slot.bone;
				float worldX = bone.worldX;
				float worldY = bone.worldY;
				float a = bone.a;
				float b = bone.b;
				float c = bone.c;
				float d = bone.d;
				int num = start;
				for (int i = offset; i < count; i += 2)
				{
					float num2 = items[num];
					float num3 = items[num + 1];
					worldVertices[i] = num2 * a + num3 * b + worldX;
					worldVertices[i + 1] = num2 * c + num3 * d + worldY;
					num += 2;
				}
				return;
			}
			int num4 = 0;
			int num5 = 0;
			for (int j = 0; j < start; j += 2)
			{
				int num6 = array[num4];
				num4 += num6 + 1;
				num5 += num6;
			}
			Bone[] items2 = skeleton.Bones.Items;
			if (attachmentVertices.Count == 0)
			{
				int k = offset;
				int num7 = num5 * 3;
				for (; k < count; k += 2)
				{
					float num8 = 0f;
					float num9 = 0f;
					int num11 = array[num4++];
					num11 += num4;
					while (num4 < num11)
					{
						Bone bone2 = items2[array[num4]];
						float num12 = items[num7];
						float num13 = items[num7 + 1];
						float num14 = items[num7 + 2];
						num8 += (num12 * bone2.a + num13 * bone2.b + bone2.worldX) * num14;
						num9 += (num12 * bone2.c + num13 * bone2.d + bone2.worldY) * num14;
						num4++;
						num7 += 3;
					}
					worldVertices[k] = num8;
					worldVertices[k + 1] = num9;
				}
				return;
			}
			float[] items3 = attachmentVertices.Items;
			int l = offset;
			int num15 = num5 * 3;
			int num16 = num5 << 1;
			for (; l < count; l += 2)
			{
				float num17 = 0f;
				float num18 = 0f;
				int num20 = array[num4++];
				num20 += num4;
				while (num4 < num20)
				{
					Bone bone3 = items2[array[num4]];
					float num21 = items[num15] + items3[num16];
					float num22 = items[num15 + 1] + items3[num16 + 1];
					float num23 = items[num15 + 2];
					num17 += (num21 * bone3.a + num22 * bone3.b + bone3.worldX) * num23;
					num18 += (num21 * bone3.c + num22 * bone3.d + bone3.worldY) * num23;
					num4++;
					num15 += 3;
					num16 += 2;
				}
				worldVertices[l] = num17;
				worldVertices[l + 1] = num18;
			}
		}

		public virtual bool ApplyDeform(VertexAttachment sourceAttachment)
		{
			return this == sourceAttachment;
		}
	}
}
