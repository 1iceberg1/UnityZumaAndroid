using UnityEngine;

public class SceneTransition : MonoBehaviour
{
	public string scene = "<Insert scene name>";

	public float duration = 1f;

	public Color color = Color.black;

	public void PerformTransition()
	{
		Transition.LoadLevel(scene, duration, color);
	}
}
