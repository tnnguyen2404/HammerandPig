using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState 
{
    public abstract void EnterState(PlayerController playerController);
    public abstract void UpdateState(PlayerController playerController);
    public abstract void FixedUpdateState(PlayerController playerController);
    public abstract void ExitState(PlayerController playerController);
}
