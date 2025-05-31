using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static SetupFinding;

public class Find : MonoBehaviour
{
    Dictionary<int, Dictionary<int, PathInfo>> FindWay;
    List<PlatForm> PlatForms = new List<PlatForm>();
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
    private void Start()
    {
        FindWay = LoadPathsFromJson("pathsway.json");
        PlatForms = LoadPathsPlatForm("pathsPlatForm.json");
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

        for (int i = 0; i < WatRun.Path.Count -1 ; i++)
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
                for (int k = (int)PointNow.x; k <= minVector.x-1; k++)
                {
                    Action A = new Action();
                    A.SateAction= StateAction.move;
                    A.Tagert = new Vector2(k + 1, PointNow.y);
                    A.Position = new Vector2(k, PointNow.y);
                    ListR.Add(A);
                }
            }
            else
            {
                for (int k = (int)PointNow.x; k >= minVector.x +1; k--)
                {
                    Action A = new Action();
                    A.SateAction = StateAction.move;
                    A.Position = new Vector2(k, PointNow.y);
                    A.Tagert = new Vector2(k - 1, PointNow.y);
                    ListR.Add(A);
                }
            }
            // Tìm điểm đến trên nền tiếp theo
           
            bool found = false;
            
            foreach (var action in platForm.actionChangePlatform)
            {
                if (new Vector2(action.PointStart.PointPositioni.x, action.PointStart.PointPositioni.y) == minVector && action.PlatFormTargetID== WatRun.Path[i+1])
                {
                    PointNow = new Vector2( action.TargetPoint.x, action.TargetPoint.y);
                    Action A = new Action();
                    A.SateAction = action.stateAction;
                    if(A.SateAction == StateAction.jump)
                    {
                      A.ForceJump = new Vector2(action.ForceJump.x,action.ForceJump.y);
                    }
                    A.Tagert= new Vector2(action.TargetPoint.x, action.TargetPoint.y);
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
                    A.SateAction = StateAction.move;
                    A.Position = new Vector2(k, PointNow.y);
                    A.Tagert = new Vector2(k + 1, PointNow.y);
                    ListR.Add(A);
                }
            }
            else
            {
                for (int k = (int)PointNow.x; k >= minVectorr.x; k--)
                {
                    Action A = new Action();
                    A.SateAction = StateAction.move;
                    A.Position = new Vector2(k, PointNow.y);
                    A.Tagert = new Vector2(k - 1, PointNow.y);
                    ListR.Add(A);
                }
            }
        }
        return ListR;
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
        foreach (var platform in PlatForms) { 
              if(platform.IDPlatform == id)
            {
                tg = platform;
                break;
            } 
        } 
        return tg;
    }


}
