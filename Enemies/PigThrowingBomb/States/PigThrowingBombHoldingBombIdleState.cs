using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigThrowingBombHoldingBombIdleState : PigThrowingBombBaseState
{
    public PigThrowingBombHoldingBombIdleState(PigThrowingBombController pigThrowingBomb, string animName) : base (pigThrowingBomb, animName) 
    {

    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit() {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (pigThrowingBomb.CheckForAttackRange()) {
            pigThrowingBomb.SwitchState(pigThrowingBomb.rangeAttackState);
        } else if (!pigThrowingBomb.CheckForAttackRange() && pigThrowingBomb.CheckForPlayer()) {
            pigThrowingBomb.SwitchState(pigThrowingBomb.holdingBombChargeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
