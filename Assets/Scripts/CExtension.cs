using System;
using UnityEngine;
using UnityEngine.UI;

public static class CExtension
{
	public static void SetText(this GameObject obj, string value)
	{
		Text component = obj.GetComponent<Text>();
		if (component != null)
		{
			component.text = value;
		}
	}

	public static void SetText(this Text objText, string value)
	{
		objText.text = value;
	}

	public static void SetTimeText(this Text text, string preFix, int time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		text.text = preFix + $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
	}
}
