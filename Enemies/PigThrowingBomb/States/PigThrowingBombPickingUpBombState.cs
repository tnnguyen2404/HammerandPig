using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigThrowingBombPickingUpBombState : PigThrowingBombBaseState
{
    public PigThrowingBombPickingUpBombState(PigThrowingBombController pigThrowingBomb, string animName) : base (pigThrowingBomb, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        pigThrowingBomb.numberOfBoxesLeft++;
        pigThrowingBomb.boxHasBeenPickedUp = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Time.time >= pigThrowingBomb.stateTime + pigThrowingBomb.stats.pickingUpBoxWaitTime) {
            pigThrowingBomb.SwitchState(pigThrowingBomb.holdingBombIdleState);
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
