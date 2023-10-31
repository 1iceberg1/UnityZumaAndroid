using UnityEngine;

public class QuitGameDialog : YesNoDialog
{
	protected override void Start()
	{
		base.Start();
		onYesClick = Quit;
	}

	private void Quit()
	{
		Application.Quit();
	}
}
