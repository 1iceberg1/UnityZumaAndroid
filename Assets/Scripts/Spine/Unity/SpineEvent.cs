namespace Spine.Unity
{
	public class SpineEvent : SpineAttributeBase
	{
		public SpineEvent(string startsWith = "", string dataField = "")
		{
			base.startsWith = startsWith;
			base.dataField = dataField;
		}
	}
}
