using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigGetHitState : PigBaseState
{
    public PigGetHitState(PigController pig, string animName) : base (pig, animName)
    {
       
    }

    public override void Enter()
    {
        base.Enter();
        KnockBack();
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
    }

    public override void AnimaitonFinishedTrigger()
    {
        base.AnimaitonFinishedTrigger();
        pig.SwitchState(pig.detectPlayerState);
    }

    private void KnockBack() {
        pig.rb.velocity = new Vector2 (pig.stats.knockBackSpeedX * pig.playerFacingDirection, pig.stats.knockBackSpeedY);
    }

    private void Die() {
        //Destroy(gameObject);
    }
}
