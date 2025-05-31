using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SetupFinding;
[System.Serializable]
public class InfoAction 
{
    public StateAction action;
    [JsonIgnore]
    public Vector2 PosionGet;
    public Vector2i PosionGeti = new Vector2i();
}
