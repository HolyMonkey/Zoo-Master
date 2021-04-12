using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB
{
    private static readonly string LevelKey = "level";

    public static int GetLevel() => PlayerPrefs.GetInt(LevelKey) + 1;

    public static void IncreaseLevel() => PlayerPrefs.SetInt(LevelKey, PlayerPrefs.GetInt(LevelKey) + 1);

    public static void ResetLevel() => PlayerPrefs.SetInt(LevelKey, 0);
}
