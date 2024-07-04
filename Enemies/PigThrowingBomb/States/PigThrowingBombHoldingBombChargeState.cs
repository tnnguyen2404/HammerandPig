using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathBerserker2d;
public class PigThrowingBombHoldingBombChargeState : PigThrowingBombBaseState
{
    public PigThrowingBombHoldingBombChargeState(PigThrowingBombController pigThrowingBomb, string animName) : base (pigThrowingBomb, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        pigThrowingBomb.target = pigThrowingBomb.player.transform;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (pigThrowingBomb.target == null)
            return;

        Vector2 targetPos = pigThrowingBomb.GetTargetPosition();
        float distToTarget = Vector2.Distance(pigThrowingBomb.transform.position, targetPos);

        if (distToTarget > pigThrowingBomb.stats.attackRange && 
            !(pigThrowingBomb.agent.PathGoal.HasValue &&  Vector2.Distance(pigThrowingBomb.agent.PathGoal.Value, targetPos) <= pigThrowingBomb.stats.travelStopRadius))
        {
            if (!pigThrowingBomb.agent.UpdatePath(targetPos) && pigThrowingBomb.stats.targetPredictionTime > 0)
            {
                pigThrowingBomb.agent.UpdatePath(pigThrowingBomb.target.position);
            }
        } else if (distToTarget <= pigThrowingBomb.stats.attackRange)
        {
            pigThrowingBomb.agent.Stop();
            pigThrowingBomb.SwitchState(pigThrowingBomb.rangeAttackState);
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
