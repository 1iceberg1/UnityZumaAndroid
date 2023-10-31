namespace Spine
{
	public class MeshAttachment : VertexAttachment
	{
		internal float regionOffsetX;

		internal float regionOffsetY;

		internal float regionWidth;

		internal float regionHeight;

		internal float regionOriginalWidth;

		internal float regionOriginalHeight;

		internal float[] uvs;

		internal float[] regionUVs;

		internal int[] triangles;

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

		internal int hulllength;

		internal MeshAttachment parentMesh;

		internal bool inheritDeform;

		public int HullLength
		{
			get
			{
				return hulllength;
			}
			set
			{
				hulllength = value;
			}
		}

		public float[] RegionUVs
		{
			get
			{
				return regionUVs;
			}
			set
			{
				regionUVs = value;
			}
		}

		public float[] UVs
		{
			get
			{
				return uvs;
			}
			set
			{
				uvs = value;
			}
		}

		public int[] Triangles
		{
			get
			{
				return triangles;
			}
			set
			{
				triangles = value;
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

		public string Path
		{
			get;
			set;
		}

		public object RendererObject
		{
			get;
			set;
		}

		public float RegionU
		{
			get;
			set;
		}

		public float RegionV
		{
			get;
			set;
		}

		public float RegionU2
		{
			get;
			set;
		}

		public float RegionV2
		{
			get;
			set;
		}

		public bool RegionRotate
		{
			get;
			set;
		}

		public float RegionOffsetX
		{
			get
			{
				return regionOffsetX;
			}
			set
			{
				regionOffsetX = value;
			}
		}

		public float RegionOffsetY
		{
			get
			{
				return regionOffsetY;
			}
			set
			{
				regionOffsetY = value;
			}
		}

		public float RegionWidth
		{
			get
			{
				return regionWidth;
			}
			set
			{
				regionWidth = value;
			}
		}

		public float RegionHeight
		{
			get
			{
				return regionHeight;
			}
			set
			{
				regionHeight = value;
			}
		}

		public float RegionOriginalWidth
		{
			get
			{
				return regionOriginalWidth;
			}
			set
			{
				regionOriginalWidth = value;
			}
		}

		public float RegionOriginalHeight
		{
			get
			{
				return regionOriginalHeight;
			}
			set
			{
				regionOriginalHeight = value;
			}
		}

		public bool InheritDeform
		{
			get
			{
				return inheritDeform;
			}
			set
			{
				inheritDeform = value;
			}
		}

		public MeshAttachment ParentMesh
		{
			get
			{
				return parentMesh;
			}
			set
			{
				parentMesh = value;
				if (value != null)
				{
					bones = value.bones;
					vertices = value.vertices;
					worldVerticesLength = value.worldVerticesLength;
					regionUVs = value.regionUVs;
					triangles = value.triangles;
					HullLength = value.HullLength;
					Edges = value.Edges;
					Width = value.Width;
					Height = value.Height;
				}
			}
		}

		public int[] Edges
		{
			get;
			set;
		}

		public float Width
		{
			get;
			set;
		}

		public float Height
		{
			get;
			set;
		}

		public MeshAttachment(string name)
			: base(name)
		{
		}

		public void UpdateUVs()
		{
			float regionU = RegionU;
			float regionV = RegionV;
			float num = RegionU2 - RegionU;
			float num2 = RegionV2 - RegionV;
			float[] array = regionUVs;
			if (uvs == null || uvs.Length != array.Length)
			{
				uvs = new float[array.Length];
			}
			float[] array2 = uvs;
			if (RegionRotate)
			{
				int i = 0;
				for (int num3 = array2.Length; i < num3; i += 2)
				{
					array2[i] = regionU + array[i + 1] * num;
					array2[i + 1] = regionV + num2 - array[i] * num2;
				}
			}
			else
			{
				int j = 0;
				for (int num4 = array2.Length; j < num4; j += 2)
				{
					array2[j] = regionU + array[j] * num;
					array2[j + 1] = regionV + array[j + 1] * num2;
				}
			}
		}

		public override bool ApplyDeform(VertexAttachment sourceAttachment)
		{
			return this == sourceAttachment || (inheritDeform && parentMesh == sourceAttachment);
		}
	}
}
