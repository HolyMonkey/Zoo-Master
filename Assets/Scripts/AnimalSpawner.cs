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

    private AnimalSet _set;
    private List<Animal> _animals = new List<Animal>();
    private List<int> _indices = new List<int>();

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
        int index = Random.Range(0, _set.Size);
        Animal animal = Instantiate(_set.GetAnimalTemplate(index), position, Quaternion.LookRotation(Vector3.back, Vector3.up));
        //_indices.Add(index);
        _animals.Add(animal);
        return animal;
    }

    private void ChangeAnimalSet(int level, LevelType type)
    {
        _set = type.AnimalSet;
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
