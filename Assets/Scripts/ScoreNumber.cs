using UnityEngine;
using UnityEngine.UI;

public class ScoreNumber : MonoBehaviour
{
	private int _number;

	public CProgress progress;

	public static ScoreNumber instance;

	public int Number
	{
		get
		{
			return _number;
		}
		set
		{
			_number = value;
			UpdateUI();
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		Number = 0;
	}

	private void UpdateUI()
	{
		GetComponent<Text>().text = Number.ToString();
	}

	public void AddScore(int value)
	{
		Number += value;
		progress.Current += value;
	}
}
