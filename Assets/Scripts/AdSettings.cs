using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class AdSettings : MonoBehaviour, IInterstitialAdListener, IBannerAdListener
{
    private const string AppKey = "07f6408b0529fe47a937f41594c0c040b08ccfb526cbf3cb";

    public event UnityAction InterstitialVideoShown;

    private void Start()
    {
        int adTypes = Appodeal.INTERSTITIAL | Appodeal.BANNER_BOTTOM;
        Appodeal.initialize(AppKey, adTypes, true);

        Appodeal.setInterstitialCallbacks(this);
        Appodeal.setBannerCallbacks(this);
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
    }

    public void ShowInterstitial()
    {
        if (Appodeal.canShow(Appodeal.INTERSTITIAL, "Levels") && !Appodeal.isPrecache(Appodeal.INTERSTITIAL))
            Appodeal.show(Appodeal.INTERSTITIAL, "Levels");
        else
            InterstitialVideoShown?.Invoke();
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
        AddReportEvent("Levels", "interstitial video shown");
    }

    public void onInterstitialClosed()
    {
        Debug.Log("onInterstitialClosed");
        InterstitialVideoShown?.Invoke();
    }

    public void onInterstitialClicked()
    {
        Debug.Log("onInterstitialClicked");
        AddReportEvent("Levels", "interstitial video clicked");
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
}
