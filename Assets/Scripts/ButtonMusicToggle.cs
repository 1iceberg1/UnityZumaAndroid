public class ButtonMusicToggle : TButton
{
	protected override void Start()
	{
		base.Start();
		base.IsOn = Music.instance.IsEnabled();
	}

	public override void OnButtonClick()
	{
		base.OnButtonClick();
		Music.instance.SetEnabled(base.IsOn, updateMusic: true);
		Sound.instance.PlayButton();
	}
}
