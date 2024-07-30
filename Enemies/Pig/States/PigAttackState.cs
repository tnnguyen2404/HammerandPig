using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigAttackState : PigBaseState
{
    public PigAttackState(PigController pig, string animName) : base (pig, animName) 
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
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(pig.attackHitBoxPos.position, pig.stats.attackRadius, pig.whatIsDamageable);
        pig.stats.attackDetails[0] = pig.stats.attackDamage;
        pig.stats.attackDetails[1] = pig.transform.position.x;

        foreach (Collider2D collider in detectedObjects) {
            collider.transform.SendMessage("TakeDamage", pig.stats.attackDetails);
        }
    }

    public override void AnimaitonFinishedTrigger()
    {
        base.AnimaitonFinishedTrigger();
        pig.SwitchState(pig.detectPlayerState);
    }

}
