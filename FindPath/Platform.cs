using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlatForm 
{
    public int IDPlatform;
    public List<InfoPoint> PointInPlatForm = new List<InfoPoint>();
    [JsonIgnore]
    public Vector2 LeftPoint = new Vector2();
    [JsonIgnore]
    public Vector2 RightPoint = new Vector2();

    // Sử dụng NonSerialized để tránh lỗi vòng lặp tuần tự hóa

    public List<ActionChangePlatform> actionChangePlatform = new List<ActionChangePlatform>();
}
