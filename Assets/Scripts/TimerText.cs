using System;
using System.Collections;
using UnityEngine;

public class TimerText : MonoBehaviour
{
	public bool countUp = true;

	public bool runOnStart;

	public int timeValue;

	public bool showHour;

	public bool showMinute = true;

	public bool showSecond = true;

	public Action onCountDownComplete;

	private bool isRunning;

	private void Start()
	{
		UpdateText();
		if (runOnStart)
		{
			Run();
		}
	}

	public void Run()
	{
		isRunning = true;
		StartCoroutine(UpdateClockText());
	}

	private IEnumerator UpdateClockText()
	{
		while (isRunning)
		{
			UpdateText();
			yield return new WaitForSeconds(1f);
			if (countUp)
			{
				timeValue++;
			}
			else if (timeValue == 0)
			{
				if (onCountDownComplete != null)
				{
					onCountDownComplete();
				}
				Stop();
			}
			else
			{
				timeValue--;
			}
		}
	}

	public void SetTime(int value)
	{
		if (value < 0)
		{
			value = 0;
		}
		timeValue = value;
		UpdateText();
	}

	private void UpdateText()
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(timeValue);
		CExtension.SetText(value: (showHour && showMinute && showSecond) ? $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}" : ((!showHour || !showMinute) ? $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}" : $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}"), obj: base.gameObject);
	}

	public void Stop()
	{
		isRunning = false;
	}
}
