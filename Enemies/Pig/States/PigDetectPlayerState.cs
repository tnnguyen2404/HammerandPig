using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigDetectPlayerState : PigBaseState
{
    public override void EnterState(PigController controller)
    {
       controller.enemyType.alert.SetActive(true);
    }

    public override void UpdateState(PigController controller)
    {
        if(Time.deltaTime > controller.enemyType.detectionWaitTime)
            controller.SwitchState(controller.chargeState);
        else if (controller.CheckForAttackRange())
            controller.SwitchState(controller.attackState);
    }

    public override void FixedUpdateState(PigController controller)
    {
        
    }

    public override void ExitState(PigController controller)
    {
        controller.enemyType.alert.SetActive(false);
    }
}
