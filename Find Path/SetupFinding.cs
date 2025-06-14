
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps; 
using Newtonsoft.Json;
using System.IO;
using static Find;
using System;
using System.Net.WebSockets;
using System.Security.Cryptography;
using Unity.VisualScripting;
public class SetupFinding : MonoBehaviour
{
    public static SetupFinding Instance { get; private set; }
    [Header("Value cần để tính góc nhảy")]
    [SerializeField]float jumpForce = 10;
    [SerializeField] float horizontalSpeed = 5f;
    [SerializeField] float gravityScale = 3f;
    [SerializeField] float SizeColiderCheck = 0.3f;
    [SerializeField] float DivisionByHeight = 3;
    [SerializeField] float DivisionByWidth = 8;
    public LayerMask MapLayer;
    [Header("Cấu hình Tilemap")]
    [SerializeField] List<Tilemap> TileMapMoveDefault;
    [SerializeField] List<Tilemap> TileMapCanJump;
    [SerializeField] List<Tilemap> TrapTileMap;
    [SerializeField] Tilemap combinedTilemap;
    [Header("Gizmos Support")]
    [SerializeField] bool DrawLine = false;
    [SerializeField] bool DrawPlatform = false;
    [SerializeField] bool ViewJumpPonit = false;
    [SerializeField] Vector2 PositionPoint;
    List<InfoPoint> infoPoints = new List<InfoPoint>();
    List<PlatForm> PlatForms = new List<PlatForm>();
    Dictionary<int, Dictionary<int, PathInfo>> FindWay;
    List<PlatForm> PlatFormss = new List<PlatForm>();
   
    
    [ContextMenu("Run Method")]
    public void StartMethod()
    {
        if (CheckValueDead()) return;
        combinedTilemap.ClearAllTiles();
        infoPoints.Clear();
        PlatForms.Clear();
        CombineTilemapsIntoOne(TileMapMoveDefault);
        if (TileMapCanJump.Count>0)
        {
            CombineTilemapsIntoOne(TileMapCanJump);
        }
        if (TrapTileMap.Count>0) {
            CombineTilemapsIntoOne(TrapTileMap);
        }
        RunFromInspector(combinedTilemap);
        SetUpJumpLine();
        SelectStepPoint(MapLayer);
        SetUpLinePlatform();
        SelectLineJump();
        SetUpLineChangePlatform();
        SetVector2AfterSaveJson();
        var allPaths = FindAllPaths(PlatForms);
        SavePathsToJson(allPaths, "pathsway.json");
        SavePlatForm("pathsPlatForm.json");
    }
    bool CheckValueDead()
    {
        bool ok = false;
        if (combinedTilemap == null)
        {
            Debug.Log("Chưa có combinedTilemap");
            ok = true;
        }
        if (TileMapMoveDefault.Count<=0)
        {
            Debug.Log("Chưa có combinedTilemap"); ok = true;
        }
        if (jumpForce<=0)
        {
            Debug.Log("jumpForce Phải lớn hơn 0"); ok = true;
        }
        if (gravityScale<=0)
        {
            Debug.Log("gravityScale Phải lớn hơn 0"); ok = true;
        }
        if (horizontalSpeed<=0)
        {
            Debug.Log("horizontalSpeed Phải lớn hơn 0"); ok = true;
        }
        if (SizeColiderCheck<=0)
        {
            Debug.Log("SizeColiderCheck Phải lớn hơn 0"); ok = true;
        }
        return ok;
    }
    private void Awake()
    {
        StartMethod();
        Instance = this;
    }
    private void Start()
    {
       
        FindWay = LoadPathsFromJson("pathsway.json");
        PlatForms = LoadPathsPlatForm("pathsPlatForm.json");
        
    }
    public static Dictionary<int, Dictionary<int, PathInfo>> FindAllPaths(List<PlatForm> platforms)
    {
        Dictionary<int, Dictionary<int, PathInfo>> allPaths = new Dictionary<int, Dictionary<int, PathInfo>>();

        foreach (var platform in platforms)
        {
            int startId = platform.IDPlatform;
            allPaths[startId] = FindShortestPaths(platforms, startId);
        }

        return allPaths;
    }
    public void SetVector2AfterSaveJson()
    {
        foreach(var i in infoPoints)
        {
            i.PointPositioni.x = i.PointPosition.x;
            i.PointPositioni.y = i.PointPosition.y;
            i.LeftPoint.PosionGeti.x = i.LeftPoint.PosionGet.x;
            i.LeftPoint.PosionGeti.y = i.LeftPoint.PosionGet.y;
            i.RightPoint.PosionGeti.x = i.RightPoint.PosionGet.x;
            i.RightPoint.PosionGeti.y = i.RightPoint.PosionGet.y;
            foreach(var j in i.jumpInfo)
            {
                j.EndJumpi.x = j.EndJump.x;
                j.EndJumpi.y = j.EndJump.y;
                j.ForceJumpi.x = j.ForceJump.x;
                j.ForceJumpi.y = j.ForceJump.y;
            }
        }
    }
    private static Dictionary<int, PathInfo> FindShortestPaths(List<PlatForm> platforms, int startId)
    {
        var graph = platforms.ToDictionary(p => p.IDPlatform, p => p);
        var previousNodes = new Dictionary<int, int>();
        var visited = new HashSet<int>();
        var queue = new Queue<int>();

        previousNodes[startId] = -1;
        queue.Enqueue(startId);
        visited.Add(startId);

        while (queue.Count > 0)
        {
            int currentId = queue.Dequeue();
            if (!graph.ContainsKey(currentId)) continue;

            foreach (var action in graph[currentId].actionChangePlatform)
            {
                int neighborId = action.PlatFormTargetID;
                if (!visited.Contains(neighborId))
                {
                    visited.Add(neighborId);
                    previousNodes[neighborId] = currentId;
                    queue.Enqueue(neighborId);
                }
            }
        }

        var shortestPaths = new Dictionary<int, PathInfo>();
        foreach (var node in previousNodes.Keys)
        {
            List<int> path = ReconstructPath(previousNodes, node);
            shortestPaths[node] = new PathInfo(path);
        }

        return shortestPaths;
    }

