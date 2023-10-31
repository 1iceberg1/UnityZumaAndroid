using UnityEngine.UI;

public class PauseDialog : YesNoDialog
{
	public Toggle music;

	public Toggle sound;

	protected override void Start()
	{
		base.Start();
		onYesClick = MenuClick;
		onNoClick = ContinueClick;
		music.isOn = Music.instance.IsEnabled();
		sound.isOn = Sound.instance.IsEnabled();
		Timer.Schedule(this, 0.5f, delegate
		{
			GameState.pauseGame = true;
		});
	}

	private void MenuClick()
	{
		GotoMenu();
	}

	private void GotoMenu()
	{
		GameState.pauseGame = false;
		SaveSetting();
		CUtils.LoadScene(1, useScreenFader: true);
	}

	private void ContinueClick()
	{
		GameState.pauseGame = false;
		SaveSetting();
	}

	public void ReplayClick()
	{
		GameState.pauseGame = false;
		CUtils.ReloadScene(useScreenFader: true);
		Sound.instance.PlayButton();
		Close();
	}

	public void SaveSetting()
	{
		Music.instance.SetEnabled(music.isOn, updateMusic: true);
		Sound.instance.SetEnabled(sound.isOn);
	}

	public void CloseDialog()
	{
		GameState.pauseGame = false;
	}

	public void OnToggleChanged()
	{
		Sound.instance.PlayButton();
	}
}
