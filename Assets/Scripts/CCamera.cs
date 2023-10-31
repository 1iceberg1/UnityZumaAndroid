using System;
using UnityEngine;

public class CCamera : MonoBehaviour
{
	public bool landscape;

	public float minWidth;

	public float minHeight;

	public float maxWidth;

	public float maxHeight;

	[HideInInspector]
	public float virtualWidth;

	[HideInInspector]
	public float virtualHeight;

	private int screenWidth;

	private int screenHeight;

	private float aspect;

	private Camera cam;

	private bool matchWidth;

	private float width;

	private float height;

	private float minAspect;

	private float maxAspect;

	public Action onScreenSizeChanged;

	protected virtual void Awake()
	{
		cam = GetComponent<Camera>();
		matchWidth = (minWidth == maxWidth);
		if (landscape)
		{
			minAspect = minWidth / Mathf.Min(minHeight, maxHeight);
			maxAspect = maxWidth / Mathf.Max(minHeight, maxHeight);
		}
		else
		{
			minAspect = minHeight / Mathf.Max(minWidth, maxWidth);
			maxAspect = maxHeight / Mathf.Min(minWidth, maxWidth);
		}
		UpdateCamera();
	}

	private void Update()
	{
		if (screenWidth != Screen.width || screenHeight != Screen.height)
		{
			UpdateCamera();
		}
	}

	private void UpdateCamera()
	{
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		if (landscape)
		{
			aspect = (float)Screen.width / (float)Screen.height;
			SetUpForLandscape();
		}
		else
		{
			aspect = (float)Screen.height / (float)Screen.width;
			SetUpForPortrait();
		}
		virtualHeight = cam.orthographicSize * 200f;
		virtualWidth = ((!landscape) ? (virtualHeight / aspect) : (virtualHeight * aspect));
		if (onScreenSizeChanged != null)
		{
			onScreenSizeChanged();
		}
	}

	private void SetUpForPortrait()
	{
		float num = Mathf.Clamp(aspect, minAspect, maxAspect);
		if (matchWidth)
		{
			width = minWidth / 200f;
			height = width * num;
		}
		else
		{
			height = minHeight / 200f;
			width = height / num;
		}
		cam.orthographicSize = ((!(aspect < maxAspect)) ? (width * aspect) : height);
	}

	private void SetUpForLandscape()
	{
		float num = Mathf.Clamp(aspect, minAspect, maxAspect);
		if (matchWidth)
		{
			width = minWidth / 200f;
			height = width / num;
		}
		else
		{
			height = minHeight / 200f;
			width = height * num;
		}
		cam.orthographicSize = ((!(aspect >= minAspect)) ? (width / aspect) : height);
	}

	public float GetHeight()
	{
		return height;
	}

	public float GetWidth()
	{
		return width;
	}
}
