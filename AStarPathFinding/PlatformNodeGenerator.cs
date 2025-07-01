using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformNodeGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap oneWayPlatformTilemap; // Assign your one-way platform tilemap here (can be null)

    [Header("Node Setup")]
    public GameObject nodePrefab;
    public float nodeSpacing = 1f;      // Usually 1 if your tiles are 1x1 units
    public int verticalSearch = 10;     // How far to look down for a landing

    private Dictionary<Vector2Int, Node> nodeGrid = new Dictionary<Vector2Int, Node>();

    // Allow node generation from Inspector
    [ContextMenu("Generate Nodes")]
    public void GenerateNodes()
    {
        // Clean up old nodes
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        nodeGrid.Clear();

        // Use ground tilemap bounds
        BoundsInt bounds = groundTilemap.cellBounds;

        // 1. Place nodes on walkable surfaces (ground or one-way platform)
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                bool isGround = groundTilemap.HasTile(cellPos);
                bool isOneWay = oneWayPlatformTilemap != null && oneWayPlatformTilemap.HasTile(cellPos);

                if (isGround || isOneWay)
                {
                    // Only if air above (not blocked by any tilemap)
                    Vector3Int above = cellPos + Vector3Int.up;
                    bool airAbove =
                        !groundTilemap.HasTile(above) &&
                        (oneWayPlatformTilemap == null || !oneWayPlatformTilemap.HasTile(above));

                    if (airAbove)
                    {
                        Vector2 worldPos = groundTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 1f, 0f);
                        Node newNode = Instantiate(nodePrefab, worldPos, Quaternion.identity, transform).GetComponent<Node>();
                        nodeGrid[new Vector2Int(x, y)] = newNode;
                    }
                }
            }
        }

        // 2. Connect horizontally and diagonally (stairs)
        foreach (var kvp in nodeGrid)
        {
            Vector2Int pos = kvp.Key;
            Node node = kvp.Value;

            // Horizontal
            Vector2Int left = new Vector2Int(pos.x - 1, pos.y);
            Vector2Int right = new Vector2Int(pos.x + 1, pos.y);
            if (nodeGrid.ContainsKey(left))
                node.connections.Add(nodeGrid[left]);
            if (nodeGrid.ContainsKey(right))
                node.connections.Add(nodeGrid[right]);

            // Step up/down (stairs): Only connect if neighbor is exactly one above/below
            Vector2Int upLeft = new Vector2Int(pos.x - 1, pos.y + 1);
            Vector2Int upRight = new Vector2Int(pos.x + 1, pos.y + 1);
            Vector2Int downLeft = new Vector2Int(pos.x - 1, pos.y - 1);
            Vector2Int downRight = new Vector2Int(pos.x + 1, pos.y - 1);

            if (nodeGrid.ContainsKey(upLeft))
                node.connections.Add(nodeGrid[upLeft]);
            if (nodeGrid.ContainsKey(upRight))
                node.connections.Add(nodeGrid[upRight]);
            if (nodeGrid.ContainsKey(downLeft))
                node.connections.Add(nodeGrid[downLeft]);
            if (nodeGrid.ContainsKey(downRight))
                node.connections.Add(nodeGrid[downRight]);
        }



        // 3. Detect edge nodes and create vertical jump/fall lines
        List<Node> edgeNodes = new List<Node>();
        foreach (var kvp in nodeGrid)
        {
            var node = kvp.Value;
            Vector2Int pos = kvp.Key;
            int horizontalNeighbors = 0;
            if (nodeGrid.ContainsKey(new Vector2Int(pos.x - 1, pos.y))) horizontalNeighbors++;
            if (nodeGrid.ContainsKey(new Vector2Int(pos.x + 1, pos.y))) horizontalNeighbors++;
            if (horizontalNeighbors == 1)
                edgeNodes.Add(node);
        }

        foreach (Node edge in edgeNodes)
        {
            // Determine direction: left or right
            Vector2Int edgeCell = WorldToCell(edge.transform.position - new Vector3(0.5f, 1f, 0f));
            Vector2Int leftCell = new Vector2Int(edgeCell.x - 1, edgeCell.y);
            Vector2Int rightCell = new Vector2Int(edgeCell.x + 1, edgeCell.y);
            bool leftHasNode = nodeGrid.ContainsKey(leftCell);
            bool rightHasNode = nodeGrid.ContainsKey(rightCell);
            Vector2Int jumpDir = rightHasNode ? Vector2Int.left : Vector2Int.right;

            Vector2Int jumpStartCell = edgeCell + jumpDir;

            // Only create jump node if space is empty (no ground or one-way)
            if (!groundTilemap.HasTile(new Vector3Int(jumpStartCell.x, jumpStartCell.y, 0)) &&
                (oneWayPlatformTilemap == null || !oneWayPlatformTilemap.HasTile(new Vector3Int(jumpStartCell.x, jumpStartCell.y, 0))))
            {
                Vector3 jumpStartPos = groundTilemap.CellToWorld(new Vector3Int(jumpStartCell.x, jumpStartCell.y, 0)) + new Vector3(0.5f, 1f, 0f);
                Node jumpNode = Instantiate(nodePrefab, jumpStartPos, Quaternion.identity, transform).GetComponent<Node>();
                edge.connections.Add(jumpNode);
                jumpNode.connections.Add(edge); // Bidirectional link

                Node lastNode = jumpNode;

                // Always create air nodes until just above the landing,
                // then connect the last air node to the existing landing node.
                bool landingPlaced = false;
                for (int i = 1; i <= verticalSearch; i++)
                {
                    Vector2Int downCell = new Vector2Int(jumpStartCell.x, jumpStartCell.y - i);
                    Vector3Int cell3d = new Vector3Int(downCell.x, downCell.y, 0);

                    bool foundGround = groundTilemap.HasTile(cell3d);
                    bool foundOneWay = oneWayPlatformTilemap != null && oneWayPlatformTilemap.HasTile(cell3d);

                    if (foundGround || foundOneWay)
                    {
                        // Use existing landing node if possible
                        Node landingNode;
                        if (nodeGrid.ContainsKey(downCell))
                        {
                            landingNode = nodeGrid[downCell];
                            landingNode.isJumpPoint = true;
                        }
                        else
                        {
                            Vector3 landingPos = groundTilemap.CellToWorld(cell3d) + new Vector3(0.5f, 1f, 0f);
                            landingNode = Instantiate(nodePrefab, landingPos, Quaternion.identity, transform).GetComponent<Node>();
                            landingNode.isJumpPoint = true;
                            nodeGrid[downCell] = landingNode;
                        }
                        lastNode.connections.Add(landingNode);
                        landingNode.connections.Add(lastNode);
                        landingPlaced = true;
                        break;
                    }
                    else
                    {
                        Vector3 airPos = groundTilemap.CellToWorld(cell3d) + new Vector3(0.5f, 1f, 0f);
                        Node airNode = Instantiate(nodePrefab, airPos, Quaternion.identity, transform).GetComponent<Node>();
                        lastNode.connections.Add(airNode);
                        airNode.connections.Add(lastNode);
                        lastNode = airNode;
                    }
                }

            }
        }
    }
    
    // Convert world position to cell coordinates
    private Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3Int cell = groundTilemap.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }
}
