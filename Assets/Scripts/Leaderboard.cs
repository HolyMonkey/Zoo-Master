#pragma warning disable

using System.Collections;
using Agava.YandexGames;
using Agava.YandexGames.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private string _name = "PlaytestBoard";
    [SerializeField] private EntryView _playerInfoTemplate;
    [SerializeField] private EntryView _playerEntryView;
    [SerializeField] private Transform _container;
    [SerializeField] private GameObject _loginWindow;
    [SerializeField] private GameObject _requestDataWarning;
    [SerializeField] private GameObject _leaderboard;

    private List<EntryView> _entryViews = new List<EntryView>();

    public string Name => _name;

    private void Awake()
    {
        YandexGamesSdk.CallbackLogging = true;
    }

    private IEnumerator Start()
    {
        _leaderboard = this.gameObject;
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif
        // Always wait for it if invoking something immediately in the first scene.
        yield return YandexGamesSdk.WaitForInitialization();
    }

    private void Update()
    {
        // Mute sounds when app is running in the background.
        AudioListener.pause = WebApplication.InBackground;
    }

    public void UpdateLeaderboard()
    {

#if !UNITY_WEBGL || UNITY_EDITOR
        return;
#endif
        if (!PlayerAccount.IsAuthorized)
        {
            _loginWindow.SetActive(true);
            return;
        }

        _leaderboard.SetActive(true);
        Time.timeScale = 0;

        if(!PlayerAccount.HasPersonalProfileDataPermission)
            _requestDataWarning.SetActive(true);

        Agava.YandexGames.Leaderboard.GetEntries(_name, (result) =>
        {
            foreach (var entryView in _entryViews)
            {
                Debug.Log(entryView);
                Destroy(entryView.gameObject);
            }
            _entryViews.Clear();
            foreach (var entry in result.entries)
            {
                string name = entry.player.publicName;
                if (string.IsNullOrEmpty(name))
                    name = "Anonymous";

                EntryView entryView = Instantiate(_playerInfoTemplate, _container);
                entryView.Init(entry.rank.ToString(), name, entry.score.ToString());

                Agava.YandexGames.Leaderboard.GetPlayerEntry(_name, (playerEntry) =>
                {
                    string name = playerEntry.player.publicName;
                    if (string.IsNullOrEmpty(name))
                        name = "Anonymous";
                    _playerEntryView.Init(playerEntry.rank.ToString(), name, playerEntry.score.ToString());

                    if (entry == playerEntry)
                        entryView.Background.gameObject.SetActive(true);
                });
                _entryViews.Add(entryView);
            }
        });
    }

    public void OnRequestDataButtonDown()
    {
        PlayerAccount.RequestPersonalProfileDataPermission();
    }

    public void OnAuthorizeButtonDown()
    {
        PlayerAccount.Authorize();
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
    }
}
