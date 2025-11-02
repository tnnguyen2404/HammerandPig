using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding : MonoBehaviour
{
    private const float GIZMO_NODE_SIZE = 0.08f;
    
    public Tilemap tileMap;
    public Vector3 nodeOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private int dropHeight;

    public Dictionary<Vector2Int, Node> nodeDictionary = new();
    
    private List<Node> cachedAllNodes;
    private bool nodesCacheDirty = true;

    public List<Node> AllNodes
    {
        get
        {
            if (nodesCacheDirty || cachedAllNodes == null)
            {
                cachedAllNodes = new List<Node>(nodeDictionary.Values);
                nodesCacheDirty = false;
            }
            return cachedAllNodes;
        }
    }

    [ContextMenu("Generate Nodes")]
    void GenerateNodes()
    {
        if (tileMap == null)
        {
            Debug.LogWarning("Tilemap is not assigned");
            return;
        }

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
                    AddNode(x, y);
                }
            }
        }
        
        nodesCacheDirty = true;
    }
    
    [ContextMenu("PlaceEdgeNodes")]
    void PlaceEdgeJumpNodes()
    {
        if (nodeDictionary == null || nodeDictionary.Count == 0)
        {
            Debug.LogWarning("Node dictionary is empty. Generate nodes first");
            return;
        }

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
                    continue;

                Vector3Int candidatePos = new Vector3Int(candidate.x, candidate.y, 0);
                Vector3Int posToPlace = new Vector3Int(candidate.x, candidate.y - 1, 0);

                bool noTile = !tileMap.HasTile(candidatePos);
                bool hasNodeBelow = false;

                for (int i = 1; i <= dropHeight; i++)
                {
                    Vector2Int checkBelow = new Vector2Int(candidate.x, candidate.y - i);
                    hasNodeBelow = nodeDictionary.ContainsKey(checkBelow);
                    if (hasNodeBelow) 
                        break;
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
        
        nodesCacheDirty = true;
    }

    [ContextMenu("Link Node")]
    void LinkNodeNeighbors()
    {
        if (nodeDictionary == null || nodeDictionary.Count == 0)
        {
            Debug.LogWarning("Node dictionary is empty. Generate nodes first");
            return;
        }

        foreach (var node in nodeDictionary.Values)
        {
            Vector2Int pos = new Vector2Int(node.gridX, node.gridY);

            TryAddNeighbor(node, pos + Vector2Int.left);
            TryAddNeighbor(node, pos + Vector2Int.right);
        }
    }
    
    [ContextMenu("Link Stair Nodes")]
    void LinkStairNodes()
    {
        if (nodeDictionary == null || nodeDictionary.Count == 0)
        {
            Debug.LogWarning("Node dictionary is empty. Generate nodes first");
            return;
        }

        foreach (var node in nodeDictionary.Values)
        {
            Vector2Int pos = new Vector2Int(node.gridX, node.gridY);
        
            Vector2Int upLeft = pos + new Vector2Int(-1, 1);
            Vector2Int upRight = pos + new Vector2Int(1, 1);
            Vector2Int downLeft = pos + new Vector2Int(-1, -1);
            Vector2Int downRight = pos + new Vector2Int(1, -1);
        
            TryAddStairNeighbor(node, pos, upLeft);
            TryAddStairNeighbor(node, pos, upRight);
            TryAddStairNeighbor(node, pos, downLeft);
            TryAddStairNeighbor(node, pos, downRight);
        }
    }

    [ContextMenu("Link Drop Connections")]
    void LinkDropConnections()
    {
        if (nodeDictionary == null || nodeDictionary.Count == 0)
        {
            Debug.LogWarning("Node dictionary is empty. Generate nodes first");
            return;
        }

        foreach (var node in nodeDictionary.Values)
        {
            if (!node.jumpNode)
                continue;
        
            Vector2Int pos = new Vector2Int(node.gridX, node.gridY);
        
            for (int i = 1; i <= dropHeight; i++)
            {
                Vector2Int belowPos = pos + new Vector2Int(0, -i);
            
                if (nodeDictionary.TryGetValue(belowPos, out Node nodeBelow))
                {
                    if (!node.walkNeighbours.Contains(nodeBelow))
                    {
                        node.walkNeighbours.Add(nodeBelow);
                    }
                    break;
                }
            }
        }
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = GetClosestNode(startPos);
        Node targetNode = GetClosestNode(targetPos);
    
        if (startNode == null || targetNode == null)
        {
            Debug.LogWarning("Could not find valid start or target node");
            return null;
        }
    
        Heap<Node> openSet = new Heap<Node>(nodeDictionary.Count);
        HashSet<Node> closedSet = new HashSet<Node>();
        
        ResetNodeCosts();
        
        openSet.Add(startNode);
    
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);
        
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }
        
            foreach (Node neighbor in currentNode.walkNeighbours)
            {
                if (closedSet.Contains(neighbor))
                    continue;
            
                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
            
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;
                
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else
                        openSet.UpdateItem(neighbor);
                }
            }
        }
    
        return null;
    }

    public Node GetClosestNode(Vector3 worldPos)
    {
        Node closestNode = null;
        float closestDistance = float.MaxValue;
    
        foreach (var node in nodeDictionary.Values)
        {
            float distance = Vector3.Distance(worldPos, node.worldPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }
    
        return closestNode;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
    
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
    
        path.Reverse();
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
    
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    void ResetNodeCosts()
    {
        foreach (var node in nodeDictionary.Values)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }
    }

    void TryAddStairNeighbor(Node fromNode, Vector2Int fromPos, Vector2Int neighborPos)
    {
        if (!nodeDictionary.TryGetValue(neighborPos, out Node neighbor))
            return;
    
        Vector3Int fromTilePos = new Vector3Int(fromPos.x, fromPos.y - 1, 0);
        Vector3Int neighborTilePos = new Vector3Int(neighborPos.x, neighborPos.y - 1, 0);
    
        bool hasGroundAtFrom = tileMap.HasTile(fromTilePos);
        bool hasGroundAtNeighbor = tileMap.HasTile(neighborTilePos);
    
        if (hasGroundAtFrom && hasGroundAtNeighbor)
        {
            Vector3Int blockingTileFrom = new Vector3Int(fromPos.x, fromPos.y, 0);
            Vector3Int blockingTileNeighbor = new Vector3Int(neighborPos.x, neighborPos.y, 0);
        
            bool pathClear = !tileMap.HasTile(blockingTileFrom) && !tileMap.HasTile(blockingTileNeighbor);
        
            if (pathClear && !fromNode.walkNeighbours.Contains(neighbor))
            {
                fromNode.walkNeighbours.Add(neighbor);
            }
        }
    }

    void AddNode(int gridX, int gridY)
    {
        Vector3 worldPos = tileMap.CellToWorld(new Vector3Int(gridX, gridY, 0)) + nodeOffset;
        Vector2Int gridKey = new Vector2Int(gridX, gridY + 1);

        if (!nodeDictionary.ContainsKey(gridKey))
        {
            Node node = new Node(NodeType.Walkable, worldPos, gridKey.x, gridKey.y);
            nodeDictionary[gridKey] = node;
        }
    }

    void TryAddNeighbor(Node fromNode, Vector2Int neighborPos)
    {
        if (nodeDictionary.TryGetValue(neighborPos, out Node neighbor))
        {
            if (!fromNode.walkNeighbours.Contains(neighbor))
            {
                fromNode.walkNeighbours.Add(neighbor);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (nodeDictionary == null || nodeDictionary.Count == 0) 
            return;

        foreach (var node in nodeDictionary.Values)
        {
            Gizmos.color = node.jumpNode ? new Color(1f, 0.5f, 0f) : Color.green;
            Gizmos.DrawSphere(node.worldPosition, GIZMO_NODE_SIZE);
            
            Gizmos.color = Color.yellow;
            foreach (var neighbor in node.walkNeighbours)
            {
                Gizmos.DrawLine(node.worldPosition, neighbor.worldPosition);
            }
        }
    }
}
