using Superpow;

public class RateDialog : YesNoDialog
{
	public override void OnYesClick()
	{
		base.OnYesClick();
		CUtils.RateGame();
	}

	public override void OnNoClick()
	{
		base.OnNoClick();
		Utils.SetAskRateTime();
	}
}
