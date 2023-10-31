using UnityEngine;

public class LevelController
{
	public static int GetCurrentLevel()
	{
		return CPlayerPrefs.GetInt("current_level", 1);
	}

	public static void SetCurrentLevel(int level)
	{
		CPlayerPrefs.SetInt("current_level", level);
	}

	public static int GetUnlockLevel()
	{
		return CPlayerPrefs.GetInt("unlocked_level", 1);
	}

	public static void SetUnlockLevel(int level)
	{
		CPlayerPrefs.SetInt("unlocked_level", level);
	}

	public static int GetMovedLevel()
	{
		return CPlayerPrefs.GetInt("moved_level", 1);
	}

	public static void SetMovedLevel(int value)
	{
		CPlayerPrefs.SetInt("moved_level", value);
	}

	public static int GetNumStar(int level, int defaultStar = 0)
	{
		return CPlayerPrefs.GetInt("num_star_level_" + level, defaultStar);
	}

	public static void SetNumStar(int level, int value)
	{
		int numStar = GetNumStar(level);
		CPlayerPrefs.SetInt("num_star_level_" + level, Mathf.Max(numStar, value));
	}
}
