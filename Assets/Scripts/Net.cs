using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Net : MonoBehaviour
{
    [SerializeField] private Node _prefab;
    [SerializeField] private float _distance = 3;
    [SerializeField] private int _radius = 5;
    [SerializeField] private AnimalSpawner _spawner;

    private List<Node> _nodes = new List<Node>();
    private List<Node> _selectedNodes = new List<Node>();

    private void Start()
    {
        for (int i = 0; i < _radius; i++)
            SpawnCircle(i, i == _radius - 1);

        foreach (Node node in _nodes)
            node.SetConnected(FindConnected(node));

        List<Node> freeNodes = new List<Node>();
        foreach (var node in _nodes)
            freeNodes.Add(node);

        foreach (var node in _nodes)
            SpawnAnimal(node);

        //for (int i = 0; i < 10; i++)
        //{
        //    int index = Random.Range(0, freeNodes.Count);
        //    SpawnAnimal(freeNodes[index]);
        //    freeNodes.RemoveAt(index);
        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 1)
                Time.timeScale = 0.1f;
            else
                Time.timeScale = 1;
        }

        if (Input.GetMouseButtonDown(0) &&
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000))
        {
            if (hit.transform.TryGetComponent(out Node node))
            {
                List<Node> nearAnimals = new List<Node>() { node };
                bool needLoop = true;
                while (needLoop)
                {
                    needLoop = false;
                    List<Node> newItems = new List<Node>();
                    foreach (Node item in nearAnimals)
                    {
                        List<Node> near = GetNearNodesWithSameAnimal(item);
                        foreach (Node nearItem in near)
                        {
                            if (nearAnimals.Contains(nearItem) == false)
                            {
                                newItems.Add(nearItem);
                                needLoop = true;
                            }
                        }
                    }

                    nearAnimals.AddRange(newItems);
                }

                _selectedNodes = nearAnimals;

                foreach (Node item in _nodes)
                {
                    if (nearAnimals.Contains(item))
                        item.Select();
                    else
                        item.Deselect();
                }
            }
            else if (hit.transform.TryGetComponent(out Aviary aviary))
            {
                if (CanMove(_selectedNodes))
                {
                    aviary.TakeGroup(_selectedNodes);
                    CalcMove();
                }
                else
                {
                    foreach (Node item in _selectedNodes)
                        item.Animal.Shake();
                }
            }
        }
    }

    private bool CanMove(List<Node> nodes)
    {
        foreach (Node node in nodes)
            if (node.OnEdge || node.Connected.FirstOrDefault(item => item.IsBusy == false) != null)
                return true;

        return false;
    }

    private void CalcMove()
    {
        bool needUpdate = true;
        while (needUpdate)
        {
            needUpdate = false;
            foreach (var node in _nodes)
            {
                if (TryUpdateNode(node))
                    needUpdate = true;
            }
        }
    }

    private List<Node> GetNearNodesWithSameAnimal(Node node)
    {
        List<Node> near = new List<Node>();
        foreach (Node item in node.Connected)
        {
            if (item.IsBusy && item.Animal.ID == node.Animal.ID)
            {
                near.Add(item);
            }
        }

        return near;
    }

    private Node[] FindConnected(Node node)
    {
        return _nodes.Where(item => item != node && Vector3.Distance(node.transform.position, item.transform.position) < _distance * 1.1f).ToArray();
    }

    private void SpawnCircle(int radius, bool edge)
    {
        int rows = radius * 2 + 1;
        int cols = radius + 1;
        float dX = _distance;
        float dZ = Mathf.Sqrt(dX * dX - (dX * dX) / 4);
        float x0 = -dX * radius / 2;
        float z0 = -dZ * radius;
        for (int row = 0; row < rows; row++)
        {
            float z = row * dZ;
            if (row == 0 || row == rows - 1)
            {
                for (int col = 0; col < cols; col++)
                {
                    float x = dX * col;
                    Spawn(new Vector3(x0 + x, 0, z0 + z), radius, edge);
                }
            }
            else
            {
                int newCols = cols + row - 1;
                if (row > rows / 2)
                    newCols = rows - (row - rows / 2) - 1;

                float x = newCols * -dX / 2;
                Spawn(new Vector3(x, 0, z0 + z), radius, edge);
                Spawn(new Vector3(x + newCols * dX, 0, z0 + z), radius, edge);
            }
        }
    }

    private void Spawn(Vector3 position, int index, bool edge)
    {
        Node node = Instantiate(_prefab, position, Quaternion.identity);
        node.Init(index, edge);
        _nodes.Add(node);
    }

    private void SpawnAnimal(Node node)
    {
        node.MakeBusy(_spawner.Spawn(node.transform.position));
    }

    private bool TryUpdateNode(Node node)
    {
        if (node.IsBusy && node.TryGetPreferedNode(out Node prefered))
        {
            Animal animal = node.Animal;
            node.Clear();
            prefered.MakeBusy(animal);
            return true;
        }

        return false;
    }
}
