using System;

public class OkDialog : Dialog
{
	public Action onOkClick;

	public virtual void OnOkClick()
	{
		Sound.instance.PlayButton();
		if (onOkClick != null)
		{
			onOkClick();
		}
		Sound.instance.PlayButton();
		Close();
	}
}
