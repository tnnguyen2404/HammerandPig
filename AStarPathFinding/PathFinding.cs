using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding : MonoBehaviour
{
    public Tilemap tileMap;
    public Vector3 nodeOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private int dropHeight;

    private bool hasNodeBelow;
    
    public Dictionary<Vector2Int, Node> nodeDictionary = new();

    public List<Node> AllNodes => new List<Node>(nodeDictionary.Values);
    
    [ContextMenu("Generate Nodes")]
    void GenerateNodes()
    {
        BoundsInt bounds = tileMap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int belowPos = new Vector3Int(x, y, 0);
                Vector3Int abovePos = new Vector3Int(x, y + 1, 0);

                bool hasGroundBelow = tileMap.HasTile(belowPos);
                bool isAbove = !tileMap.HasTile(abovePos);
                
                if (hasGroundBelow && isAbove)
                {
                    Vector3 worldPos = tileMap.CellToWorld(belowPos) + nodeOffset;
                    Vector2Int gridKey = new Vector2Int(x, y + 1);

                    Node node = new Node(NodeType.Walkable, worldPos, gridKey.x, gridKey.y);
                    nodeDictionary[gridKey] = node;
                }
            }
        }
    }
    
    [ContextMenu("PlaceEdgeNodes")]
    void PlaceEdgeJumpNodes()
    {
        List<Node> newEdgeNodes = new();

        foreach (var node in nodeDictionary.Values)
        {
            if (node.walkNeighbours.Count != 1)
                continue;

            Vector2Int current = new Vector2Int(node.gridX, node.gridY);
            
            Vector2Int[] directions = { Vector2Int.left, Vector2Int.right };

            foreach (var dir in directions)
            {
                Vector2Int candidate = current + dir;
                
                if (nodeDictionary.ContainsKey(candidate))
                {
                    continue;
                }

                Vector3Int candidatePos = new Vector3Int(candidate.x, candidate.y, 0);
                Vector3Int posToPlace = new Vector3Int(candidate.x, candidate.y - 1, 0);

                bool noTile = !tileMap.HasTile(candidatePos);

                for (int i = 1; i <= dropHeight; i++)
                {
                    Vector2Int checkBelow = new Vector2Int(candidate.x, candidate.y - i);
                    hasNodeBelow = nodeDictionary.ContainsKey(checkBelow);
                    if (hasNodeBelow) break;
                }
                
                
                if (noTile && hasNodeBelow)
                {
                    Vector3 worldPos = tileMap.CellToWorld(posToPlace) + nodeOffset;
                    Node edgeNode = new Node(NodeType.DropPoint, worldPos, candidate.x, candidate.y);
                    edgeNode.jumpNode = true;

                    newEdgeNodes.Add(edgeNode);
                }
            }

        }
        
        foreach (var edgeNode in newEdgeNodes)
        {
            Vector2Int key = new Vector2Int(edgeNode.gridX, edgeNode.gridY);
            nodeDictionary[key] = edgeNode;
        }
    }


    
    [ContextMenu("Link Node")]
    void LinkNodeNeighbors()
    {
        foreach (var node in nodeDictionary.Values)
        {
            Vector2Int pos = new Vector2Int(node.gridX, node.gridY);

            TryAddNeighbor(node, pos + Vector2Int.left);
            TryAddNeighbor(node, pos + Vector2Int.right);
        }
    }

    void TryAddNeighbor(Node fromNode, Vector2Int neighborPos)
    {
        if (nodeDictionary.TryGetValue(neighborPos, out Node neighbor))
        {
            fromNode.walkNeighbours.Add(neighbor);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (nodeDictionary == null) return;

        foreach (var node in nodeDictionary.Values)
        {
            Gizmos.color = node.jumpNode ? new Color(1f, 0.5f, 0f) : Color.green;
            Gizmos.DrawSphere(node.worldPosition, 0.08f);
            
            Gizmos.color = Color.yellow;
            foreach (var neighbor in node.walkNeighbours)
            {
                Gizmos.DrawLine(node.worldPosition, neighbor.worldPosition);
            }
        }
    }
}
