using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigThrowingBombBaseState 
{
    protected PigThrowingBombController pigThrowingBomb;
    protected string animName;
    public PigThrowingBombBaseState(PigThrowingBombController pigThrowingBomb, string animName) {
        this.pigThrowingBomb = pigThrowingBomb;
        this.animName = animName;
    }

    public virtual void Enter() {
        pigThrowingBomb.anim.SetBool(animName, true);
    }

    public virtual void LogicUpdate() {

    }

    public virtual void PhysicsUpdate() {

    }

    public virtual void Exit() {
        pigThrowingBomb.anim.SetBool(animName, false);
    }

    public virtual void AnimationAttackTrigger() {

    }

    public virtual void AnimaitonFinishedTrigger() {
        
    }
}
