using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;

public class Aviary : MonoBehaviour
{
    [SerializeField] private AviaryDoor _door;
    [SerializeField] private float _movePerAnimal = 0.1f;
    [SerializeField] private ComboText _comboText;
    [SerializeField] private Image _comboImage;
    [SerializeField] private ParticleSystem _confetti;

    private List<Animal> _animals = new List<Animal>();

    public Vector3 DoorPosition => _door.transform.position;
    public event UnityAction<Aviary> GotAnimal;
    public ComboText ComboText => _comboText;

    public void OpenDoor()
    {
        _door.Open();
    }

    public void PlayConfetti()
    {
        _confetti.Play();
    }

    public void CloseDoor(float delay)
    {
        StartCoroutine(CloseAfter(delay));
    }

    public void TakeGroup(List<Node> nodes)
    {
        List<Node> sortedNodes = nodes.OrderBy(item => Vector3.Distance(item.transform.position, transform.position)).ToList();
        List<Animal> animals = new List<Animal>();
        for (int i = 0; i < sortedNodes.Count; i++)
        {
            Node node = sortedNodes[i];
            if (node.IsBusy)
                animals.Add(node.Animal);

            node.Deselect();
            node.Clear();
        }
        OpenDoor();
        StartCoroutine(AddAnimalsLoop(animals));
    }

    private IEnumerator AddAnimalsLoop(List<Animal> animals)
    {
        foreach (var animal in animals)
        {
            MoveAnimalsBack(_movePerAnimal);

            _animals.Add(animal);

            animal.MoveToAviary(this);

            yield return new WaitForSeconds(0.1f);
        }

        CloseDoor(0.4f);
        StartCoroutine(ReactOnNewAnimals(0.4f, animals));
    }

    private void MoveAnimalsBack(float delta)
    {
        foreach (Animal animal in _animals)
            animal.MoveTo(animal.transform.position - transform.forward * delta);
    }

    private IEnumerator ReactOnNewAnimals(float delay, List<Animal> newAnimals)
    {
        yield return new WaitForSeconds(delay);

        if (newAnimals.Count != _animals.Count)
        {
            int newAnimalsID = newAnimals[0].ID;
            if (_animals.Where(item => item.ID == newAnimalsID).ToArray().Length == _animals.Count)
            {
                string animation = newAnimals.Count > 3 ? "spin" : "bounce";
                foreach (Animal animal in _animals)
                    animal.PlayAnimation(animation);
            }
            else
            {
                foreach (Animal animal in _animals)
                    animal.PlayAnimation("fear");
            }
        }
    }

    private IEnumerator CloseAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _door.Close();
    }

    private List<Animal> _countedAnimals = new List<Animal>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NavMeshAgent agent))
            agent.enabled = true;

        if (other.TryGetComponent(out Animal animal))
        {
            if (_countedAnimals.Contains(animal) == false)
            {
                if (_countedAnimals.Count == 0 || _countedAnimals[_countedAnimals.Count - 1].ID == animal.ID)
                {
                    _comboImage.color = animal.CountColor;
                    _comboText.Increase();
                }
                else
                {
                    _comboText.QuickReset();
                    _comboText.Increase();
                }

                _countedAnimals.Add(animal);
                GotAnimal?.Invoke(this);
            }
        }
    }

    private int GetSameAnimalsInRowCount()
    {
        int count = 0;
        int prevID = -1;
        foreach (var item in _animals)
        {
            if (item.ID == prevID)
                count++;
            else
                count = 0;

            prevID = item.ID;
        }

        return count;
    }
}
