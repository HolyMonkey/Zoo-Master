using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnAnimals
{

}

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private float _spawnedRatio;

    private AnimalSet _set;
    private List<Animal> _animals = new List<Animal>();
    private int[] _spawned;

    private void OnEnable()
    {
        _game.LevelStarted += ChangeAnimalSet;
    }

    private void OnDisable()
    {
        _game.LevelStarted -= ChangeAnimalSet;
    }

    public Animal Spawn(Vector3 position)
    {
        var index = NewAnimalIndex();
        Animal animal = Instantiate(_set.GetAnimalTemplate(index), position, Quaternion.LookRotation(Vector3.back, Vector3.up));
        _animals.Add(animal);
        _spawned[index]++;
        return animal;
    }

    private void ChangeAnimalSet(int level, LevelType type)
    {
        _set = type.AnimalSet;
        _spawned = new int[_set.Size];
    }

    private int NewAnimalIndex()
    {
        var animalsCount = 0;
        foreach (var count in _spawned)
            animalsCount += count;
        for (var i=0; i< _set.Size; i++)
        {
            if (animalsCount - _spawned[i] > 0)
            {
                float ratio = (float)_spawned[i] / (animalsCount/_set.Size);
                if (ratio < _spawnedRatio)
                {
                    return i;
                }
            }
        }
        return Random.Range(0, _set.Size);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log(s);
    //    }
    //}

    //private void WriteString(string data)
    //{
    //    string path = "Assets/test.json";

    //    StreamWriter writer = new StreamWriter(path, false);
    //    writer.Write(data);
    //    writer.Close();
    //}

    //private string ReadString()
    //{
    //    string path = "Assets/test.json";

    //    StreamReader reader = new StreamReader(path);
    //    string result = reader.ReadToEnd();
    //    reader.Close();

    //    return result;
    //}
}
