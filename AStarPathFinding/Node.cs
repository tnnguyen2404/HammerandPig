using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Walkable,
    JumpPoint,
    DropPoint
}

public class Node : IHeapItem<Node>
{
    public NodeType type;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    public bool jumpNode;

    public List<Node> walkNeighbours;
    public List<Node> jumpNeighbours;

    int heapIndex;

    public Node(NodeType _type, Vector3 _worldPos, int _gridX, int _gridY)
    {
        type = _type;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;

        walkNeighbours = new List<Node>();
        jumpNeighbours = new List<Node>();
    }

    public int fCost => gCost + hCost;

    public int HeapIndex
    {
        get => heapIndex;
        set => heapIndex = value;
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare; // Min-heap: lower fCost comes first
    }
}
