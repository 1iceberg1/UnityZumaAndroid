using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class Admobs : MonoBehaviour
{
    public string appid_android;
    private string banner_adid_android;
    public string inter_adid_android;
    public string reward_adid_android;
    public string appid_ios;
    private string banner_adid_ios;
    public string inter_adid_ios;
    public string reward_adid_ios;
    public int timerset;
    private int timer;
    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private DateTime appOpenExpireTime;
    private AppOpenAd appOpenAd;
    public BannerView bannerView;
    public InterstitialAd interstitialAd;
    public RewardedAd rewardedAd;
    private RewardedInterstitialAd rewardedInterstitialAd;
    private float deltaTime;
    private bool isShowingAppOpenAd;
    public UnityEvent OnAdLoadedEvent;
    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;
    public bool showFpsMeter = true;
    public Text fpsMeter;
    public Text statusText;
    //public RewardedAd rewardedAd;
   // public InterstitialAd interstitialAd;
    //public BannerView bannerView;
    //public BannerView exitDialogAd;
    //public int coinToAdd = 100;
   
    
    // Use this for initialization
    public void Start()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);



        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);

        // Get singleton reward based video ad reference.
        //this.rewardedAd = RewardedAd.Instance;
        // Listen to application foreground / background events.
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;

        timer = PlayerPrefs.GetInt("xtimer", timerset);

    }
    //init handle
    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        Debug.Log("Initialization complete.");

        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // the main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            // statusText.text = "Initialization complete.";
            RequestInterstitial();
            RequestRewardBasedVideo();
        });
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();
    }

    //request banner
    public void RequestBanner()
    {
        PrintStatus("Requesting Banner ad.");

#if UNITY_ANDROID
        string adUnitId = banner_adid_android;
#elif UNITY_IPHONE
            string adUnitId = banner_adid_ios;
#else
            string adUnitId = "unexpected_platform";
#endif
        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Add Event Handlers
        bannerView.OnAdLoaded += (sender, args) =>
        {
          //  PrintStatus("Banner ad loaded.");
            OnAdLoadedEvent.Invoke();
        };
        bannerView.OnAdFailedToLoad += (sender, args) =>
        {
           // PrintStatus("Banner ad failed to load with error: " + args.LoadAdError.GetMessage());
           OnAdFailedToLoadEvent.Invoke();
        };
        bannerView.OnAdOpening += (sender, args) =>
        {
           // PrintStatus("Banner ad opening.");
            OnAdOpeningEvent.Invoke();
        };
        bannerView.OnAdClosed += (sender, args) =>
        {
          //  PrintStatus("Banner ad closed.");
            OnAdClosedEvent.Invoke();
        };
        bannerView.OnPaidEvent += (sender, args) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "Banner ad received a paid event.",
                                        args.AdValue.CurrencyCode,
                                        args.AdValue.Value);
           PrintStatus(msg);
        };

        // Load a banner ad
        bannerView.LoadAd(CreateAdRequest());

        //ad request this game only:
        //  if (bannerView == null)
        //  {
        // bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        // AdRequest request = new AdRequest.Builder().Build();
        // this.bannerView.LoadAd(request);
        // bannerView.Hide();
        // }
        // if (exitDialogAd == null)
        // {
        // exitDialogAd = new BannerView(adUnitId, AdSize.MediumRectangle, AdPosition.Center);
        // AdRequest request2 = new AdRequest.Builder().Build();
        // this.exitDialogAd.LoadAd(request2);
        // exitDialogAd.Hide();
        // }
    }
    public void ShowBanner()
    {
       // bannerView.Show();
    }

    public void HideBanner()
    {
       // bannerView.Hide();
    }

    public void DestroyBanner()
    {
       // if (bannerView != null)
       // {
       //     bannerView.Destroy();
       // }
    }
    //handle banner:

   




    //handleinter:
    /*
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {

    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {

    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {

    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        interstitialAd.Destroy();
        RequestInterstitial();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {

    }
    */


    //handlereward:

    /*
        public void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdLoaded event received");
        }

        public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
        {
            MonoBehaviour.print(
                "HandleRewardedAdFailedToLoad event received with message: "
                                 + args.Message);
        }

        public void HandleRewardedAdOpening(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdOpening event received");
        }

        public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            MonoBehaviour.print(
                "HandleRewardedAdFailedToShow event received with message: "
                                 + args.Message);
        }

        public void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdClosed event received");
            RequestRewardBasedVideo();
            CBPData.adTime = CBPData.adTime.Add(DateTime.Now.Subtract(CBPData.adstartTime));
            CBPData.InterstitialTime = Time.time;
        }

        public void HandleUserEarnedReward(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            MonoBehaviour.print(
                "HandleRewardedAdRewarded event received for "
                            + amount.ToString() + " " + type);
            CBPData.isHint = true;
            if (CBPData.SelectedMapColor == 0)
            {
                GameObject.Find("HintDescription").GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
            else
            {
                GameObject.Find("HintDescription").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
            }
            //GameManager.Instance.AddCoin(coinToAdd);
            //var hint = PlayerPrefs.GetInt("MONEY", 0);

            //hint += 5;
            //PlayerPrefs.SetInt(GlobalValue.Coins, coins);
            //PlayerPrefs.SetInt("MONEY", money);
            // GameManager.Instance.AddHints();
            //PlayerPrefs.Save();
        }

    */
    public void OnAppStateChanged(AppState state)
    {
        // Display the app open ad when the app is foregrounded.
        UnityEngine.Debug.Log("App State is " + state);

        // OnAppStateChanged is not guaranteed to execute on the Unity UI thread.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if (state == AppState.Foreground)
            {
               // ShowAppOpenAd();
            }
        });
    }








    //request Inter


    public void RequestInterstitial()
    {
        PrintStatus("Requesting Interstitial ad.");

#if UNITY_ANDROID
        string adUnitId = inter_adid_android;
#elif UNITY_IPHONE
        string adUnitId = inter_adid_ios;
#else
        string adUnitId = "unexpected_platform";
#endif
        // Clean up interstitial before using it
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        // Initialize an InterstitialAd.
        interstitialAd = new InterstitialAd(adUnitId);

        // Add Event Handlers
        interstitialAd.OnAdLoaded += (sender, args) =>
        {
            PrintStatus("Interstitial ad loaded.");
            OnAdLoadedEvent.Invoke();
        };
        interstitialAd.OnAdFailedToLoad += (sender, args) =>
        {
            PrintStatus("Interstitial ad failed to load with error: " + args.LoadAdError.GetMessage());
            OnAdFailedToLoadEvent.Invoke();
        };
        interstitialAd.OnAdOpening += (sender, args) =>
        {
            PrintStatus("Interstitial ad opening.");
            OnAdOpeningEvent.Invoke();
        };
        interstitialAd.OnAdClosed += (sender, args) =>
        {
            PrintStatus("Interstitial ad closed.");
            interstitialAd.Destroy();
            RequestInterstitial();
            OnAdClosedEvent.Invoke();
        };
        interstitialAd.OnAdDidRecordImpression += (sender, args) =>
        {
            PrintStatus("Interstitial ad recorded an impression.");
        };
        interstitialAd.OnAdFailedToShow += (sender, args) =>
        {
            PrintStatus("Interstitial ad failed to show.");
        };
        interstitialAd.OnPaidEvent += (sender, args) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "Interstitial ad received a paid event.",
                                        args.AdValue.CurrencyCode,
                                        args.AdValue.Value);
            PrintStatus(msg);
        };

        // Load an interstitial ad
        interstitialAd.LoadAd(CreateAdRequest());


        // Create an empty ad request.
      //  AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        //interstitialAd.LoadAd(request);


        Debug.Log("Leule");
    }

    //showinter

    public void ShowAds()
    {

        if (interstitialAd != null && interstitialAd.IsLoaded())
        {
            timer -= 1;
            if (timer <= 0)
            {
                interstitialAd.Show();
                Debug.Log("Showroi");
                timer = timerset;
            }
            else
            {
               // timer -= 1;
            }
            PlayerPrefs.SetInt("xtimer", timer);
        }
        else
        {
            PrintStatus("Interstitial ad is not ready yet.");
        }

       

    }

    //requestreward:

    public void RequestRewardBasedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = reward_adid_android;
