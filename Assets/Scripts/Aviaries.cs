using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Aviaries : MonoBehaviour
{
    [SerializeField] private Aviary[] _aviaries;

    public event UnityAction<List<Animal>> ReleasedAnimals;

    private void OnEnable()
    {
        foreach (var item in _aviaries)
            item.ReleasedAnimals += OnReleasedAnimals;
    }

    private void OnDisable()
    {
        foreach (var item in _aviaries)
            item.ReleasedAnimals -= OnReleasedAnimals;
    }

    private void OnReleasedAnimals(List<Animal> animals)
    {
        ReleasedAnimals?.Invoke(animals);
    }
}
