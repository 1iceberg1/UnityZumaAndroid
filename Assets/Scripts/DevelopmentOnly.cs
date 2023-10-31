using UnityEngine;

public class DevelopmentOnly : MonoBehaviour
{
	public bool setRuby;

	public int ruby;

	public bool setUnlockLevel;

	public int unlockLevel;

	public bool clearAllPrefs;

	private void Start()
	{
		if (setRuby)
		{
			CurrencyController.SetBalance(ruby);
		}
		if (setUnlockLevel)
		{
			LevelController.SetUnlockLevel(unlockLevel);
		}
		if (clearAllPrefs)
		{
			CPlayerPrefs.DeleteAll();
			CPlayerPrefs.Save();
		}
	}
}
