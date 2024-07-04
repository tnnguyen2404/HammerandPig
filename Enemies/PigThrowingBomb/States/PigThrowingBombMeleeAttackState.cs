using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigThrowingBombMeleeAttackState : PigThrowingBombBaseState
{
    public PigThrowingBombMeleeAttackState(PigThrowingBombController pigThrowingBomb, string animName) : base (pigThrowingBomb, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void AnimationAttackTrigger()
    {
        base.AnimationAttackTrigger();
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(pigThrowingBomb.attackHitBoxPos.position, pigThrowingBomb.stats.meleeAttackRadius, pigThrowingBomb.whatIsPlayer);
        pigThrowingBomb.stats.attackDetails[0] = pigThrowingBomb.stats.meleeAttackDamage;
        pigThrowingBomb.stats.attackDetails[1] = pigThrowingBomb.transform.position.x;

        foreach (Collider2D collider in detectedObjects) {
            collider.transform.SendMessage("TakeDamage", pigThrowingBomb.stats.attackDetails);
        }
    }

    public override void AnimaitonFinishedTrigger()
    {
        base.AnimaitonFinishedTrigger();
        pigThrowingBomb.SwitchState(pigThrowingBomb.playerDetectedState);
    }
}
