using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tries : MonoBehaviour
{
    [SerializeField] private Aviaries _aviaries;
    [SerializeField] private Game _game;
    [SerializeField] private int _adBuyAmount;
    [SerializeField] private AdSettings _adSettings;

    private int _tries;
    private bool _usedAd = false;
    private bool _AdActive;

    public int AdBuyAmount => _adBuyAmount;
    public bool UsedAd => _usedAd;

    public event UnityAction<int> TriesChanged;

    private void OnEnable()
    {
        _game.LevelStarted += ResetTries;
        _aviaries.Interacted += Try;
        _adSettings.InterstitialVideoShown += OnAddWatched;
    }

    private void OnDisable()
    {
        _game.LevelStarted -= ResetTries;
        _aviaries.Interacted -= Try;
        _adSettings.InterstitialVideoShown -= OnAddWatched;
    }

    private void OnAddWatched()
    {
        if (_AdActive == false)
            return;
        _tries += _adBuyAmount;
        TriesChanged?.Invoke(_tries);
        _usedAd = true;
        _AdActive = false;
    }

    private void Try()
    {
        _tries--;
        TriesChanged?.Invoke(_tries);
        if (_tries == 0 && _AdActive == false)
        {
            int level = DB.GetLevel();
            Dictionary<string, object> eventParameters = new Dictionary<string, object>
            {
                { "Level number",  level},
            };

            AppMetrica.Instance.ReportEvent("Lose", eventParameters);
            eventParameters.Clear();
        }
    }

    private void ResetTries(int level, LevelType type)
    {
        int rows = 1 + ((level - 1) % 4 + 1) * 2;
        _tries = rows * 2;
        if (DB.GetLevel() == 1)
            _tries = 9;
        TriesChanged?.Invoke(_tries);
    }

    public void StartAD()
    {
        _AdActive = true;
        _adSettings.ShowInterstitial();
    }
}
