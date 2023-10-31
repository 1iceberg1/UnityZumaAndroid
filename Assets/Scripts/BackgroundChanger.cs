using UnityEngine;
using UnityEngine.UI;

public class BackgroundChanger : MonoBehaviour
{
	public Image image;

	public Sprite[] sprites;

	public int[] levelIndexes;

	private void Start()
	{
		int currentLevel = LevelController.GetCurrentLevel();
		image.sprite = sprites[GetLevelIndex(currentLevel)];
	}

	private int GetLevelIndex(int level)
	{
		for (int num = levelIndexes.Length - 1; num >= 0; num--)
		{
			if (level >= levelIndexes[num])
			{
				return num;
			}
		}
		return 0;
	}
}
