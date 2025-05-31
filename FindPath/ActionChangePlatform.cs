using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SetupFinding;
[System.Serializable]
public class ActionChangePlatform 
{

    // Sử dụng ID thay vì tham chiếu trực tiếp đến PlatForm để tránh vòng lặp
    public int PlatFormTargetID; // ID của PlatForm mục tiêu
    public Vector2i TargetPoint;
    public Vector2i ForceJump;
    public StateAction stateAction;
    public InfoPoint PointStart;
}
