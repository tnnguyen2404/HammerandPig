using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigChargeState : PigBaseState
{
    public override void EnterState(PigController controller)
    {
        controller.Movement.StartPath();
        controller.Movement.isCharging = true;
    }

    public override void ExitState(PigController controller)
    {
       controller.Movement.isCharging = false;
    }

    public override void UpdateState(PigController controller)
    {
        if (controller.CheckForAttackRange())
            controller.SwitchState(controller.attackState);
    }

    public override void FixedUpdateState(PigController controller)
    {
        controller.Movement.Movement();
    }
}
