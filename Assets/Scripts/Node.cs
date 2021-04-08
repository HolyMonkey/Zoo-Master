using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
    private int _index;
    private bool _onEdge = false;
    private Node[] _connectedNodes;
    private Animal _animal;

    public int Index => _index;
    public bool IsBusy => _animal != null;
    public bool OnEdge => _onEdge;
    public Animal Animal => _animal;
    public Node[] Connected => _connectedNodes;
    public int Row { get; private set; } = 0;

    public void Init(int index, int row, bool edge)
    {
        _index = index;
        _onEdge = edge;
        Row = row;
    }

    public void SetConnected(Node[] nodes)
    {
        _connectedNodes = nodes;
    }

    public void MakeBusy(Animal animal, float delay, bool fromAviary)
    {
        _animal = animal;
        if (fromAviary)
        {
            _animal.MoveFromAviary(transform.position);
        }
        else
        {
            _animal.Go(transform.position, 0.5f, delay);
        }
    }

    public void Clear()
    {
        _animal = null;
    }

    public bool TryGetPreferedNode(out Node prefered)
    {
        int min = _index;
        prefered = null;
        foreach (Node node in _connectedNodes)
        {
            if (!node.IsBusy && node.Index < min)
            {
                min = node.Index;
                prefered = node;
            }
        }

        return prefered != null;
    }

    public bool TryGetFarNode(out Node farNode)
    {
        int max = _index;
        farNode = null;
        foreach (Node node in _connectedNodes)
        {
            if (!node.IsBusy && node.Index > max)
            {
                max = node.Index;
                farNode = node;
            }
        }

        return farNode != null;
    }

    public void Select()
    {
        _animal?.Select();
    }

    public void Deselect()
    {
        _animal?.Unselect();
    }
}
