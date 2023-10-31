using UnityEngine;

public class GuideController : MonoBehaviour
{
	private Guide[] guides;

	public static GuideController instance;

	private void Awake()
	{
		instance = this;
	}

	private void UpdateList()
	{
		guides = UnityEngine.Object.FindObjectsOfType<Guide>();
	}

	public void Show(Guide.Type type)
	{
		UpdateList();
		Guide[] array = guides;
		foreach (Guide guide in array)
		{
			if (type == guide.type)
			{
				guide.Show();
			}
		}
	}

	public bool IsShowing(Guide.Type type)
	{
		if (guides == null)
		{
			return false;
		}
		Guide[] array = guides;
		foreach (Guide guide in array)
		{
			if (type == guide.type)
			{
				return guide.content.activeSelf;
			}
		}
		return false;
	}

	public void Done(Guide.Type type)
	{
		UpdateList();
		Guide[] array = guides;
		foreach (Guide guide in array)
		{
			if (type == guide.type)
			{
				guide.Done();
			}
		}
	}
}