    private static List<int> ReconstructPath(Dictionary<int, int> previousNodes, int targetId)
    {
        List<int> path = new List<int>();
        int currentId = targetId;

        while (currentId != -1)
        {
            path.Add(currentId);
            currentId = previousNodes[currentId];
        }

        path.Reverse();
        return path;
    }

    public static void SavePathsToJson(Dictionary<int, Dictionary<int, PathInfo>> allPaths, string filePath)
    {
        string json = JsonConvert.SerializeObject(allPaths, Formatting.Indented);
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log($"Saved paths to {filePath}");
    }
    public void SavePlatForm(string filename)
    {
        string json = JsonConvert.SerializeObject(PlatForms, Formatting.Indented);
        System.IO.File.WriteAllText(filename, json);
    }
    void SelectLineJump()
    {
        foreach (var point in infoPoints)
        {
            PlatForm platformLeft = null;
            PlatForm platformRight = null;

            // Xác định platform cho điểm bên trái và bên phải nếu cần
            if (point.SatePoint == SatePoint.LeftEdge || point.SatePoint == SatePoint.Solo)
            {
                platformLeft = GetPlatformToPoint(point.LeftPoint.PosionGet);
            }
            if (point.SatePoint == SatePoint.RightEdge || point.SatePoint == SatePoint.Solo)
            {
                platformRight = GetPlatformToPoint(point.RightPoint.PosionGet);
            }

            // Lặp ngược qua danh sách jumpInfo
            for (int j = point.jumpInfo.Count - 1; j >= 0; j--)
            {
                var jumpInfo = point.jumpInfo[j];
                var endJump = jumpInfo.EndJump;

                // Điều kiện loại bỏ nếu điểm kết thúc chính là vị trí của điểm hiện tại
                if (endJump == point.PointPosition)
                {
                    point.jumpInfo.RemoveAt(j);
                    continue;
                }

                // Kiểm tra xem EndJump có khớp với bất kỳ PointPosition nào trong infoPoints
                bool isEndJumpValid = infoPoints.Any(p => p.PointPosition == endJump);

                // Loại bỏ nếu không khớp bất kỳ điểm nào hoặc không hợp lệ
                if (!isEndJumpValid ||
                    CheckJumpPlatform(point.PointPosition, endJump) ||
                    (platformRight != null && platformRight == GetPlatformToPoint(endJump)) ||
                    (platformLeft != null && platformLeft == GetPlatformToPoint(endJump)))
                {
                    point.jumpInfo.RemoveAt(j);
                }
            }
        }
        if (TileMapCanJump.Count >0)
        {
            foreach (var point in infoPoints)
            {
                foreach (var i in TileMapCanJump)
                {
                    if (i.HasTile(new Vector3Int((int)point.PointPosition.x, (int)point.PointPosition.y + 1, (int)0)))
                    {
                        JumpInfo j = new JumpInfo();
                        j.EndJump = new Vector2(point.PointPosition.x, point.PointPosition.y + 2);
                        j.ForceJump = new Vector2(0, jumpForce);
                        point.jumpInfo.Add(j);
                        break;
                    }
                }
            }
        }
    }

