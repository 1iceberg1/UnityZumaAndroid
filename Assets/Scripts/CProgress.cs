using UnityEngine;

public class CProgress : MonoBehaviour
{
	public float[] targetsPercent;

	public float maxProgress = 1000f;

	public GameObject[] stars;

	private bool[] reachTargets;

	private RectTransform rectransform;

	private float maxWidth;

	private float current;

	public float Current
	{
		get
		{
			return current;
		}
		set
		{
			current = value;
			UpdateUI();
		}
	}

	private void Start()
	{
		rectransform = GetComponent<RectTransform>();
		Vector2 sizeDelta = rectransform.sizeDelta;
		maxWidth = sizeDelta.x;
		reachTargets = new bool[targetsPercent.Length];
		Current = 0f;
	}

	private void UpdateUI()
	{
		if (maxProgress == 0f)
		{
			return;
		}
		float num = Mathf.Clamp(Current / maxProgress, 0f, 1f);
		RectTransform rectTransform = rectransform;
		float x = maxWidth * num;
		Vector2 sizeDelta = rectransform.sizeDelta;
		rectTransform.sizeDelta = new Vector2(x, sizeDelta.y);
		for (int i = 0; i < targetsPercent.Length; i++)
		{
			if (!reachTargets[i] && num >= targetsPercent[i])
			{
				OnReachTarget(i);
				reachTargets[i] = true;
			}
		}
	}

	private void OnReachTarget(int target)
	{
		if (stars.Length > target)
		{
			stars[target].SetActive(value: true);
		}
	}

	public int GetReachedTarget()
	{
		int num = 0;
		for (int i = 0; i < reachTargets.Length; i++)
		{
			if (reachTargets[i])
			{
				num++;
			}
		}
		return num;
	}
}
