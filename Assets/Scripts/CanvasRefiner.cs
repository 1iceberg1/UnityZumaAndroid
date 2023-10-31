using System;
using UnityEngine;

public class CanvasRefiner : MonoBehaviour
{
	public bool isFullScreen;

	private RectTransform tr;

	private void Start()
	{
		tr = GetComponent<RectTransform>();
		OnScreenSizeChanged();
		UICamera instance = UICamera.instance;
		instance.onScreenSizeChanged = (Action)Delegate.Combine(instance.onScreenSizeChanged, new Action(OnScreenSizeChanged));
	}

	private void OnLevelWasLoaded(int level)
	{
		tr = GetComponent<RectTransform>();
		OnScreenSizeChanged();
		UICamera instance = UICamera.instance;
		instance.onScreenSizeChanged = (Action)Delegate.Combine(instance.onScreenSizeChanged, new Action(OnScreenSizeChanged));
	}

	private void OnScreenSizeChanged()
	{
		if (isFullScreen)
		{
			tr.sizeDelta = new Vector2(UICamera.instance.virtualWidth, UICamera.instance.virtualHeight);
		}
		else
		{
			tr.sizeDelta = new Vector2(UICamera.instance.GetWidth() * 200f, UICamera.instance.GetHeight() * 200f);
		}
	}
}
