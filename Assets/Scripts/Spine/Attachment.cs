using System;

namespace Spine
{
	public abstract class Attachment
	{
		public string Name
		{
			get;
			private set;
		}

		public Attachment(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null");
			}
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
