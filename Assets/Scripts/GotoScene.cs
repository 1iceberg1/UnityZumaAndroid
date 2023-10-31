using UnityEngine;

public class GotoScene : MonoBehaviour
{
	public int sceneIndex;

	public bool useScreenFader;

	public bool useKeyCode;

	public KeyCode keyCode;

	public virtual void LoadScene()
	{
		CUtils.LoadScene(sceneIndex, useScreenFader);
	}

	private void Update()
	{
		if (useKeyCode && UnityEngine.Input.GetKeyDown(keyCode) && !DialogController.instance.IsDialogShowing())
		{
			LoadScene();
		}
	}
}
