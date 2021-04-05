using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Net _net;
    [SerializeField] private TapMark[] _masks;
    [SerializeField] private ComboText _combo;
    [SerializeField] private PopupText _plusText;
    [SerializeField] private ComboText _score;
    [SerializeField] private Aviary[] _aviaries;
    [SerializeField] private PopupText _plusTextPrefab;

    private Aviary _lastAviary;

    private void OnEnable()
    {
        _net.Selected += OnSelectedAnimals;
        _net.Deselected += OnDeselectedAnimals;
        _net.AnimalsChanged += OnAnimalsChanged;
        _combo.WillDisappear += DoneCombo;
        foreach (var item in _aviaries)
            item.GotAnimal += OnGotAnimal;
    }

    private void OnDisable()
    {
        _net.Selected -= OnSelectedAnimals;
        _net.Deselected -= OnDeselectedAnimals;
        _net.AnimalsChanged -= OnAnimalsChanged;
        _combo.WillDisappear -= DoneCombo;
        foreach (var item in _aviaries)
            item.GotAnimal -= OnGotAnimal;
    }

    private void OnSelectedAnimals()
    {
        foreach (var item in _masks)
            item.Show();
    }

    private void OnDeselectedAnimals()
    {
        foreach (var item in _masks)
            item.Hide();
    }

    private void OnGotAnimal(Aviary aviary)
    {
        _lastAviary = aviary;
        _combo.transform.position = Camera.main.WorldToScreenPoint(aviary.DoorPosition + aviary.transform.forward * 0.5f);
        _combo.Increase();
    }

    private void DoneCombo(int combo)
    {
        if (combo > 1)
        {
            int score = combo * 10;
            if (_lastAviary != null)
                _plusText.Show(_lastAviary.DoorPosition + _lastAviary.transform.forward * 2f, "+" + score.ToString());

            StartCoroutine(MovePlusText(_plusText, 0.4f, score));
        }
    }

    private void OnAnimalsChanged(int count)
    {
        Debug.Log(count);
        if (count == 0)
        {
            StartCoroutine(FinishGame());
        }
    }

    private IEnumerator FinishGame()
    {
        yield return new WaitForSeconds(1.2f);

        foreach (var item in _aviaries)
        {
            int combo = item.ComboText.Value;
            if (combo > 1)
            {
                int score = combo * 10;
                item.ComboText.Reset();
                item.PlayConfetti();
                PopupText plusText = Instantiate(_plusTextPrefab, transform);
                plusText.transform.position = item.ComboText.transform.position + Vector3.up * 100;
                plusText.Show("+" + score.ToString());

                StartCoroutine(MovePlusText(plusText, 1, score));

                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator MovePlusText(PopupText plusText, float delay, int score)
    {
        yield return new WaitForSeconds(delay);

        float duration = 0.3f;
        float time = 0;
        Vector3 position = plusText.transform.position;
        Vector3 target = _score.transform.position;
        while (time < duration)
        {
            float value = Ease.EaseInEaseOut(time / duration);
            plusText.transform.position = Vector3.Lerp(position, target, value);
            yield return null;
            time += Time.deltaTime;
        }

        _score.Increase(score);
    }
}
