using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] private Animal[] _prefabs;

    private List<Animal> _animals = new List<Animal>();
    private List<int> _indices = new List<int>();

    public Animal Spawn(Vector3 position)
    {
        int index = Random.Range(0, _prefabs.Length);
        Animal animal = Instantiate(_prefabs[index], position, Quaternion.LookRotation(Vector3.back, Vector3.up));

        _indices.Add(index);
        _animals.Add(animal);
        return animal;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        string s = "[";
    //        for (int i = 0; i < _indices.Count; i++)
    //        {
    //            s += _indices[i] + ", ";
    //        }

    //        s += "]";
    //        Debug.Log(s);
    //    }
    //}
}
