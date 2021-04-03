using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Net : MonoBehaviour
{
    [SerializeField] private Node _prefab;
    [SerializeField] private float _distance = 3;
    [SerializeField] private int _radius = 5;
    [SerializeField] private AnimalSpawner _spawner;

    private List<Node> _nodes = new List<Node>();
    private List<Node> _selectedNodes = new List<Node>();

    public event UnityAction Selected;
    public event UnityAction Deselected;
    public event UnityAction<int> AnimalsChanged;

    private void Start()
    {
        //for (int i = 0; i < _radius; i++)
        //    SpawnCircle(i, i == _radius - 1);

        SpawnGrid(_radius);

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

                if (nearAnimals.Count > 0)
                    Select(nearAnimals);
                else
                    Deselect();
            }
            else if (hit.transform.TryGetComponent(out Aviary aviary))
            {
                if (CanMove(_selectedNodes))
                {
                    aviary.TakeGroup(_selectedNodes);
                    StartCoroutine(CalcMoveAfter(_selectedNodes.Count * 0.1f));
                    Deselect();

                    AnimalsChanged?.Invoke(GetAnimalsCount());
                }
                else
                {
                    foreach (Node item in _selectedNodes)
                        item.Animal.Shake();
                }
            }
        }
    }

    private int GetAnimalsCount()
    {
        int count = 0;
        foreach (var item in _nodes)
            if (item.IsBusy)
                count++;

        return count;
    }

    private void Select(List<Node> nodes)
    {
        _selectedNodes = nodes;

        foreach (Node item in _nodes)
            if (nodes.Contains(item))
                item.Select();
            else
                item.Deselect();

        Selected?.Invoke();
    }

    private void Deselect()
    {
        _selectedNodes.Clear();
        foreach (Node item in _nodes)
                item.Deselect();

        Deselected?.Invoke();
    }

    private bool CanMove(List<Node> nodes)
    {
        foreach (Node node in nodes)
            if (node.OnEdge || node.Connected.FirstOrDefault(item => item.IsBusy == false) != null)
                return true;

        return false;
    }

    private IEnumerator CalcMoveAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        CalcMove();
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

    private void SpawnGrid(int rows)
    {
        int midRow = rows / 2;
        int cols = 4;
        float dX = _distance;
        float dZ = Mathf.Sqrt(dX * dX - (dX * dX) / 4);
        float z0 = -(rows - 1) * dZ / 2;
        for (int row = 0; row < rows; row++)
        {
            float z = row * dZ;
            int newCols = cols + row;
            if (row > midRow)
                newCols = cols + midRow + midRow - row;

            float x0 = (newCols - 1) * -dX / 2;
            for (int col = 0; col < newCols; col++)
            {
                float x = dX * col;
                bool isEdge =  row == 0 || row == rows - 1 || col == 0 || col == newCols - 1;
                Spawn(new Vector3(x0 + x, 0, z0 + z), row * 10 + Mathf.Abs(col - newCols/2), isEdge);
            }
        }
    }

    private void Spawn(Vector3 position, int index, bool edge)
    {
        Node node = Instantiate(_prefab, transform.position + position, Quaternion.identity);
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
