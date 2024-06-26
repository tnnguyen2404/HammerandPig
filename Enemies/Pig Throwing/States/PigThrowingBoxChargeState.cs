using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathBerserker2d;
public class PigThrowingBoxChargeState : PigThrowingBoxBaseState
{
    public PigThrowingBoxChargeState(PigThrowingBoxController pigThrowing, string animName) : base (pigThrowing, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        pigThrowing.target = pigThrowing.player.transform;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (pigThrowing.target == null)
            return;

        Vector2 targetPos = pigThrowing.GetTargetPosition();
        float distToTarget = Vector2.Distance(pigThrowing.transform.position, targetPos);

        if (distToTarget > pigThrowing.stats.attackRange && 
            !(pigThrowing.agent.PathGoal.HasValue &&  Vector2.Distance(pigThrowing.agent.PathGoal.Value, targetPos) <= pigThrowing.stats.travelStopRadius))
        {
            if (!pigThrowing.agent.UpdatePath(targetPos) && pigThrowing.stats.targetPredictionTime > 0)
            {
                pigThrowing.agent.UpdatePath(pigThrowing.target.position);
            }
        } else if (distToTarget <= pigThrowing.stats.attackRange)
        {
            pigThrowing.agent.Stop();
            pigThrowing.SwitchState(pigThrowing.attackState);
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
