using System;
using UnityEngine;

[Serializable]
public class GameConfig
{
	public Admob admob;

	[Header("")]
	public int adPeriod;

	public int rewardedVideoPeriod;

	public int rewardedVideoAmount;

	public string androidPackageID;

	public string iosAppID;

	public string macAppID;

	public string facebookPageID;
}
