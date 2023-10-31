using System;
using UnityEngine;

public class JobWorker : MonoBehaviour
{
	public Action<string> onEnterScene;

	public Action onLink2Store;

	public Action onDailyGiftReceived;

	public Action onShowBanner;

	public Action onCloseBanner;

	public Action onShowFixedBanner;

	public Action onShowInterstitial;

	public static JobWorker instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		CurrencyController.onBallanceIncreased = (Action<int>)Delegate.Combine(CurrencyController.onBallanceIncreased, new Action<int>(OnBallanceIncreased));
	}

	private void OnFacebookLoginComplete()
	{
	}

	private void OnBallanceIncreased(int value)
	{
		int leaderboardScore = CUtils.GetLeaderboardScore();
		CUtils.SetLeaderboardScore(leaderboardScore + value);
	}
}
