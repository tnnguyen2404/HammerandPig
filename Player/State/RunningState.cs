using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : PlayerState
{
    public override void EnterState(PlayerController controller)
    {
        
    }

    public override void ExitState(PlayerController controller)
    {
        
    }

    public override void UpdateState(PlayerController controller)
    {
        controller.Movement.Move(controller.InputHandler.horizontalInput);
        
        if (controller.InputHandler.jumpInput)
            controller.StateMachine.SwitchState(controller.jumpState, controller);
        
        if (controller.InputHandler.horizontalInput == 0) 
            controller.StateMachine.SwitchState(controller.idleState, controller);
    }

    public override void FixedUpdateState(PlayerController controller)
    {
        
    }
}
