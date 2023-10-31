namespace Spine.Unity
{
	public class SpineAnimation : SpineAttributeBase
	{
		public SpineAnimation(string startsWith = "", string dataField = "")
		{
			base.startsWith = startsWith;
			base.dataField = dataField;
		}
	}
}
