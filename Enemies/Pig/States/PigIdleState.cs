using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigIdleState : PigBaseState
{
    public override void EnterState(PigController controller)
    {
        
    }

    public override void UpdateState(PigController controller)
    {
        if(controller.DetectPlayer())
            controller.SwitchState(controller.detectPlayerState);
    }

    public override void ExitState(PigController controller)
    {
        
    }

    public override void FixedUpdateState(PigController controller)
    {
        
    }
}
