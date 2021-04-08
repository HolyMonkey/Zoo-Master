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

    private Stack<Animal>  _animals = new Stack<Animal>();

    public Vector3 DoorPosition => _door.transform.position;
    public ComboText ComboText => _comboText;

    public event UnityAction<Aviary> GotAnimal;
    public event UnityAction<List<Animal>> ReleasedAnimals;
    public event UnityAction NiceMove;
    public event UnityAction VeryNiceMove;
    public event UnityAction BadMove;

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
        bool sameAnimals = _animals.Count == 0 || animals[0].ID == _animals.Peek().ID;
        foreach (var animal in animals)
        {
            MoveAnimalsBack(_movePerAnimal);

            _animals.Push(animal);
            animal.MoveToAviary(this);
            StartCoroutine(UpdateCounter(GetSameAnimalsInRowCount(), animal.CountColor, 0.3f, sameAnimals));

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

    private void MoveAnimalsForward()
    {
        float minDistance = 1000;
        foreach (Animal animal in _animals)
        {
            float distance = Vector3.Distance(animal.transform.position, transform.position);
            if (distance < minDistance)
                minDistance = distance;
        }

        foreach (Animal animal in _animals)
            animal.MoveTo(animal.transform.position + transform.forward * minDistance);
    }

    private IEnumerator UpdateCounter(int value, Color color, float delay, bool sameAnimals)
    {
        yield return new WaitForSeconds(delay);

        _comboImage.color = color;
        _comboText.QuickReset();
        _comboText.Increase(value);
        if (sameAnimals)
            GotAnimal?.Invoke(this);
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

                if (newAnimals.Count > 4)
                    VeryNiceMove?.Invoke();
                else if (newAnimals.Count > 2)
                    NiceMove?.Invoke();
            }
            else
            {
                foreach (Animal animal in _animals)
                    animal.PlayAnimation("fear");

                int count = GetSameAnimalsInRowCount();
                ReleaseAnimals(count);

                BadMove?.Invoke();
            }
        }
        else
        {
            if (newAnimals.Count > 4)
                VeryNiceMove?.Invoke();
            else if (newAnimals.Count > 2)
                NiceMove?.Invoke();
        }
    }

    private IEnumerator CloseAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _door.Close();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Animal animal))
            animal.EnableNavAgent();
    }

    private int GetSameAnimalsInRowCount()
    {
        int count = 0;
        int prevID = _animals.Peek().ID;
        foreach (var item in _animals)
        {
            if (item.ID == prevID)
                count++;
            else
                break;
        }

        return count;
    }

    private void ReleaseAnimals(int count)
    {
        List<Animal> animals = new List<Animal>();
        for (int i = 0; i < count; i++)
        {
            Animal animal = _animals.Pop();
            animal.DisableNavAgent();
            animals.Add(animal);
        }

        OpenDoor();
        CloseDoor(1f);
        MoveAnimalsForward();
        StartCoroutine(UpdateCounter(GetSameAnimalsInRowCount(), _animals.Peek().CountColor, 0.2f, false));

        ReleasedAnimals?.Invoke(animals);
    }
}
