using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : PlayerState
{
    public override void EnterState(PlayerController controller)
    {
        if (controller.InputHandler.isAttacking && controller.Combat.attackTimer >= controller.Combat.attackCd)
        {
            controller.AnimationController.anim.SetTrigger("Attack");
            controller.Combat.attackTimer = 0;
        }
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
