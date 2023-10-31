public class ButtonOpenDialog : MyButton
{
	public DialogType dialogType;

	public DialogShow dialogShow;

	public override void OnButtonClick()
	{
		base.OnButtonClick();
		DialogController.instance.ShowDialog(dialogType, dialogShow);
	}
}
