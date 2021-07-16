using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tries : MonoBehaviour
{
    [SerializeField] private Aviaries _aviaries;
    [SerializeField] private Game _game;
    [SerializeField] private int _adBuyAmount;

    private int _tries;
    private bool _usedAd = false;

    public int AdBuyAmount => _adBuyAmount;
    public bool UsedAd => _usedAd;

    public event UnityAction<int> TriesChanged;

    private void OnEnable()
    {
        _game.LevelStarted += ResetTries;
        _aviaries.Interacted += Try;
    }

    private void OnDisable()
    {
        _game.LevelStarted -= ResetTries;
        _aviaries.Interacted -= Try;
    }

    public void OnAddWatched()
    {
        _tries += _adBuyAmount;
        TriesChanged?.Invoke(_tries);
        _usedAd = true;
    }

    private void Try()
    {
        _tries--;
        TriesChanged?.Invoke(_tries);
    }

    private void ResetTries(int level, LevelType type)
    {
        int rows = 1 + ((level - 1) % 4 + 1) * 2;
        _tries = rows * 2;
        TriesChanged?.Invoke(_tries);
    }
}
