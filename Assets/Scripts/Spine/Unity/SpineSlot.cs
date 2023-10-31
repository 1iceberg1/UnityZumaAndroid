namespace Spine.Unity
{
	public class SpineSlot : SpineAttributeBase
	{
		public bool containsBoundingBoxes;

		public SpineSlot(string startsWith = "", string dataField = "", bool containsBoundingBoxes = false)
		{
			base.startsWith = startsWith;
			base.dataField = dataField;
			this.containsBoundingBoxes = containsBoundingBoxes;
		}
	}
}
