using UnityEngine;

namespace Spine.Unity
{
	public class SpineAtlasRegion : PropertyAttribute
	{
		public string atlasAssetField;

		public SpineAtlasRegion(string atlasAssetField = "")
		{
			this.atlasAssetField = atlasAssetField;
		}
	}
}
