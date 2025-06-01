using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public abstract class PigBaseState
{
    public abstract void EnterState(PigController controller);
    public abstract void ExitState(PigController controller);
    public abstract void UpdateState(PigController controller);
    public abstract void FixedUpdateState(PigController controller);
}
