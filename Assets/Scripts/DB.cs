using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB
{
    private static readonly string LevelKey = "level";
    private static readonly string ScoreKey = "score";

    public static int GetLevel() => PlayerPrefs.GetInt(LevelKey) + 1;

    public static void IncreaseLevel() => PlayerPrefs.SetInt(LevelKey, PlayerPrefs.GetInt(LevelKey) + 1);

    public static void ResetLevel() => PlayerPrefs.SetInt(LevelKey, 0);

    public static int GetScore() => PlayerPrefs.GetInt(ScoreKey) + 1;

    public static void AddScore(int value) => PlayerPrefs.SetInt(ScoreKey, PlayerPrefs.GetInt(ScoreKey) + value);

    public static void ResetScore() => PlayerPrefs.SetInt(ScoreKey, 0);
}
