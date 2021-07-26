using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private Game _game;

    private void OnEnable()
    {
        _game.LevelCompleted += Play;
    }

    private void OnDisable()
    {
        _game.LevelCompleted -= Play;
    }

    private void Play()
    {
        _audio.Play();
    }
}
