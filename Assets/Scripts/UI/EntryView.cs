using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntryView : MonoBehaviour
{
    [SerializeField] private TMP_Text _rank;
    [SerializeField] private TMP_Text _nickname;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private Image _background;

    public TMP_Text RankText => _rank;
    public TMP_Text NicknameText => _nickname;
    public TMP_Text ScoreText => _score;
    public Image Background => _background;

    public void Init(string rank, string nickname, string score)
    {
        _rank.text = rank;
        _nickname.text = nickname;
        _score.text = score;
    }

}
