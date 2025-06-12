using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;
[System.Serializable]
public class ColliderChild 
{
    public string Name;
    public Vector2 size;
    public LayerMask mask;
    public Vector2 Direction;
    public bool IsCol;
    public bool Draw = false;
    public bool FolowScale=false;
    public bool CheckWall = false;

}
