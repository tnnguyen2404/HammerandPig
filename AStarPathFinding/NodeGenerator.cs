using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap groundTilemap;           // Regular ground/platforms
    public Tilemap oneWayTilemap;           // One-way platforms (optional, can leave null if not used)

    [Header("Node Settings")]
    public GameObject nodePrefab;           // Node prefab with Node script
    public float nodeSpacing = 1f;          // Minimum X distance between nodes on the same platform
    public float jumpHeight = 3f;           // Enemy jump height
    public float jumpDistance = 3f;         // Enemy jump max X distance

    [Header("Layer Mask")]
    public LayerMask groundMask;            // For raycast/jump checks
    
    [ContextMenu("Generate Node")]

    void GenerateNodes()
    {
        ClearNodes();

        List<Node> nodes = new List<Node>();

        // Gather bounds of all tilemaps used
        BoundsInt bounds = groundTilemap.cellBounds;
        if (oneWayTilemap != null)
        {
            bounds.xMin = Mathf.Min(bounds.xMin, oneWayTilemap.cellBounds.xMin);
            bounds.xMax = Mathf.Max(bounds.xMax, oneWayTilemap.cellBounds.xMax);
            bounds.yMin = Mathf.Min(bounds.yMin, oneWayTilemap.cellBounds.yMin);
            bounds.yMax = Mathf.Max(bounds.yMax, oneWayTilemap.cellBounds.yMax);
        }

        // STEP 1: Place nodes at platform edges & on one-way platforms
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
                    // Only put nodes at walkable places: on top of ground/oneway tiles
                    Vector3 worldPos = groundTilemap.CellToWorld(pos) + new Vector3(0.5f, 1.5f, 0f);

                    // Place at left and right edge, or at a certain interval along the platform
                    bool leftEmpty = !(groundTilemap.HasTile(new Vector3Int(x - 1, y, 0)) ||
                                      (oneWayTilemap != null && oneWayTilemap.HasTile(new Vector3Int(x - 1, y, 0))));
                    bool rightEmpty = !(groundTilemap.HasTile(new Vector3Int(x + 1, y, 0)) ||
                                      (oneWayTilemap != null && oneWayTilemap.HasTile(new Vector3Int(x + 1, y, 0))));

                    bool placeNode = false;

                    // Place at platform edges
                    if (leftEmpty || rightEmpty)
                        placeNode = true;

                    // Also place at intervals along the platform
                    if (!placeNode && x % Mathf.RoundToInt(nodeSpacing) == 0)
                        placeNode = true;

                    if (placeNode)
                    {
                        GameObject nodeGO = Instantiate(nodePrefab, worldPos, Quaternion.identity, this.transform);
                        Node node = nodeGO.GetComponent<Node>();
                        nodes.Add(node);
                    }
                }
            }
        }

        // STEP 2: Connect horizontally-adjacent nodes (walkable connections)
        float connectDist = nodeSpacing + 0.2f;
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                if (i == j) continue;
                // On same height & close in X, connect
                if (Mathf.Abs(nodes[i].transform.position.y - nodes[j].transform.position.y) < 0.2f &&
                    Mathf.Abs(nodes[i].transform.position.x - nodes[j].transform.position.x) <= connectDist)
                {
                    nodes[i].connections.Add(nodes[j]);
                }
            }
        }

        // STEP 3: Jump links (from edge nodes, up and down)
        foreach (var node in nodes)
        {
            // Search for possible landing nodes within jump range (up or down)
            foreach (var target in nodes)
            {
                if (node == target) continue;
                Vector2 delta = target.transform.position - node.transform.position;
                if (Mathf.Abs(delta.x) > 0.2f && Mathf.Abs(delta.x) <= jumpDistance &&
                    delta.y > 0.1f && delta.y <= jumpHeight)
                {
                    // Optional: Linecast to check if jump is clear (no walls/ceilings in the way)
                    
                    node.isJumpPoint = true;
                    node.jumpTarget = target;
                    Debug.Log($"Jump created from {node.transform.position} to {target.transform.position}");
                    
                }
                // You can add drop-down (fall through one-way) as another connection if wanted:
                // if (delta.y < -0.1f && Mathf.Abs(delta.x) < jumpDistance && ... )
            }
        }

        Debug.Log("Nodes generated: " + nodes.Count);
    }

    public void ClearNodes()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
