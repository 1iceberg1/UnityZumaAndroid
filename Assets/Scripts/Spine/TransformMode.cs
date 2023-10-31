using System;

namespace Spine
{
	[Flags]
	public enum TransformMode
	{
		Normal = 0x0,
		OnlyTranslation = 0x7,
		NoRotationOrReflection = 0x1,
		NoScale = 0x2,
		NoScaleOrReflection = 0x6
	}
}
