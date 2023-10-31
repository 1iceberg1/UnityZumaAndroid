using System;
using System.Collections;
using UnityEngine;

public class ClockText : MonoBehaviour
{
	public string action;

	public int duration;

	public string contentWhenComplete = string.Empty;

	public bool availableFirstTime = true;

	public Action onClockComplete;

	private int remainingTime;

	public void ShowClockText()
	{
		if (!CUtils.IsActionAvailable(action, duration, availableFirstTime))
		{
			int num = (int)(CUtils.GetCurrentTime() - CUtils.GetActionTime(action));
			remainingTime = duration - num;
			StartCoroutine(UpdateClockText());
		}
		else
		{
			ClockComplete();
		}
	}

	public void UpdateTimeClockText()
	{
		if (!CUtils.IsActionAvailable(action, duration, availableFirstTime))
		{
			int num = (int)(CUtils.GetCurrentTime() - CUtils.GetActionTime(action));
			remainingTime = duration - num;
			UpdateText();
		}
	}

	private IEnumerator UpdateClockText()
	{
		while (remainingTime > 0)
		{
			UpdateText();
			yield return new WaitForSeconds(1f);
			remainingTime--;
		}
		ClockComplete();
	}

	private void UpdateText()
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
		string value = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		base.gameObject.SetText(value);
	}

	private void ClockComplete()
	{
		base.gameObject.SetText(contentWhenComplete);
		if (onClockComplete != null)
		{
			onClockComplete();
		}
	}
}
