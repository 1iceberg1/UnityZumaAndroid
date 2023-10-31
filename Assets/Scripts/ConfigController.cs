using UnityEngine;

public class ConfigController : ConnectServer
{
	public GameConfig config;

	public static ConfigController instance;

	public static GameConfig Config => instance.config;

	public string KeyPref => "game_config_" + versionAPI;

	private void Awake()
	{
		instance = this;
		GetConfig();
	}

	private void Start()
	{
		string url = rootUrl + versionAPI + "/config.txt";
		StartCoroutine(GetDataFromServer(url, ApplyConfig));
	}

	private void GetConfig()
	{
		if (CPlayerPrefs.HasKey(KeyPref))
		{
			try
			{
				CPlayerPrefs.useRijndael(use: false);
				string @string = CPlayerPrefs.GetString(KeyPref);
				CPlayerPrefs.useRijndael(use: true);
				GameConfig gameConfig = JsonUtility.FromJson<GameConfig>(@string);
				if (gameConfig != null)
				{
					config = gameConfig;
				}
			}
			catch
			{
			}
		}
	}

	public void ApplyConfig(string data)
	{
		CPlayerPrefs.useRijndael(use: false);
		CPlayerPrefs.SetString(KeyPref, data);
		CPlayerPrefs.useRijndael(use: true);
		GetConfig();
	}
}
