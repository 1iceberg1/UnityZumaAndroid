namespace Spine
{
	public class PathAttachment : VertexAttachment
	{
		internal float[] lengths;

		internal bool closed;

		internal bool constantSpeed;

		public float[] Lengths
		{
			get
			{
				return lengths;
			}
			set
			{
				lengths = value;
			}
		}

		public bool Closed
		{
			get
			{
				return closed;
			}
			set
			{
				closed = value;
			}
		}

		public bool ConstantSpeed
		{
			get
			{
				return constantSpeed;
			}
			set
			{
				constantSpeed = value;
			}
		}

		public PathAttachment(string name)
			: base(name)
		{
		}
	}
}
