using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SetupFinding;
[System.Serializable]
public class InfoPoint
{
    public SatePoint SatePoint;
    [JsonIgnore]
    public Vector2 PointPosition;
    public Vector2i PointPositioni = new Vector2i();
    [JsonIgnore]
    public InfoAction LeftPoint = new InfoAction();
    [JsonIgnore]
    public InfoAction RightPoint = new InfoAction();
    public List<JumpInfo> jumpInfo = new List<JumpInfo>();

}
