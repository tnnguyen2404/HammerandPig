using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class WayToTagert 
{
    [JsonIgnore]
    public Vector2 PlatFormStartID;
    public Vector2 PlatFormTagertID;// ID của PlatForm mục tiêu
    public StateAction stateAction;
    public InfoPoint PointStart;
}
