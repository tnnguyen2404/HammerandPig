using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SetupFinding;
[System.Serializable]
public class JumpInfo 
{
    public Vector2i EndJumpi = new Vector2i();
    public Vector2i ForceJumpi = new Vector2i();
    [JsonIgnore]
    public Vector2 EndJump;
    [JsonIgnore]
    public Vector2 ForceJump;
    [JsonIgnore]
    public List<Vector2> PointStep = new List<Vector2>();
}