    void CombineTilemapsIntoOne(List<Tilemap> sourceTilemap)
    {
        foreach (var tilemap in sourceTilemap)
        {
            if (tilemap != null)
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(pos)) continue;
                combinedTilemap.SetTile(pos, tilemap.GetTile(pos));
            }
        }
    }

    void SetUpLineChangePlatform()
    {
        foreach (var pf in PlatForms)
        {
            List<ActionChangePlatform> PFtarget = new List<ActionChangePlatform>();
            foreach (var i in pf.PointInPlatForm)
            {
                if (i.SatePoint == SatePoint.LeftEdge && i.LeftPoint.PosionGet != i.PointPosition)
                {
                    ActionChangePlatform actionChangePlatform = new ActionChangePlatform();

                    actionChangePlatform.PlatFormTargetID = GetPlatformToPoint(i.LeftPoint.PosionGet).IDPlatform;
                    actionChangePlatform.stateAction = StateAction.fall;
                    actionChangePlatform.TargetPoint = i.LeftPoint.PosionGeti;
                    actionChangePlatform.PointStart = i;
                    PFtarget.Add(actionChangePlatform);
                }
                else if (i.SatePoint == SatePoint.RightEdge && i.RightPoint.PosionGet != i.PointPosition)
                {
                    ActionChangePlatform actionChangePlatform = new ActionChangePlatform();
                    actionChangePlatform.PlatFormTargetID = GetPlatformToPoint(i.RightPoint.PosionGet).IDPlatform; 
                    actionChangePlatform.stateAction = StateAction.fall;
                    actionChangePlatform.TargetPoint = i.RightPoint.PosionGeti;
                    actionChangePlatform.PointStart = i;
                    PFtarget.Add(actionChangePlatform);
                }
                else if (i.SatePoint == SatePoint.Solo )
                {
                    if (i.RightPoint.PosionGet != i.PointPosition)
                    {
                        ActionChangePlatform actionChangePlatform = new ActionChangePlatform();

                        actionChangePlatform.PlatFormTargetID = GetPlatformToPoint(i.RightPoint.PosionGet).IDPlatform;
                        actionChangePlatform.stateAction = StateAction.fall;
                        actionChangePlatform.TargetPoint = i.RightPoint.PosionGeti;
                        actionChangePlatform.PointStart = i;
                        PFtarget.Add(actionChangePlatform);
                    }
                    if (i.LeftPoint.PosionGet != i.PointPosition)
                    {
                        ActionChangePlatform actionChangePlatform1 = new ActionChangePlatform();

                        actionChangePlatform1.PlatFormTargetID = GetPlatformToPoint(i.LeftPoint.PosionGet).IDPlatform;
                        actionChangePlatform1.stateAction = StateAction.fall;
                        actionChangePlatform1.TargetPoint = i.LeftPoint.PosionGeti;
                        actionChangePlatform1.PointStart = i;
                        PFtarget.Add(actionChangePlatform1);
                    }
                }

                foreach (var JumpIf in i.jumpInfo) {
                    ActionChangePlatform actionChangePlatform1 = new ActionChangePlatform();
                    actionChangePlatform1.PlatFormTargetID = GetPlatformToPoint(JumpIf.EndJump).IDPlatform;
                    actionChangePlatform1.stateAction = StateAction.jump;
                    actionChangePlatform1.PointStart = i;
                    actionChangePlatform1.TargetPoint = JumpIf.EndJumpi;
                    foreach (var j in actionChangePlatform1.PointStart.jumpInfo)
                    {
                        if(j.EndJumpi == actionChangePlatform1.TargetPoint)
                        {
                            actionChangePlatform1.ForceJump = j.ForceJumpi;
                            break;
                        }
                    }
                    PFtarget.Add(actionChangePlatform1);
                }
            }

            pf.actionChangePlatform = PFtarget;
        }
        foreach (var pf in PlatForms)
        {
            List<ActionChangePlatform> Pftg = new List<ActionChangePlatform>();
            foreach (var p in pf.actionChangePlatform)
            {   bool ok = false;
                
                    foreach (var i in Pftg)
                {
                    if(p == i && p.PlatFormTargetID == i.PlatFormTargetID)
                    {
                        ok = true; break;
                    }
                }
                if (ok == true)
                {
                    continue;
                }
                Pftg.Add(p);
            }
            pf.actionChangePlatform = Pftg;
        }
    }
    #region SetUpLinePlatform
    void SetUpLinePlatform()
    {
        foreach (var i in infoPoints)
        {
            // Bỏ qua các điểm có trạng thái là RightEdge hoặc Flatform
            if (i.SatePoint == SatePoint.RightEdge || i.SatePoint == SatePoint.Flatform)
            {
                continue;
            }

            PlatForm PF = new PlatForm();
            PF.IDPlatform = PlatForms.Count;
            // Nếu điểm là LeftEdge
            if (i.SatePoint == SatePoint.LeftEdge)
            {
                PF.LeftPoint = i.PointPosition;
                InfoPoint CurrenCheck = i;
                PF.PointInPlatForm.Add(CurrenCheck);
                // Duyệt qua các điểm để xây dựng platform cho đến khi gặp RightEdge
                do
                {
                    

                    // Tìm điểm kế tiếp trong platform
                    InfoPoint nextPoint = null;
                    foreach (var j in infoPoints)
                    {
                        if (j.PointPosition.y == CurrenCheck.PointPosition.y && j.PointPosition.x == CurrenCheck.PointPosition.x + 1)
                        {
                            nextPoint = j;
                            break;
                        }
                    }

                    // Nếu tìm được điểm tiếp theo, cập nhật
                    if (nextPoint != null)
                    {
                        CurrenCheck = nextPoint;
                        PF.PointInPlatForm.Add(CurrenCheck);
                    }
                    else
                    {
                        // Nếu không tìm được điểm tiếp theo, thoát vòng lặp
                        break;
                    }

                } while (CurrenCheck.SatePoint != SatePoint.RightEdge);

                // Gán RightPoint khi gặp RightEdge
                PF.RightPoint = CurrenCheck.PointPosition;

            }
            // Nếu điểm là Solo
            else if (i.SatePoint == SatePoint.Solo)
            {
                PF.LeftPoint = i.PointPosition;
                PF.RightPoint = i.PointPosition;
                PF.PointInPlatForm.Add(i);
                PlatForms.Add(PF);
                continue;
            }

            // Thêm PlatForm vào danh sách PlatForms
            PlatForms.Add(PF);
        }
    }
    #endregion

    #region Setup line
    private void RunFromInspector(Tilemap tilemap)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is null!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            bool Check = false;
            if (TrapTileMap.Count >0)
                foreach (var i in TrapTileMap)
                {
                    bool ok = false;
                    foreach (Vector3Int pos in i.cellBounds.allPositionsWithin)
                    {
                        if (cellPosition == pos)
                        {  Check = true;
                            ok = true;
                            break; }
                    }
                    if (ok) break;
                }
            if (Check) continue;

            if (!tilemap.HasTile(cellPosition))
                continue;

            if (!tilemap.HasTile(cellPosition + new Vector3Int(0, 1, 0)))
            {
                InfoPoint point = new InfoPoint();
                point.PointPosition = new Vector2(cellPosition.x, cellPosition.y + 1);

                // Check Right Edge
                if (tilemap.HasTile(cellPosition + new Vector3Int(1, 1, 0)) || !tilemap.HasTile(cellPosition + new Vector3Int(1, 0, 0)))
                {
                    point.SatePoint = SatePoint.RightEdge;
                }

                // Check Left Edge
                if (tilemap.HasTile(cellPosition + new Vector3Int(-1, 1, 0)) || !tilemap.HasTile(cellPosition + new Vector3Int(-1, 0, 0)))
                {
                    if (point.SatePoint == SatePoint.RightEdge)
                    {
                        point.SatePoint = SatePoint.Solo;
                    }
                    else
                    {
                        point.SatePoint = SatePoint.LeftEdge;
                    }
                }

                // Default to Flatform if no other condition is met
                if (point.SatePoint == SatePoint.None)
                {
                    point.SatePoint = SatePoint.Flatform;
                }

                infoPoints.Add(point);
            }
        }

        for (int i = 0; i < infoPoints.Count; i++)
        {
            switch (infoPoints[i].SatePoint)
            {
                case SatePoint.LeftEdge:
                    {
                        if (infoPoints[i].LeftPoint == null) infoPoints[i].LeftPoint = new InfoAction();
                        if (infoPoints[i].RightPoint == null) infoPoints[i].RightPoint = new InfoAction();

                        infoPoints[i].RightPoint.PosionGet = infoPoints[i].PointPosition + new Vector2(1, 0);
                        Vector2 tg = infoPoints[i].PointPosition;

                        if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x - 1, (int)infoPoints[i].PointPosition.y, 0)))
                        {
                            infoPoints[i].LeftPoint.PosionGet = tg;
                            break;
                        }

                        for (int j = (int)infoPoints[i].PointPosition.y; j > bounds.min.y; j--)
                        {
                            if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x - 1, j, 0)))
                            {
                                if (TrapTileMap.Count > 0)
                                { 
                                    bool ok = false;
                                    foreach (var Tilemap in TrapTileMap)
                                    {
                                        if (Tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x - 1, j, 0)))
                                        {
                                            ok = true;
                                            break;
                                        }
                                        if (ok) break;
                                    }
                                }
                                infoPoints[i].LeftPoint.action = StateAction.fall;
                                tg = new Vector2((int)infoPoints[i].PointPosition.x - 1, j + 1);
                                break;
                            }
                        }

                        infoPoints[i].LeftPoint.PosionGet = tg;
                    }
                    break;

                case SatePoint.RightEdge:
                    {
                        if (infoPoints[i].LeftPoint == null) infoPoints[i].LeftPoint = new InfoAction();
                        if (infoPoints[i].RightPoint == null) infoPoints[i].RightPoint = new InfoAction();

                        infoPoints[i].LeftPoint.PosionGet = infoPoints[i].PointPosition + new Vector2(-1, 0);
                        Vector2 tg = infoPoints[i].PointPosition;

                        if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x + 1, (int)infoPoints[i].PointPosition.y, 0)))
                        {
                            infoPoints[i].RightPoint.PosionGet = tg;
                            break;
                        }

                        for (int j = (int)infoPoints[i].PointPosition.y; j > bounds.min.y; j--)
                        {
                            if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x + 1, j, 0)))
                            {
                                if (TrapTileMap.Count > 0)
                                {
                                    bool ok = false;
                                    foreach (var Tilemap in TrapTileMap)
                                    {
                                        if (Tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x + 1, j, 0)))
                                        {
                                            ok = true;
                                            break;
                                        }
                                        if (ok) break;
                                    }
                                }
                                infoPoints[i].RightPoint.action = StateAction.fall;
                                tg = new Vector2((int)infoPoints[i].PointPosition.x + 1, j + 1);
                                break;
                            }
                        }

                        infoPoints[i].RightPoint.PosionGet = tg;
                    }
                    break;

                case SatePoint.Solo:
                    {
                        if (infoPoints[i].LeftPoint == null) infoPoints[i].LeftPoint = new InfoAction();
                        if (infoPoints[i].RightPoint == null) infoPoints[i].RightPoint = new InfoAction();

                        Vector2 tgr = infoPoints[i].PointPosition;
                        Vector2 tg = infoPoints[i].PointPosition;

                        if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x + 1, (int)infoPoints[i].PointPosition.y, 0)))
                        {
                            infoPoints[i].RightPoint.PosionGet = tgr;
                            for (int j = (int)infoPoints[i].PointPosition.y; j > bounds.min.y; j--)
                            {
                                if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x - 1, j, 0)))
                                {
                                    if (TrapTileMap.Count > 0)
                                    {
                                        bool ok = false;
                                        foreach (var Tilemap in TrapTileMap)
                                        {
                                            if (Tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x - 1, j, 0)))
                                            {
                                                ok = true;
                                                break;
                                            }
                                            if (ok) break;
                                        }
                                    }
                                    infoPoints[i].LeftPoint.action = StateAction.fall;
                                    tg = new Vector2((int)infoPoints[i].PointPosition.x - 1, j + 1);
                                    break;
                                }
                            }
                            infoPoints[i].LeftPoint.PosionGet = tg;
                        }

                        if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x - 1, (int)infoPoints[i].PointPosition.y, 0)))
                        {
                            infoPoints[i].LeftPoint.PosionGet = tg;
                            for (int j = (int)infoPoints[i].PointPosition.y; j > bounds.min.y; j--)
                            {
                                if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x + 1, j, 0)))
                                {
                                    if (TrapTileMap.Count > 0)
                                    {
                                        bool ok = false;
                                        foreach (var Tilemap in TrapTileMap)
                                        {
                                            if (Tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x + 1, j, 0)))
                                            {
                                                ok = true;
                                                break;
                                            }
                                            if (ok) break;
                                        }
                                    }
                                    infoPoints[i].RightPoint.action = StateAction.fall;
                                    tgr = new Vector2((int)infoPoints[i].PointPosition.x + 1, j + 1);
                                    break;
                                }
                            }
                            infoPoints[i].RightPoint.PosionGet = tgr;
                        }
                        if(!tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x - 1, (int)infoPoints[i].PointPosition.y, 0)) && !tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x + 1, (int)infoPoints[i].PointPosition.y, 0)))
                        {
                            infoPoints[i].RightPoint.PosionGet = tgr;
                            for (int j = (int)infoPoints[i].PointPosition.y; j > bounds.min.y; j--)
                            {
                                if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x - 1, j, 0)))
                                {
                                    if (TrapTileMap.Count > 0)
                                    {
                                        bool ok = false;
                                        foreach (var Tilemap in TrapTileMap)
                                        {
                                            if (Tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x - 1, j, 0)))
                                            {
                                                ok = true;
                                                break;
                                            }
                                            if (ok) break;
                                        }
                                    }
                                    infoPoints[i].LeftPoint.action = StateAction.fall;
                                    tg = new Vector2((int)infoPoints[i].PointPosition.x - 1, j + 1);
                                    break;
                                }
                            }
                            infoPoints[i].LeftPoint.PosionGet = tg;

                            for (int j = (int)infoPoints[i].PointPosition.y; j > bounds.min.y; j--)
                            {
                                if (tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x + 1, j, 0)))
                                {
                                    if (TrapTileMap.Count > 0)
                                    {
                                        bool ok = false;
                                        foreach (var Tilemap in TrapTileMap)
                                        {
                                            if (Tilemap.HasTile(new Vector3Int((int)infoPoints[i].PointPosition.x + 1, j, 0)))
                                            {
                                                ok = true;
                                                break;
                                            }
                                            if (ok) break;
                                        }
                                    }
                                    infoPoints[i].RightPoint.action = StateAction.fall;
                                    tgr = new Vector2((int)infoPoints[i].PointPosition.x + 1, j + 1);
                                    break;
                                }
                            }
                            infoPoints[i].RightPoint.PosionGet = tgr;
                        }
                        if (infoPoints[i].LeftPoint.PosionGet == Vector2.zero && infoPoints[i].RightPoint.PosionGet == Vector2.zero)
                        {
                            infoPoints[i].LeftPoint.PosionGet = tg;
                            infoPoints[i].RightPoint.PosionGet = tg;
                        }
                    }
                    break;

                case SatePoint.Flatform:
                    {
                        if (infoPoints[i].LeftPoint == null) infoPoints[i].LeftPoint = new InfoAction();
                        if (infoPoints[i].RightPoint == null) infoPoints[i].RightPoint = new InfoAction();

                        infoPoints[i].LeftPoint.action = StateAction.move;
                        infoPoints[i].RightPoint.action = StateAction.move;
                        infoPoints[i].RightPoint.PosionGet = infoPoints[i].PointPosition + new Vector2(1, 0);
                        infoPoints[i].LeftPoint.PosionGet = infoPoints[i].PointPosition + new Vector2(-1, 0);
                    }
                    break;
            }
        }
    }

    #endregion
    void SetUpJumpLine()
    {
        for (int k = -1; k <= 1; k+=2)
        {
            for (int i = 1; i <= DivisionByWidth; i++)
            {
                for (float j = 1; j <= DivisionByHeight; j += 0.2f)
                {
                    foreach (var point in infoPoints)
                    {

                        DrawJumpTrajectory( point.PointPosition + new Vector2(0.5f,0f), new Vector2(horizontalSpeed*k / i, jumpForce / j));
                    }
                }
            }
        }
    }
    void SelectStepPoint(LayerMask tileLayerMask)
    {
        foreach (var point in infoPoints)
        {
            for (int j = point.jumpInfo.Count - 1; j >= 0; j--) // Duyệt ngược để tránh lỗi xóa phần tử
            {
                var InforJump = point.jumpInfo[j];
                // Tìm điểm có độ cao lớn nhất
                int maxHeightIndex = -1;
                float maxHeight = float.MinValue;
                for (int i = 0; i < InforJump.PointStep.Count; i++)
                {
                    if (InforJump.PointStep[i].y > maxHeight)
                    {
                        maxHeight = InforJump.PointStep[i].y;
                        maxHeightIndex = i;
                    }
                }
                int firstCollisionIndex = -1;
                for (int i = 0; i < InforJump.PointStep.Count; i++)
                {
                    Vector2 pointPosition = new Vector2(InforJump.PointStep[i].x, InforJump.PointStep[i].y);
                    Collider2D collider = Physics2D.OverlapCircle(pointPosition, SizeColiderCheck, tileLayerMask);
                    if (TrapTileMap.Count>0)
                    {
                        Collider2D collider1 = new Collider2D();
                        foreach (var tilemap in TrapTileMap) {
                            collider1 = Physics2D.OverlapCircle(pointPosition, SizeColiderCheck, tilemap.gameObject.layer);
                            if (collider1 != null)
                            {
                                point.jumpInfo.RemoveAt(j);
                                continue;
                            }
                        }
                        
                    }
                    if (collider != null)
                    {
                        if (maxHeightIndex > i && point.PointPosition != new Vector2(CustomRound(pointPosition.x), CustomRound(pointPosition.y)))
                        {
                            firstCollisionIndex = i;
                            InforJump.EndJump = new Vector2(CustomRound(pointPosition.x), CustomRound(pointPosition.y));
                            break;
                        }
                        else if (maxHeightIndex <= i)
                        {
                            firstCollisionIndex = i;
                            InforJump.EndJump = new Vector2(CustomRound(pointPosition.x), CustomRound(pointPosition.y));
                            break;
                        }
                    }
                }
                if (firstCollisionIndex < maxHeightIndex)
                {
                    point.jumpInfo.RemoveAt(j);
                    continue;
                }
                if (firstCollisionIndex != -1)
                {
                    for (int i = InforJump.PointStep.Count - 1; i >= firstCollisionIndex; i--)
                    {
                        InforJump.PointStep.RemoveAt(i);
                    }
                }
            }
        }
    }
    int CustomRound(float value)
    {
        if (value > 0)
        {
            return (int)value;
        }
        else
        {
             return Mathf.FloorToInt(value);
        }
    }
    PlatForm GetPlatformToPoint(Vector2 P)
    {

        PlatForm platForm = new PlatForm();
        platForm.IDPlatform = -1;
        foreach (var Pl in PlatForms)
        {
            if(Pl.LeftPoint.y == P.y)
            {
                bool ok = false;
                foreach(InfoPoint Point in Pl.PointInPlatForm)
                {
                    if(P == Point.PointPosition)
                    {
                        ok = true;
                      platForm = Pl;
                        break;
                    }
                }
                if (ok)
                {
                    break;
                }
            }
        }
        return platForm;
    }
    bool CheckJumpPlatform(Vector2 PointStart , Vector2 PointTarget)
    {
       
        foreach (var pl in PlatForms)
        {
            if (pl.LeftPoint.y == PointTarget.y)
            {
                bool t1 = false;
                bool t2 = false;
                foreach(var p in pl.PointInPlatForm)
                {
                    if (PointStart == p.PointPosition)
                    {
                        t1 = true;
                    }
                    if (PointTarget == p.PointPosition)
                    {
                        t2 = true;
                    }
                }
                if(t1 && t2)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void DrawJumpTrajectory(Vector3 previousPoint, Vector2 force)
    {
        Vector2 LocationJump = new Vector2(previousPoint.x - 0.5f, previousPoint.y);
       
        int totalSteps = 30; 
        List<Vector2> PointsStep = new List<Vector2>();
        PointsStep.Add(new Vector2(previousPoint.x, previousPoint.y)); 
        float totalTime =1;
        float timeStep = totalTime / (totalSteps - 1);

        for (int i = 1; i < totalSteps; i++) 
        {
            float t = i * timeStep;
            float x = force.x * t;
            float y = (force.y * t) - (0.5f * gravityScale*9.8f * t * t);
            Vector2 currentPoint = new Vector2(previousPoint.x + x, previousPoint.y + y);
            PointsStep.Add(currentPoint);
          
        }
        // Tạo thông tin nhảy
        JumpInfo jif = new JumpInfo();
        jif.ForceJump = force;
        jif.PointStep = PointsStep;
        foreach (var pointi in infoPoints)
        {
            if (pointi.PointPosition == LocationJump)
            {
                pointi.jumpInfo.Add(jif);
            }
        }
        
    }
    void OnDrawGizmos()
        {
        if (ViewJumpPonit)
        {
            foreach (var pointi in infoPoints)
            {
                if (pointi.PointPosition == PositionPoint) {
                    foreach(JumpInfo Jump in pointi.jumpInfo){
                        Gizmos.color = Color.white;
                        for (int i = 0; i < Jump.PointStep.Count -2; i++)
                        {
                            Gizmos.DrawLine(Jump.PointStep[i], Jump.PointStep[i + 1]);
                        }
                         Gizmos.color = Color.red;
                        Gizmos.DrawLine(pointi.PointPosition+new Vector2(0.5f,0), Jump.EndJump + new Vector2(0.5f, 0));
                    }
                    Gizmos.color = Color.red;
                    if (pointi.LeftPoint.action == StateAction.fall)
                    {
                        Gizmos.DrawLine(pointi.PointPosition + new Vector2(0.5f, 0), pointi.LeftPoint.PosionGet + new Vector2(0.5f, 0));
                    }
                    if (pointi.RightPoint.action == StateAction.fall)
                    {
                        Gizmos.DrawLine(pointi.PointPosition + new Vector2(0.5f, 0), pointi.LeftPoint.PosionGet + new Vector2(0.5f, 0));
                    }
                    break;
                }
            }
        }
        if(DrawLine)
            if (infoPoints.Count > 0)
                foreach (var i in infoPoints)
                {
                    switch (i.SatePoint)
                    {
                        case SatePoint.LeftEdge:
                            {
                                Gizmos.color = UnityEngine.Color.yellow;
                                draw(i);
                            }
                            break;
                        case SatePoint.RightEdge:
                            {
                                Gizmos.color = UnityEngine.Color.yellow;
                                draw(i);
                            }
                            break;
                        case SatePoint.Solo:
                            {
                                Gizmos.color = UnityEngine.Color.green;
                                draw(i);
                            }
                            break;
                        case SatePoint.Flatform:
                            {
                                Gizmos.color = UnityEngine.Color.black;
                                draw(i);
                            }
                            break;
                    }

                }
            Gizmos.color = UnityEngine.Color.black;  
        Gizmos.color = UnityEngine.Color.white;
        if(DrawPlatform)
        foreach(var i in PlatForms)
           {
            Gizmos.DrawLine(i.LeftPoint + new Vector2(0,0.5f), i.RightPoint + new Vector2(1, 0.5f));
           }
        }
        void draw(InfoPoint i)
        {
            Gizmos.DrawSphere(i.PointPosition + new Vector2(0.5f, 0), 0.2f);
            Gizmos.color = UnityEngine.Color.blue;
            Gizmos.DrawLine(i.PointPosition + new Vector2(0.5f, 0), i.RightPoint.PosionGet + new Vector2(0.5f, 0));
            Gizmos.DrawLine(i.PointPosition + new Vector2(0.5f, 0), i.LeftPoint.PosionGet + new Vector2(0.5f, 0));
        Gizmos.color = UnityEngine.Color.red;
        foreach (var j in i.jumpInfo)
        {
            Gizmos.DrawLine(i.PointPosition + new Vector2(0.5f, 0), j.EndJump + new Vector2(0.5f, 0));
        }
        }


    public Dictionary<int, Dictionary<int, PathInfo>> LoadPathsFromJson(string filePath)
    {
        Dictionary<int, Dictionary<int, PathInfo>> allPaths = null;
        try
        {
            // Kiểm tra xem file có tồn tại không
            if (!File.Exists(filePath))
            {
                Debug.LogError("File không tồn tại: " + filePath);
                return null;
            }

            // Đọc JSON từ file
            string json = File.ReadAllText(filePath);



            // Chuyển đổi JSON thành Dictionary<int, Dictionary<int, PathInfo>>
            allPaths = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, PathInfo>>>(json);

            // Kiểm tra xem JSON có deserialize thành công không
            if (allPaths == null)
            {
                Debug.LogError("JSON không hợp lệ, không thể deserialize.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi khi đọc và deserializing JSON: " + ex.Message);
        }

        return allPaths;
    }
    public List<PlatForm> LoadPathsPlatForm(string filePath)
    {
        // Đọc JSON từ file
        string json = File.ReadAllText(filePath);

        List<PlatForm> platforms = JsonConvert.DeserializeObject<List<PlatForm>>(json);


        return platforms;
    }

    public List<Action> Findd(Vector2 start, Vector2 end)
    {
        if (FindWay == null || FindWay.Count == 0)
        {
            Debug.Log("Không đọc được Json");
            return new List<Action>();
        }
        return FindWayPointToPoint(new Vector2(CustomRound(start.x), CustomRound(start.y)), new Vector2(CustomRound(end.x), CustomRound(end.y)));
    }

    public List<Action> FindWayPointToPoint(Vector2 start, Vector2 end)
    {
        PlatForm PfStart = getPlatForm(FindPlatForm(start));
        PlatForm PfEnd = getPlatForm(FindPlatForm(end));
        List<Action> ListR = new List<Action>();
        if (PfStart == null || PfEnd == null)
        {
            Debug.LogError("Không tìm thấy nền tảng!");
            return ListR;
        }
        PathInfo WatRun = new PathInfo();
        if (FindWay.TryGetValue(PfStart.IDPlatform, out var innerDict) &&
            innerDict.TryGetValue(PfEnd.IDPlatform, out PathInfo savedWatRun))
        {
            WatRun = savedWatRun;
        }
        if (WatRun.Path == null || WatRun.Path.Count == 0)
        {

            return new List<Action>();
        }

        Vector2 PointNow = start;

        for (int i = 0; i < WatRun.Path.Count - 1; i++)
        {

            PlatForm platForm = getPlatForm(WatRun.Path[i]);

            List<Vector2> ListPointCanChange = new List<Vector2>();

            foreach (var action in platForm.actionChangePlatform)
            {
                if (action.PlatFormTargetID == WatRun.Path[i + 1]) // Sửa điều kiện
                {
                    ListPointCanChange.Add(new Vector2(action.PointStart.PointPositioni.x, action.PointStart.PointPositioni.y));
                }
            }

            if (ListPointCanChange.Count == 0)
            {
                Debug.LogError($"Không tìm thấy điểm chuyển trên nền tảng {WatRun.Path[i]}");
                return new List<Action>();
            }

            // Chọn điểm gần nhất
            Vector2 minVector = ListPointCanChange[0];
            float minDistance = MathF.Abs(PointNow.x - minVector.x);

            foreach (var point in ListPointCanChange)
            {
                float distance = MathF.Abs(PointNow.x - point.x);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minVector = point;
                }
            }

            // Di chuyển theo trục X
            if (minVector.x > PointNow.x)
            {
                for (int k = (int)PointNow.x; k <= minVector.x - 1; k++)
                {
                    Action A = new Action();
                    A.StateAction = StateAction.move;
                    A.Target = new Vector2(k + 1, PointNow.y);
                    A.Position = new Vector2(k, PointNow.y);
                    ListR.Add(A);
                }
            }
            else
            {
                for (int k = (int)PointNow.x; k >= minVector.x + 1; k--)
                {
                    Action A = new Action();
                    A.StateAction = StateAction.move;
                    A.Position = new Vector2(k, PointNow.y);
                    A.Target = new Vector2(k - 1, PointNow.y);
                    ListR.Add(A);
                }
            }
            // Tìm điểm đến trên nền tiếp theo

            bool found = false;

            foreach (var action in platForm.actionChangePlatform)
            {
                if (new Vector2(action.PointStart.PointPositioni.x, action.PointStart.PointPositioni.y) == minVector && action.PlatFormTargetID == WatRun.Path[i + 1])
                {
                    PointNow = new Vector2(action.TargetPoint.x, action.TargetPoint.y);
                    Action A = new Action();
                    A.StateAction = action.stateAction;
                    if (A.StateAction == StateAction.jump)
                    {
                        A.ForceJump = new Vector2(action.ForceJump.x, action.ForceJump.y);
                    }
                    A.Target = new Vector2(action.TargetPoint.x, action.TargetPoint.y);
                    A.Position = new Vector2(action.PointStart.PointPositioni.x, action.PointStart.PointPositioni.y);
                    ListR.Add(A);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogError($"Không tìm thấy điểm đến từ {PointNow} trên nền {WatRun.Path[i + 1]}");
                Debug.LogError(minVector);
                return new List<Action>();
            }
        }

        Vector2 minVectorr = end;
        if (end != PointNow)
        {
            if (minVectorr.x > PointNow.x)
            {
                for (int k = (int)PointNow.x; k <= minVectorr.x; k++)
                {
                    Action A = new Action();
                    A.StateAction = StateAction.move;
                    A.Position = new Vector2(k, PointNow.y);
                    A.Target = new Vector2(k + 1, PointNow.y);
                    ListR.Add(A);
                }
            }
            else
            {
                for (int k = (int)PointNow.x; k >= minVectorr.x; k--)
                {
                    Action A = new Action();
                    A.StateAction = StateAction.move;
                    A.Position = new Vector2(k, PointNow.y);
                    A.Target = new Vector2(k - 1, PointNow.y);
                    ListR.Add(A);
                }
            }
        }
        return ListR;
    }
    public int FindPlatForm(Vector2 Pposision)
    {
        Vector2 posision = new Vector2(CustomRound(Pposision.x), CustomRound(Pposision.y));
        int ID = -1;
        foreach (var platform in PlatForms)
        {
            bool found = false;
            foreach (var point in platform.PointInPlatForm)
            {
                if (point.PointPositioni.x == posision.x && point.PointPositioni.y == posision.y)
                {
                    ID = platform.IDPlatform;
                    found = true;
                    break;
                }
            }
            if (found)
            {
                break;
            }
        }
        return ID;
    }
    PlatForm getPlatForm(int id)
    {
        PlatForm tg = new PlatForm();
        foreach (var platform in PlatForms)
        {
            if (platform.IDPlatform == id)
            {
                tg = platform;
                break;
            }
        }
        return tg;
    }

}

