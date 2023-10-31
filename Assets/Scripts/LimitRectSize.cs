using UnityEngine;

public class LimitRectSize : MonoBehaviour
{
	public bool landscape;

	[Range(0f, 1f)]
	public int matchWidthHeight;

	public float minWidth;

	public float minHeight;

	public float maxWidth;

	public float maxHeight;

	private float aspect;

	private float minAspect;

	private float maxAspect;

	private bool matchWidth;

	private float width;

	private float height;

	private RectTransform rectTransform;

	private void Start()
	{
		matchWidth = (matchWidthHeight == 0);
		rectTransform = GetComponent<RectTransform>();
		if (landscape)
		{
			minAspect = minWidth / Mathf.Max(minHeight, maxHeight);
			maxAspect = maxWidth / Mathf.Min(minHeight, maxHeight);
		}
		else
		{
			minAspect = minHeight / Mathf.Max(minWidth, maxWidth);
			maxAspect = maxHeight / Mathf.Min(minWidth, maxWidth);
		}
	}

	private void Update()
	{
		if (landscape)
		{
			aspect = (float)Screen.width / (float)Screen.height;
			float num = Mathf.Clamp(aspect, minAspect, maxAspect);
			if (matchWidth)
			{
				width = ((!(aspect <= maxAspect)) ? (minWidth * maxAspect / aspect) : minWidth);
				height = width / num;
			}
			else
			{
				height = ((!(aspect >= minAspect)) ? (minHeight * aspect / minAspect) : minHeight);
				width = height * num;
			}
		}
		else
		{
			aspect = (float)Screen.height / (float)Screen.width;
			float num2 = Mathf.Clamp(aspect, minAspect, maxAspect);
			if (matchWidth)
			{
				width = ((!(aspect >= minAspect)) ? (minWidth * aspect / minAspect) : minWidth);
				height = width * num2;
			}
			else
			{
				height = ((!(aspect <= maxAspect)) ? (minHeight * maxAspect / aspect) : minHeight);
				width = height / num2;
			}
		}
		rectTransform.sizeDelta = new Vector2(width, height);
	}
}
