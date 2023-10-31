using System.Collections;
using UnityEngine;

public class BaseController : MonoBehaviour
{
	public GameObject donotDestroyOnLoad;

	public string sceneName;

	public Music.Type music;

	protected int numofEnterScene;

	protected virtual void Awake()
	{
		if (DonotDestroyOnLoad.instance == null && donotDestroyOnLoad != null)
		{
			Object.Instantiate(donotDestroyOnLoad);
		}
		//iTween.dimensionMode = iTween.DimensionMode.mode2D;
		CPlayerPrefs.useRijndael(use: true);
		numofEnterScene = CUtils.IncreaseNumofEnterScene(sceneName);
	}

	protected virtual void Start()
	{
		CPlayerPrefs.Save();
		if (JobWorker.instance.onEnterScene != null)
		{
			JobWorker.instance.onEnterScene(sceneName);
		}
		Music.instance.Play(music);
	}

	public virtual void OnApplicationPause(bool pause)
	{
		UnityEngine.Debug.Log("On Application Pause");
		CPlayerPrefs.Save();
	}

	private IEnumerator SavePrefs()
	{
		while (true)
		{
			yield return new WaitForSeconds(5f);
			CPlayerPrefs.Save();
		}
	}
}
