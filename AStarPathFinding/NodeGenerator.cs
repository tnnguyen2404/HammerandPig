using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeGenerator : MonoBehaviour
{
    public LayerMask groundLayer;
    
    [Header("Tilemaps")]
    public Tilemap groundTilemap;         
    public Tilemap oneWayTilemap; 

    [Header("Node Settings")]
    public GameObject nodePrefab;           
    public float nodeSpacing = 1f;          
    public float jumpHeight = 3f;           
    public float jumpDistance = 3f;         
    
    [ContextMenu("Generate Node")]

    void GenerateNodes()
    {
        ClearNodes();

        List<Node> nodes = new List<Node>();
        
        BoundsInt bounds = groundTilemap.cellBounds;
        if (oneWayTilemap != null)
        {
            bounds.xMin = Mathf.Min(bounds.xMin, oneWayTilemap.cellBounds.xMin);
            bounds.xMax = Mathf.Max(bounds.xMax, oneWayTilemap.cellBounds.xMax);
            bounds.yMin = Mathf.Min(bounds.yMin, oneWayTilemap.cellBounds.yMin);
            bounds.yMax = Mathf.Max(bounds.yMax, oneWayTilemap.cellBounds.yMax);
        }
        
        for (int y = bounds.yMin; y <= bounds.yMax; y++)
{
    for (int x = bounds.xMin; x <= bounds.xMax; x++)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        bool isGround = groundTilemap.HasTile(pos);
        bool isOneWay = oneWayTilemap != null && oneWayTilemap.HasTile(pos);

        bool airAbove =
            (!groundTilemap.HasTile(new Vector3Int(pos.x, pos.y + 1, pos.z))) &&
            (oneWayTilemap == null || !oneWayTilemap.HasTile(new Vector3Int(pos.x, pos.y + 1, pos.z)));

        if ((isGround || isOneWay) && airAbove)
        {
            Vector3 worldPos = groundTilemap.CellToWorld(pos) + new Vector3(0.5f, 1f, 0f);

            // Edge check: left or right is empty
            bool leftEmpty = !(groundTilemap.HasTile(new Vector3Int(x - 1, y, 0)) ||
                              (oneWayTilemap != null && oneWayTilemap.HasTile(new Vector3Int(x - 1, y, 0))));
            bool rightEmpty = !(groundTilemap.HasTile(new Vector3Int(x + 1, y, 0)) ||
                              (oneWayTilemap != null && oneWayTilemap.HasTile(new Vector3Int(x + 1, y, 0))));

            // Check if a node is already placed at this position
            bool nodeExists = false;
            foreach (var node in nodes)
            {
                if (Vector2.Distance(node.transform.position, worldPos) < 0.2f)
                {
                    nodeExists = true;
                    break;
                }
            }

            bool placeNode = false;

            // Always place at edges
            if (leftEmpty || rightEmpty)
                placeNode = true;

            // Also place at regular interval, but don't duplicate
            if (!placeNode && (x - bounds.xMin) % Mathf.RoundToInt(nodeSpacing) == 0)
                placeNode = true;

            if (placeNode && !nodeExists)
            {
                GameObject nodeGO = Instantiate(nodePrefab, worldPos, Quaternion.identity, this.transform);
                Node node = nodeGO.GetComponent<Node>();
                nodes.Add(node);
            }
        }
    }
}

        
        float connectDist = nodeSpacing + 0.2f;
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                if (i == j) continue;
                if (Mathf.Abs(nodes[i].transform.position.y - nodes[j].transform.position.y) < 0.2f &&
                    Vector2.Distance(nodes[i].transform.position, nodes[j].transform.position) <= connectDist)
                {
                    if (!nodes[i].connections.Contains(nodes[j]))
                        nodes[i].connections.Add(nodes[j]);
                }
            }
        }
        
        foreach (var node in nodes)
        {
            foreach (var target in nodes)
            {
                if (node == target) continue;
                Vector2 delta = target.transform.position - node.transform.position;
                if (Mathf.Abs(delta.x) > 0.2f && Mathf.Abs(delta.x) <= jumpDistance &&
                    delta.y > 0.1f && delta.y <= jumpHeight)
                {
                    node.isJumpPoint = true;
                    node.jumpTarget = target;
                }
            }
        }
        
        float dropMaxDistance = 6f; // max distance to look for a platform below

        foreach (var node in nodes)
        {
            Vector2 dropOrigin = node.transform.position + Vector3.down * 0.1f;
            RaycastHit2D hit = Physics2D.Raycast(dropOrigin, Vector2.down, dropMaxDistance, groundLayer);

            if (hit.collider != null)
            {
                // Find the node closest to hit.point, horizontally aligned
                Node dropTarget = null;
                float closest = 0.5f;
                foreach (var candidate in nodes)
                {
                    if (Mathf.Abs(candidate.transform.position.x - node.transform.position.x) < closest &&
                        candidate.transform.position.y < node.transform.position.y - 0.2f && // below
                        Mathf.Abs(candidate.transform.position.y - hit.point.y) < 0.5f)
                    {
                        dropTarget = candidate;
                        closest = Mathf.Abs(candidate.transform.position.x - node.transform.position.x);
                    }
                }

                if (dropTarget != null)
                {
                    node.connections.Add(dropTarget);
                    // Optionally: mark as "isDropPoint" if you want special logic
                }
            }
        }

    }

    public void ClearNodes()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
