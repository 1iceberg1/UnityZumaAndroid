public class ButtonPlay : MyButton
{
	public int gotoIndex = 1;

	public bool useScreenFader;

	public override void OnButtonClick()
	{
		base.OnButtonClick();
		CUtils.LoadScene(gotoIndex, useScreenFader);
	}
}
