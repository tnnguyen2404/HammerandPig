using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public Node jumpTarget;
    public List<Node> connections = new List<Node>();
    
    public float gScore;
    public float hScore;

    public bool isJumpPoint;
    public float FScore() => gScore + hScore;

    void OnDrawGizmos()
    {
        // Draw walk connections in green
        Gizmos.color = Color.green;
        foreach (var n in connections)
        {
            if (n != null && Mathf.Approximately(n.transform.position.y, transform.position.y))
                Gizmos.DrawLine(transform.position, n.transform.position);
        }
        // Draw vertical (jump/fall) connections in red
        Gizmos.color = Color.red;
        foreach (var n in connections)
        {
            if (n != null && !Mathf.Approximately(n.transform.position.y, transform.position.y))
                Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }

}
