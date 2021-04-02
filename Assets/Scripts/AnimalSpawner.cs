using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] private Animal[] _prefabs;

    private List<Animal> _animals = new List<Animal>();

    public Animal Spawn(Vector3 position)
    {
        Animal animal = Instantiate(_prefabs[Random.Range(0, _prefabs.Length)], position, Quaternion.LookRotation(Vector3.back, Vector3.up));
        _animals.Add(animal);
        return animal;
    }
}
