using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigChargeState : PigBaseState
{
    public PigChargeState(PigController pig, string animName) : base (pig, animName) 
    {

    }

    public override void Enter() {
        base.Enter();
        pig.stats.chargeTime = 0f;
        pig.target = pig.player.transform;
    }

    public override void LogicUpdate() {
        base.LogicUpdate();
        if (pig.target == null)
            return;

        Vector2 targetPos = pig.GetTargetPosition();
        float distToTarget = Vector2.Distance(pig.transform.position, targetPos);

        if (distToTarget > pig.stats.attackRange && 
            !(pig.agent.PathGoal.HasValue &&  Vector2.Distance(pig.agent.PathGoal.Value, targetPos) <= pig.stats.travelStopRadius))
        {
            if (!pig.agent.UpdatePath(targetPos) && pig.stats.targetPredictionTime > 0)
            {
                pig.agent.UpdatePath(pig.target.position);
            }
        } else if (distToTarget <= pig.stats.attackRange)
        {
            pig.agent.Stop();
            pig.SwitchState(pig.attackState);
        }    
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }

    public override void Exit() {
        base.Exit();
    }

    void ReturnToOriginalPos() {
        Vector2 direction = (pig.startPos - (Vector2)pig.transform.position).normalized;
        pig.rb.velocity = direction * pig.stats.chargeSpeed;

        if (direction.x > 0 && !pig.isFacingRight || direction.x < 0 && pig.isFacingRight) {
            FlipSprite();
        }

        if (Vector2.Distance(pig.transform.position, pig.startPos) < 0.1f) {
            pig.stats.chargeTime = 0f;
            pig.rb.velocity = Vector2.zero;
        }
    }

    void FlipSprite() {
        pig.facingDirection *= -1;
        pig.isFacingRight = !pig.isFacingRight;
        pig.transform.Rotate(0, 180, 0);
    }
}
