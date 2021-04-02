using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Aviary : MonoBehaviour
{
    [SerializeField] private AviaryDoor _door;
    [SerializeField] private float _movePerAnimal = 0.1f;

    private List<Animal> _animals = new List<Animal>();

    public void OpenDoor()
    {
        _door.Open();
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
            animal.Go(transform.position, 0.5f);

            yield return new WaitForSeconds(0.1f);
        }

        CloseDoor(0.4f);
    }

    private void MoveAnimalsBack(float delta)
    {
        foreach (Animal animal in _animals)
            animal.MoveTo(animal.transform.position - transform.forward * delta);
    }

    private IEnumerator CloseAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _door.Close();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NavMeshAgent agent))
        {
            agent.enabled = true;
        }
    }
}
