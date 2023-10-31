using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PromoteController : ConnectServer
{
	[HideInInspector]
	public List<Promote> promotes;

	public static PromoteController instance;

	public string KeyPref => "promotes_" + versionAPI;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		promotes = GetPromotes();
		UpdatePromotion();
		string url = rootUrl + versionAPI + "/promote.txt";
		StartCoroutine(GetDataFromServer(url, ApplyPromotion));
	}

	public Promote GetPromote(PromoteType promoteType)
	{
		if (promotes == null)
		{
			return null;
		}
		List<Promote> list = promotes.FindAll((Promote x) => x.type == promoteType && !CUtils.IsAppInstalled(x.package) && CUtils.IsCacheExists(x.featureUrl));
		if (list == null || list.Count == 0)
		{
			return null;
		}
		return CUtils.GetRandom(list.ToArray());
	}

	private List<string> GetPackages()
	{
		return (from x in promotes
			select x.package).ToList();
	}

	public void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			UpdatePromotion();
		}
	}

	private void UpdatePromotion()
	{
		if (promotes != null)
		{
			List<Promote> list = promotes.FindAll((Promote x) => CUtils.IsAppInstalled(x.package) && x.rewardType == RewardType.RemoveAds);
			if (list.Count == 0)
			{
				CUtils.SetRemoveAds(value: false);
			}
			list = promotes.FindAll((Promote x) => !CUtils.IsAppInstalled(x.package) && x.rewardType == RewardType.RemoveAds && IsRewarded(x.package));
			foreach (Promote item in list)
			{
				CPlayerPrefs.SetBool(item.package + "_rewarded", value: false);
			}
			List<string> installedApp = GetInstalledApp();
			Reward(installedApp);
		}
	}

	private List<string> GetInstalledApp()
	{
		return GetPackages().FindAll((string x) => CUtils.IsAppInstalled(x) && !IsRewarded(x));
	}

	private void Reward(List<string> packages)
	{
		foreach (string package in packages)
		{
			if (CPlayerPrefs.GetBool(package + "_clicked_install"))
			{
				Reward(package);
			}
		}
	}

	private bool IsRewarded(string package)
	{
		return CPlayerPrefs.GetBool(package + "_rewarded");
	}

	private void Reward(string package)
	{
		CPlayerPrefs.SetBool(package + "_rewarded", value: true);
		Promote promote = promotes.Find((Promote x) => x.package == package);
		if (promote != null)
		{
			switch (promote.rewardType)
			{
			case RewardType.RemoveAds:
				CUtils.SetRemoveAds(value: true);
				Toast.instance.ShowMessage(promote.rewardMessage);
				break;
			case RewardType.Currency:
				CurrencyController.CreditBalance(promote.rewardValue);
				Toast.instance.ShowMessage(promote.rewardMessage);
				break;
			}
		}
	}

	private void CacheFeature()
	{
		if (promotes != null)
		{
			foreach (Promote promote in promotes)
			{
				StartCoroutine(CUtils.CachePicture(promote.featureUrl, null));
			}
		}
	}

	public void ApplyPromotion(string data)
	{
		CPlayerPrefs.useRijndael(use: false);
		CPlayerPrefs.SetString(KeyPref, data);
		CPlayerPrefs.useRijndael(use: true);
		promotes = GetPromotes(data);
		CacheFeature();
	}

	private List<Promote> GetPromotes(string data)
	{
		try
		{
			return JsonUtility.FromJson<Promotes>(data).promotes;
		}
		catch
		{
			return null;
		}
	}

	private List<Promote> GetPromotes()
	{
		if (promotes != null)
		{
			return promotes;
		}
		if (!CPlayerPrefs.HasKey(KeyPref))
		{
			return null;
		}
		CPlayerPrefs.useRijndael(use: false);
		string @string = CPlayerPrefs.GetString(KeyPref);
		CPlayerPrefs.useRijndael(use: true);
		return GetPromotes(@string);
	}

	public void OnInstallClick(string package)
	{
		CPlayerPrefs.SetBool(package + "_clicked_install", value: true);
	}
}
