using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private Net _net;
    [SerializeField] private TapMark[] _masks;
    [SerializeField] private ComboText _combo;
    [SerializeField] private PopupText _plusText;
    [SerializeField] private ComboText _score;
    [SerializeField] private Aviary[] _aviaries;
    [SerializeField] private PopupText _plusTextPrefab;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private HandPointer _pointer;
    [SerializeField] private LevelDoneScreen _doneScreen;
    [SerializeField] private ParticleSystem[] _finishEffects;

    private Aviary _lastAviary;
    private int _level;
    private const int _levelsPerScene = 4;

    private void Awake()
    {
        _doneScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        _level = DB.GetLevel();
        int rows = 1 + ((_level - 1) % _levelsPerScene + 1) * 2;
        _net.BuildLevel(9);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DB.ResetLevel();
            Debug.Log("Reset level!");
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(FinishGame());
        }
    }

    private void OnEnable()
    {
        _net.Selected += OnSelectedAnimals;
        _net.Deselected += OnDeselectedAnimals;
        _net.AnimalsChanged += OnAnimalsChanged;
        _net.GoodClick += OnClickGood;
        _net.BadClick += OnClickBad;
        _combo.WillDisappear += DoneCombo;
        _doneScreen.NextButtonClicked += LoadNextLevel;
        foreach (var item in _aviaries)
        {
            item.GotAnimal += OnGotAnimal;
            item.NiceMove += ShowNice;
            item.VeryNiceMove += ShowVeryNice;
            item.BadMove += ShowBad;
        }
    }

    private void OnDisable()
    {
        _net.Selected -= OnSelectedAnimals;
        _net.Deselected -= OnDeselectedAnimals;
        _net.AnimalsChanged -= OnAnimalsChanged;
        _net.GoodClick -= OnClickGood;
        _net.BadClick -= OnClickBad;
        _combo.WillDisappear -= DoneCombo;
        _doneScreen.NextButtonClicked -= LoadNextLevel;
        foreach (var item in _aviaries)
        {
            item.GotAnimal -= OnGotAnimal;
            item.NiceMove -= ShowNice;
            item.VeryNiceMove -= ShowVeryNice;
            item.BadMove -= ShowBad;
        }
    }

    private void ShowNice() => _pointer.Play("ok");

    private void ShowVeryNice() => _pointer.Play("thumbUp");

    private void ShowBad() => _pointer.Play("angry");

    private void OnClickGood() => _pointer.ResetAngry();

    private void OnClickBad() => _pointer.AddAngry();

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

        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _combo.transform.position = Camera.main.WorldToScreenPoint(aviary.DoorPosition + aviary.transform.forward * 1.5f);
        }
        else
        {
            Vector3 worldSpacePosition = aviary.DoorPosition + aviary.transform.forward * 2.5f + aviary.transform.up * 4;
            _combo.transform.position = worldSpacePosition;
        }
        
        _combo.Increase();
    }

    private void DoneCombo(int combo)
    {
        if (combo > 1)
        {
            int score = combo * 10 + combo * (combo + 3) / 2;
            if (_lastAviary != null)
            {
                if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    _plusText.transform.position = Camera.main.WorldToScreenPoint(_lastAviary.DoorPosition + _lastAviary.transform.forward * 2.5f);
                else
                    _plusText.transform.position = _lastAviary.DoorPosition + _lastAviary.transform.forward * 3.5f + _lastAviary.transform.up * 4.5f;

                _plusText.Show("+" + score.ToString());
            }
            StartCoroutine(MovePlusText(_plusText, 0.4f, score));
        }
    }

    private void OnAnimalsChanged(int count)
    {
        if (count == 0)
            StartCoroutine(FinishGame());
    }

    private IEnumerator FinishGame()
    {
        yield return new WaitForSeconds(1f);

        //foreach (var item in _aviaries)
        //{
        //    int combo = item.ComboText.Value;
        //    if (combo > 1)
        //    {
        //        int score = combo * 10;
        //        item.ComboText.Reset();
        //        item.PlayConfetti();
        //        PopupText plusText = Instantiate(_plusTextPrefab, transform);
        //        plusText.transform.position = item.ComboText.transform.position + Vector3.up * 100;
        //        plusText.Show("+" + score.ToString());

        //        StartCoroutine(MovePlusText(plusText, 1, score));

        //        yield return new WaitForSeconds(0.2f);
        //    }
        //}

        foreach (var item in _finishEffects)
            item.Play();

        yield return new WaitForSeconds(0.1f);
        _doneScreen.Appear(_score.Value, _level);
        DB.AddScore(_score.Value);
        DB.IncreaseLevel();
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

    private void LoadNextLevel()
    {
        int sceneIndex = (DB.GetLevel() - 1) / _levelsPerScene;
        sceneIndex %= SceneManager.sceneCountInBuildSettings;

        SceneManager.LoadScene(sceneIndex);
    }
}
