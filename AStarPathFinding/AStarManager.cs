using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue;
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            int lowestF = default;

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }
            
            Node curNode = openSet[lowestF];
            openSet.Remove(curNode);

            if (curNode == end)
            {
                List<Node> path = new List<Node>();
                
                path.Insert(0, end);

                while (curNode != start)
                {
                    curNode = curNode.cameFrom;
                    path.Add(curNode);
                }
                
                path.Reverse();
                return path;
            }

            foreach (Node connectedNode in curNode.connections)
            {
                float heldGScore = curNode.gScore + Vector2.Distance(curNode.transform.position, connectedNode.transform.position);

                if (heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = curNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }

            if (curNode.isJumpPoint && curNode.jumpTarget != null)
            {
                Node jumpNode = curNode.jumpTarget;
                
                float jumpCost = Vector2.Distance(curNode.transform.position, jumpNode.transform.position);
                float heldGScore = curNode.gScore + jumpCost;

                if (heldGScore < jumpNode.gScore)
                {
                    jumpNode.gScore = heldGScore;
                    jumpNode.cameFrom = curNode;
                    jumpNode.hScore = Vector2.Distance(jumpNode.transform.position, end.transform.position);

                    if (!openSet.Contains(jumpNode))
                    {
                        openSet.Add(jumpNode);
                    }
                }
            }
        }
        return null;
    }
}
