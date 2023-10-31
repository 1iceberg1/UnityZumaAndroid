public class ButtonRate : MyButton
{
	public override void OnButtonClick()
	{
		base.OnButtonClick();
		CUtils.RateGame();
	}
}
