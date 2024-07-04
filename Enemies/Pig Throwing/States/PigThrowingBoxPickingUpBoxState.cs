using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigThrowingBoxPickingUpBoxState : PigThrowingBoxBaseState
{
    public PigThrowingBoxPickingUpBoxState(PigThrowingBoxController pigThrowing, string animName) : base (pigThrowing, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        pigThrowing.numberOfBoxesLeft++;
        pigThrowing.boxHasBeenPickedUp = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Time.time >= pigThrowing.stateTime + pigThrowing.stats.pickingUpBoxWaitTime) {
            pigThrowing.SwitchState(pigThrowing.holdingBoxIdleState);
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