#elif UNITY_IPHONE
        string adUnitId = reward_adid_ios;
#else
            string adUnitId = "unexpected_platform";
#endif
        // create new rewarded ad instance
        rewardedAd = new RewardedAd(adUnitId);

        // Add Event Handlers
        rewardedAd.OnAdLoaded += (sender, args) =>
        {
            //PrintStatus("Reward ad loaded.");
            OnAdLoadedEvent.Invoke();
        };
        rewardedAd.OnAdFailedToLoad += (sender, args) =>
        {
            //PrintStatus("Reward ad failed to load.");
            OnAdFailedToLoadEvent.Invoke();
        };
        rewardedAd.OnAdOpening += (sender, args) =>
        {
           // PrintStatus("Reward ad opening.");
            OnAdOpeningEvent.Invoke();
        };
        rewardedAd.OnAdFailedToShow += (sender, args) =>
        {
           // PrintStatus("Reward ad failed to show with error: " + args.AdError.GetMessage());
            OnAdFailedToShowEvent.Invoke();
        };
        rewardedAd.OnAdClosed += (sender, args) =>
        {
            // PrintStatus("Reward ad closed.");
           
            RequestRewardBasedVideo();
            OnAdClosedEvent.Invoke();
        };
        rewardedAd.OnUserEarnedReward += (sender, args) =>
        {
            // PrintStatus("User earned Reward ad reward: " + args.Amount);
            CurrencyController.CreditBalance(5);

            OnUserEarnedRewardEvent.Invoke();
        };
        rewardedAd.OnAdDidRecordImpression += (sender, args) =>
        {
           // PrintStatus("Reward ad recorded an impression.");
        };
        rewardedAd.OnPaidEvent += (sender, args) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "Rewarded ad received a paid event.",
                                        args.AdValue.CurrencyCode,
                                        args.AdValue.Value);
            
            // PrintStatus(msg);
        };

        // Create empty ad request
        rewardedAd.LoadAd(CreateAdRequest());
        // Create an empty ad request.
      //  AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
       // this.rewardedAd.LoadAd(request);
    }

    //rewardshow:


    public void RewardShow()


    {
        if (rewardedAd != null)
        {
            rewardedAd.Show();
        }
        else
        {
           // PrintStatus("Rewarded ad is not ready yet.");
        }
    }

    private void PrintStatus(string message)
    {
        Debug.Log(message);
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            //statusText.text = message;
        });
    }

}
