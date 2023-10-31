using UnityEngine;
using UnityEngine.UI;

public class CTextNumber : MonoBehaviour
{
	public int defaultValue;

	private int number;

	public int Number
	{
		get
		{
			return number;
		}
		set
		{
			number = value;
			UpdateUI();
		}
	}

	private void Awake()
	{
		Number = defaultValue;
	}

	private void UpdateUI()
	{
		Text component = GetComponent<Text>();
		component.text = Number.ToString();
	}
}
