using System;
using UnityEngine;

[Serializable]
public class Admob
{
	[Header("Interstitial")]
	public string androidInterstitial;

	public string iosInterstitial;

	[Header("Banner")]
	public string androidBanner;

	public string iosBanner;

	[Header("RewardedVideo")]
	public string androidRewarded;

	public string iosRewarded;
}
