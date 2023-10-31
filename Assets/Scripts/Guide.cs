using UnityEngine;

public class Guide : MonoBehaviour
{
	public enum Type
	{
		SwitchBalls,
		TipSkipBall,
		TipSwitchBalls
	}

	public Type type;

	public GameObject content;

	public float autoHideAfterTime = -1f;

	public void Show()
	{
		content.SetActive(value: true);
		if (autoHideAfterTime != -1f)
		{
			Invoke("Done", autoHideAfterTime);
		}
	}

	public void Done()
	{
		content.SetActive(value: false);
		SetDone(type, isDone: true);
	}

	public static void SetDone(Type type, bool isDone)
	{
		CPlayerPrefs.SetBool("is_guide_" + type.ToString() + "_done", isDone);
	}

	public static bool IsDone(Type type)
	{
		return CPlayerPrefs.GetBool("is_guide_" + type.ToString() + "_done");
	}
}
