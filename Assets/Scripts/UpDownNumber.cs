using System;
using UnityEngine;

public class UpDownNumber : MonoBehaviour
{
	public bool hasLimit;

	public int max = 1000;

	public int min = 1;

	public int number = 1;

	public Action onNumberChanged;

	public int Number
	{
		get
		{
			return number;
		}
		set
		{
			number = value;
			if (hasLimit)
			{
				base.gameObject.SetText(number + "/" + max);
			}
			else
			{
				base.gameObject.SetText(number.ToString());
			}
			if (onNumberChanged != null)
			{
				onNumberChanged();
			}
		}
	}

	private void Start()
	{
		Number = number;
	}

	public void OnNumberChanged(int value)
	{
		Sound.instance.PlayButton();
		if (max != 0)
		{
			if (hasLimit)
			{
				Number = Mathf.Clamp(number + value, min, max);
			}
			else
			{
				Number = Mathf.Max(number + value, min);
			}
		}
	}
}
