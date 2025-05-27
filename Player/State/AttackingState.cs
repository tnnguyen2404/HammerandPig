using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : PlayerState
{
    public override void EnterState(PlayerController controller)
    {
        controller.rb.velocity = Vector2.zero;
        controller.Combat.AttackHitBox();
    }

    public override void ExitState(PlayerController controller)
    {
        
    }

    public override void UpdateState(PlayerController controller)
    {
        if (Mathf.Abs(controller.InputHandler.horizontalInput) > 0)
            controller.StateMachine.SwitchState(controller.runningState, controller);
    }

    public override void FixedUpdateState(PlayerController controller)
    {
        
    }
}
