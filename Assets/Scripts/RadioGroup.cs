using UnityEngine;
using UnityEngine.UI;

public class RadioGroup : MonoBehaviour
{
	public Image[] highlights;

	public string preferenceKey;

	private int selectedIndex;

	private void Start()
	{
		OnItemClick(CPlayerPrefs.GetInt(preferenceKey, 0));
	}

	public void OnItemClick(int index)
	{
		int num = 0;
		Image[] array = highlights;
		foreach (Image image in array)
		{
			if (num == index)
			{
				image.gameObject.SetActive(value: true);
			}
			else
			{
				image.gameObject.SetActive(value: false);
			}
			num++;
		}
		selectedIndex = index;
	}

	public void SaveChanged()
	{
		CPlayerPrefs.SetInt(preferenceKey, selectedIndex);
	}

	public int GetSelectedIndex()
	{
		return selectedIndex;
	}
}
