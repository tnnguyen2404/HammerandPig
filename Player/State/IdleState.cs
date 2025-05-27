using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public override void EnterState(PlayerController controller)
    {
        
    }

    public override void ExitState(PlayerController controller)
    {
        
    }

    public override void UpdateState(PlayerController controller)
    {
        if (Mathf.Abs(controller.InputHandler.horizontalInput) > 0.1f)
            controller.StateMachine.SwitchState(controller.runningState, controller);
        
        if (controller.InputHandler.jumpInput)
            controller.StateMachine.SwitchState(controller.jumpState, controller);
    }

    public override void FixedUpdateState(PlayerController controller)
    {
        
    }
}
