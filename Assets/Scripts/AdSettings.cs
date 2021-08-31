using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class AdSettings : MonoBehaviour, IInterstitialAdListener, IBannerAdListener, IRewardedVideoAdListener
{
    private const string AppKey = "07f6408b0529fe47a937f41594c0c040b08ccfb526cbf3cb";

    public event UnityAction InterstitialVideoShown;
    public event UnityAction RewardedShown;

    public bool CanShowInterstitial => Appodeal.canShow(Appodeal.INTERSTITIAL, "Interstitial") && !Appodeal.isPrecache(Appodeal.INTERSTITIAL);
    public bool CanShowRewarded => Appodeal.canShow(Appodeal.REWARDED_VIDEO, "Rewarded") && !Appodeal.isPrecache(Appodeal.REWARDED_VIDEO);

    private void Start()
    {
        int adTypes = Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM | Appodeal.REWARDED_VIDEO;
        Appodeal.initialize(AppKey, adTypes, true);

        Appodeal.setInterstitialCallbacks(this);
        Appodeal.setBannerCallbacks(this);
        Appodeal.setRewardedVideoCallbacks(this);
    }

    private void AddReportEvent(string placement, string action = null)
    {
        Dictionary<string, object> eventParameters = new Dictionary<string, object>
        {
            { "AdNetwork", "Appodeal" },
            { "Placement", placement }
        };
        if (string.IsNullOrEmpty(action) == false)
            eventParameters.Add("Action", action);

        AppMetrica.Instance.ReportEvent("ShowViewAd", eventParameters);
        eventParameters.Clear();
        AppMetrica.Instance.SendEventsBuffer();
    }

    public void ShowInterstitial()
    {
        if (CanShowInterstitial)
            Appodeal.show(Appodeal.INTERSTITIAL, "Interstitial");
    }

    public void ShowRewarded()
    {
        if (CanShowRewarded)
            Appodeal.show(Appodeal.REWARDED_VIDEO, "Rewarded");
    }

    public void ShowBanner()
    {
        Appodeal.show(Appodeal.BANNER_BOTTOM);
        AddReportEvent("default", "show BANNER_BOTTOM");
    }

    public void HideBanner()
    {
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
    }

    #region InterstitialCallbacks

    public void onInterstitialLoaded(bool isPrecache)
    {
        Debug.Log($"onInterstitialLoaded. isPrecache: {isPrecache}");
    }

    public void onInterstitialFailedToLoad()
    {
        Debug.Log("onInterstitialFailedToLoad");
    }

    public void onInterstitialShowFailed()
    {
        Debug.Log("onInterstitialShowFailed");
        InterstitialVideoShown?.Invoke();
    }

    public void onInterstitialShown()
    {
        InterstitialVideoShown?.Invoke();
        Dictionary<string, object> eventParameters = new Dictionary<string, object>
        {
            { "Type", "Interstitial" }
        };
        AppMetrica.Instance.ReportEvent("AdWatched", eventParameters);
        eventParameters.Clear();
        AppMetrica.Instance.SendEventsBuffer();
    }

    public void onInterstitialClosed()
    {
        Debug.Log("onInterstitialClosed");
        InterstitialVideoShown?.Invoke();
    }

    public void onInterstitialClicked()
    {
        Debug.Log("onInterstitialClicked");
    }

    public void onInterstitialExpired()
    {
        Debug.Log("onInterstitialExpired");
        InterstitialVideoShown?.Invoke();
    }
    #endregion

    #region BannerCallbacks
    public void onBannerLoaded(int height, bool isPrecache)
    {
        Debug.Log($"onBannerLoaded. height: {height}, isPrecache: {isPrecache}");
    }

    public void onBannerFailedToLoad()
    {
        Debug.Log("onBannerFailedToLoad");
    }

    public void onBannerShown()
    {
        Debug.Log("onBannerShown");
    }

    public void onBannerClicked()
    {
        Debug.Log("onBannerClicked");
    }

    public void onBannerExpired()
    {
        Debug.Log("onBannerExpired");
    }

    #endregion

    #region RewardedCallbacks
    public void onRewardedVideoLoaded(bool precache)
    {
        
    }

    public void onRewardedVideoFailedToLoad()
    {
        
    }

    public void onRewardedVideoShowFailed()
    {
        
    }

    public void onRewardedVideoShown()
    {
        
    }

    public void onRewardedVideoFinished(double amount, string name)
    {
        RewardedShown?.Invoke();
        Dictionary<string, object> eventParameters = new Dictionary<string, object>
        {
            { "Type", "Rewarded" }
        };
        AppMetrica.Instance.ReportEvent("AdWatched", eventParameters);
        eventParameters.Clear();
        AppMetrica.Instance.SendEventsBuffer();
    }

    public void onRewardedVideoClosed(bool finished)
    {
        
    }

    public void onRewardedVideoExpired()
    {
        
    }

    public void onRewardedVideoClicked()
    {
        
    }

    #endregion
}
