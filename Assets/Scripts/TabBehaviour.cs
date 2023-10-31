using UnityEngine;

public class TabBehaviour : MonoBehaviour
{
	public GameObject[] tabButtons;

	public GameObject[] tabContents;

	public GameObject[] tabNameObjects;

	public GameObject[] selectedTabs;

	public string[] tabName;

	protected int currentTab;

	private void Start()
	{
		int num = 0;
		while (true)
		{
			if (num < tabContents.Length)
			{
				if (tabContents[num].activeSelf)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		currentTab = num;
	}

	public void TabChanged(int index)
	{
		Sound.instance.PlayButton();
		currentTab = index;
		for (int i = 0; i < tabButtons.Length; i++)
		{
			TabVisible(i);
			if (i == index)
			{
				TabSelected(i);
			}
			else
			{
				TabUnselected(i);
			}
		}
	}

	protected virtual void TabVisible(int index)
	{
	}

	protected virtual void TabSelected(int index)
	{
		tabContents[index].SetActive(value: true);
		selectedTabs[index].SetActive(value: true);
	}

	protected virtual void TabUnselected(int index)
	{
		tabContents[index].SetActive(value: false);
		selectedTabs[index].SetActive(value: false);
	}

	public void SetCurrentTab(int index)
	{
		TabChanged(index);
	}
}
