using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SetupFinding;
[System.Serializable]
public class Action 
{
    public Vector2 Position;
    public StateAction StateAction;
    public Vector2 ForceJump;
    public Vector2 Target;
}
