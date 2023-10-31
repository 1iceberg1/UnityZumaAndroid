public class ButtonSoundToggle : TButton
{
	protected override void Start()
	{
		base.Start();
		base.IsOn = Sound.instance.IsEnabled();
	}

	public override void OnButtonClick()
	{
		base.OnButtonClick();
		Sound.instance.SetEnabled(base.IsOn);
		Sound.instance.PlayButton();
	}
}
