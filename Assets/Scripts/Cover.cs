using System;
using UnityEngine;

public class Cover : MonoBehaviour
{
	public RectTransform left;

	public RectTransform right;

	public RectTransform above;

	public RectTransform below;

	private void Start()
	{
		UpdateCover();
		UICamera instance = UICamera.instance;
		instance.onScreenSizeChanged = (Action)Delegate.Combine(instance.onScreenSizeChanged, new Action(UpdateCover));
	}

	private void OnLevelWasLoaded(int level)
	{
		UpdateCover();
		UICamera instance = UICamera.instance;
		instance.onScreenSizeChanged = (Action)Delegate.Combine(instance.onScreenSizeChanged, new Action(UpdateCover));
	}

	private void UpdateCover()
	{
		float width = UICamera.instance.GetWidth();
		float height = UICamera.instance.GetHeight();
		float virtualWidth = UICamera.instance.virtualWidth;
		float virtualHeight = UICamera.instance.virtualHeight;
		float num = (virtualWidth - width * 200f) / 2f;
		float num2 = (virtualHeight - height * 200f) / 2f;
		left.gameObject.SetActive(num > 0.0001f);
		right.gameObject.SetActive(num > 0.0001f);
		above.gameObject.SetActive(num2 > 0.0001f);
		below.gameObject.SetActive(num2 > 0.0001f);
		float num3 = (!UICamera.instance.landscape) ? 800 : 400;
		float y = (!UICamera.instance.landscape) ? 400 : 800;
		Vector2 sizeDelta = left.sizeDelta;
		if (sizeDelta.x < num)
		{
			RectTransform rectTransform = left;
			float x = num;
			Vector2 sizeDelta2 = left.sizeDelta;
			rectTransform.sizeDelta = new Vector2(x, sizeDelta2.y);
			RectTransform rectTransform2 = right;
			float x2 = num;
			Vector2 sizeDelta3 = right.sizeDelta;
			rectTransform2.sizeDelta = new Vector2(x2, sizeDelta3.y);
		}
		else
		{
			RectTransform rectTransform3 = left;
			float x3 = num3;
			Vector2 sizeDelta4 = left.sizeDelta;
			rectTransform3.sizeDelta = new Vector2(x3, sizeDelta4.y);
			RectTransform rectTransform4 = right;
			float x4 = num3;
			Vector2 sizeDelta5 = right.sizeDelta;
			rectTransform4.sizeDelta = new Vector2(x4, sizeDelta5.y);
		}
		Vector2 sizeDelta6 = above.sizeDelta;
		if (sizeDelta6.y < num2)
		{
			RectTransform rectTransform5 = above;
			Vector2 sizeDelta7 = above.sizeDelta;
			rectTransform5.sizeDelta = new Vector2(sizeDelta7.x, num2);
			RectTransform rectTransform6 = below;
			Vector2 sizeDelta8 = below.sizeDelta;
			rectTransform6.sizeDelta = new Vector2(sizeDelta8.x, num2);
		}
		else
		{
			RectTransform rectTransform7 = above;
			Vector2 sizeDelta9 = above.sizeDelta;
			rectTransform7.sizeDelta = new Vector2(sizeDelta9.x, y);
			RectTransform rectTransform8 = below;
			Vector2 sizeDelta10 = below.sizeDelta;
			rectTransform8.sizeDelta = new Vector2(sizeDelta10.x, y);
		}
	}
}
