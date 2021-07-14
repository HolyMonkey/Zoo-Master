using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriesView : MonoBehaviour
{
    [SerializeField] private TriesCounter _counter;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private string _baseText;

    private void OnEnable()
    {
        _counter.TriesChanged += OnCountChanged;
    }

    private void OnDisable()
    {
        _counter.TriesChanged -= OnCountChanged;
    }

    private void OnCountChanged(int count)
    {
        _text.text = _baseText + count.ToString();
    }
}
