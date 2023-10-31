using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SnapScrollRect : MonoBehaviour, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	private float[] points;

	[Tooltip("how many screens or pages are there within the content (steps)")]
	public int screens = 1;

	public int speed = 10;

	private float stepSize;

	private ScrollRect scroll;

	private bool LerpH;

	private float targetH;

	[HideInInspector]
	public int indexH;

	[Tooltip("Snap horizontally")]
	public bool snapInHorizontal = true;

	private bool LerpV;

	private float targetV;

	[HideInInspector]
	public int indexV;

	[Tooltip("Snap vertically")]
	public bool snapInVertical = true;

	public GameObject[] indicators;

	public Text tabName;

	public string tabNamePrefix;

	public Action<int> onPageChanged;

	private void Awake()
	{
		scroll = base.gameObject.GetComponent<ScrollRect>();
		InitPoints();
	}

	public void InitPoints()
	{
		points = new float[screens];
		if (screens > 1)
		{
			stepSize = 1f / (float)(screens - 1);
			for (int i = 0; i < screens; i++)
			{
				points[i] = (float)i * stepSize;
			}
		}
		else
		{
			points[0] = 0f;
		}
	}

	private void Update()
	{
		if (LerpH)
		{
			scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, targetH, (float)speed * scroll.elasticity * Time.deltaTime);
			if (Mathf.Approximately(scroll.horizontalNormalizedPosition, targetH))
			{
				LerpH = false;
			}
		}
		if (LerpV)
		{
			scroll.verticalNormalizedPosition = Mathf.Lerp(scroll.verticalNormalizedPosition, targetV, (float)speed * scroll.elasticity * Time.deltaTime);
			if (Mathf.Approximately(scroll.verticalNormalizedPosition, targetV))
			{
				LerpV = false;
			}
		}
	}

	public void OnEndDrag(PointerEventData data)
	{
		if (scroll.horizontal && snapInHorizontal)
		{
			int num = 0;
			Vector2 velocity = scroll.velocity;
			num = (int)Mathf.Sqrt(Mathf.Abs(velocity.x)) / 10;
			Vector2 velocity2 = scroll.velocity;
			if (velocity2.x > 0f)
			{
				num = -num;
			}
			indexH = Math.Min(screens - 1, Math.Max(0, FindNearest(scroll.horizontalNormalizedPosition, points) + num));
			targetH = points[indexH];
			UpdateIndicator(isHorizontal: true);
			LerpH = true;
		}
		if (scroll.vertical && snapInVertical)
		{
			int num2 = 0;
			Vector2 velocity3 = scroll.velocity;
			num2 = (int)Mathf.Sqrt(Mathf.Abs(velocity3.y)) / 10;
			Vector2 velocity4 = scroll.velocity;
			if (velocity4.y > 0f)
			{
				num2 = -num2;
			}
			indexV = Math.Min(screens - 1, Math.Max(0, FindNearest(scroll.verticalNormalizedPosition, points) + num2));
			targetV = points[indexV];
			UpdateIndicator(isHorizontal: false);
			LerpV = true;
		}
	}

	public void NextPage(bool isHorizontal)
	{
		Sound.instance.PlayButton();
		if (isHorizontal)
		{
			indexH = Math.Min(screens - 1, indexH + 1);
			targetH = points[indexH];
			LerpH = true;
		}
		else
		{
			indexV = Math.Min(screens - 1, indexV + 1);
			targetV = points[indexV];
			LerpV = true;
		}
		UpdateIndicator(isHorizontal);
	}

	public void PreviousPage(bool isHorizontal)
	{
		Sound.instance.PlayButton();
		if (isHorizontal)
		{
			indexH = Math.Max(0, indexH - 1);
			targetH = points[indexH];
			LerpH = true;
		}
		else
		{
			indexV = Math.Max(0, indexV - 1);
			targetV = points[indexV];
			LerpV = true;
		}
		UpdateIndicator(isHorizontal);
	}

	public void SetPage(int pageIndex, bool isHorizontal)
	{
		if (isHorizontal)
		{
			indexH = Math.Min(screens - 1, pageIndex);
			targetH = points[indexH];
			scroll.horizontalNormalizedPosition = targetH;
		}
		else
		{
			indexV = Math.Min(screens - 1, pageIndex);
			targetV = points[indexV];
			scroll.verticalNormalizedPosition = targetV;
		}
		UpdateIndicator(isHorizontal);
	}

	public void UpdateIndicator(bool isHorizontal)
	{
		int num = (!isHorizontal) ? indexV : indexH;
		for (int i = 0; i < indicators.Length; i++)
		{
			indicators[i].SetActive(i == num);
		}
		if (tabName != null)
		{
			tabName.text = tabNamePrefix + (num + 1);
		}
		if (onPageChanged != null)
		{
			onPageChanged(num);
		}
	}

	public void OnDrag(PointerEventData data)
	{
		LerpH = false;
		LerpV = false;
	}

	private int FindNearest(float f, float[] array)
	{
		float num = float.PositiveInfinity;
		int result = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (Mathf.Abs(array[i] - f) < num)
			{
				num = Mathf.Abs(array[i] - f);
				result = i;
			}
		}
		return result;
	}
}
