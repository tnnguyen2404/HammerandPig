using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class JumpState : PlayerState
{
    public override void EnterState(PlayerController controller)
    {
        controller.Jump.Jump();
    }

    public override void ExitState(PlayerController controller)
    {
        
    }

    public override void UpdateState(PlayerController controller)
    {
        if (Mathf.Abs(controller.rb.velocity.x) > 0.1f)
            controller.StateMachine.SwitchState(controller.idleState, controller);
    }

    public override void FixedUpdateState(PlayerController controller)
    {
        
    }
}
