using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigChargeState : PigBaseState
{
    private float repathTimer = 0f;
    private float repathInterval = 0.3f;
    
    public override void EnterState(PigController controller)
    {
        repathTimer = 0f;
        PathToPlayer(controller);
    }

    public override void ExitState(PigController controller)
    {
       controller.Movement.StopPath();
    }

    public override void UpdateState(PigController controller)
    {
        repathTimer += Time.deltaTime;
        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            if (!controller.Jump.isJumping && controller.Jump.isGrounded)
                PathToPlayer(controller);
        }
        
        if (controller.CheckForAttackRange())
            controller.SwitchState(controller.attackState);
    }

    public override void FixedUpdateState(PigController controller)
    {
        
    }
    
    private void PathToPlayer(PigController controller)
    {
        Vector2 playerPos = new Vector2(
            Mathf.Round(controller.player.transform.position.x),
            Mathf.Round(controller.player.transform.position.y)
        );
        controller.Movement.MoveTo(playerPos);
    }
}
