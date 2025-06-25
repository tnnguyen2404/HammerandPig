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
        Gizmos.color = Color.blue;
        foreach (var n in connections)
        {
            if (n != null)
                Gizmos.DrawLine(transform.position, n.transform.position);
        }
        // Draw jump target in red
        if (isJumpPoint && jumpTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, jumpTarget.transform.position);
            Gizmos.DrawSphere(jumpTarget.transform.position, 0.1f);
        }
    }
}
