using UnityEngine;

public class Exitgame : MonoBehaviour
{
	public static Exitgame instance;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && !DialogController.instance.IsDialogShowing())
		{
			DialogController.instance.ShowDialog(DialogType.QuitGame);
		}
	}

	private void Awake()
	{
	}
}
