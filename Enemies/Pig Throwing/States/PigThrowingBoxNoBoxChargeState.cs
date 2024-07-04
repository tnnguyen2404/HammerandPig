using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigThrowingBoxNoBoxChargeState : PigThrowingBoxBaseState
{
    public PigThrowingBoxNoBoxChargeState(PigThrowingBoxController pigThrowing, string animName) : base (pigThrowing, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        pigThrowing.target = pigThrowing.player.transform;
        Vector2 direction = pigThrowing.target.position - pigThrowing.transform.position;
        if (direction.x > 0 && !pigThrowing.isFacingRight || direction.x < 0 && pigThrowing.isFacingRight) {
            pigThrowing.Flip();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (pigThrowing.target == null)
            return;

        Vector2 targetPos = pigThrowing.GetTargetPosition();
        float distToTarget = Vector2.Distance(pigThrowing.transform.position, targetPos);

        if (distToTarget > pigThrowing.stats.meleeAttackRange && 
            !(pigThrowing.agent.PathGoal.HasValue &&  Vector2.Distance(pigThrowing.agent.PathGoal.Value, targetPos) <= pigThrowing.stats.travelStopRadius))
        {
            if (!pigThrowing.agent.UpdatePath(targetPos) && pigThrowing.stats.targetPredictionTime > 0)
            {
                pigThrowing.agent.UpdatePath(pigThrowing.target.position);
            }
        } else if (distToTarget <= pigThrowing.stats.meleeAttackRange)
        {
            pigThrowing.agent.Stop();
            pigThrowing.SwitchState(pigThrowing.meleeAttackState);
        } 
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
