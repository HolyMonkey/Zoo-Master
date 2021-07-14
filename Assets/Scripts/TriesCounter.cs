using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriesCounter : MonoBehaviour
{
    [SerializeField] private Aviaries _aviaries;
    [SerializeField] private Game _game;

    private int _tries = 0;

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

    private void Try()
    {
        _tries++;
        TriesChanged?.Invoke(_tries);
    }

    private void ResetTries(int level, LevelType type)
    {
        _tries = 0;
    }
}
