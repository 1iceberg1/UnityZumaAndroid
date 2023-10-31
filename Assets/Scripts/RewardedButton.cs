using GoogleMobileAds.Api;
using UnityEngine;

public class RewardedButton : MonoBehaviour
{
	public GameObject content;

    public AdmobController admob;

    private const string ACTION_NAME = "rewarded_video";

	private void Start()
	{
		//if (AdmobController.instance.rewardBasedVideo != null)
		//{
		//	AdmobController.instance.rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
	//	}
		//if (!IsAvailableToShow())
		//{
		//	content.SetActive(value: false);
		//}
	}

	public void OnClick()
	{
        admob = GameObject.Find("AdmobController").GetComponent<AdmobController>();
        admob.RewardShow();
		Sound.instance.PlayButton();
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		content.SetActive(value: false);
		CurrencyController.CreditBalance(5);
		//Toast.instance.ShowMessage("You got 5 free rubies");
	}

//	public bool IsAvailableToShow()
	//{
	//	//return IsActionAvailable() && IsAdAvailable();
	//}

	//private bool IsActionAvailable()
	//{
	//	return CUtils.IsActionAvailable("rewarded_video", ConfigController.Config.rewardedVideoPeriod);
	//}

	//private bool IsAdAvailable()
	//{
		//if (AdmobController.instance.rewardBasedVideo == null)
		//{
		//	return false;
		//}
		//bool flag = AdmobController.instance.rewardBasedVideo.IsLoaded();
		//if (!flag)
		//{
		//	AdmobController.instance.RequestRewardBasedVideo();
		//}
		//return flag;
	//}

	private void OnDestroy()
	{
		//if (AdmobController.instance.rewardBasedVideo != null)
		//{
		//	AdmobController.instance.rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
		//}
	}
}
