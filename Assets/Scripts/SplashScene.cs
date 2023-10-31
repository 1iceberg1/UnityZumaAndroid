using UnityEngine;

public class SplashScene : MonoBehaviour
{
	private void Start()
	{
	}

	public void TouchStartClick()
	{
		GetComponent<SceneTransition>().PerformTransition();
	}
}
