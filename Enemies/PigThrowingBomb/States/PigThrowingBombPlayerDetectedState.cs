using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigThrowingBombPlayerDetectedState : PigThrowingBombBaseState
{
    public PigThrowingBombPlayerDetectedState(PigThrowingBombController pigThrowingBomb, string animName) : base (pigThrowingBomb, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log(animName);
        pigThrowingBomb.alert.SetActive(true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Time.time >= pigThrowingBomb.stateTime + pigThrowingBomb.stats.playerDetectedWaitTime && !pigThrowingBomb.boxHasBeenPickedUp) {
            pigThrowingBomb.SwitchState(pigThrowingBomb.findingBombState);
        } else if (Time.time >= pigThrowingBomb.stateTime + pigThrowingBomb.stats.playerDetectedWaitTime && pigThrowingBomb.boxHasBeenPickedUp) {
            if (pigThrowingBomb.CheckForPlayer() && !pigThrowingBomb.CheckForAttackRange()) {
                pigThrowingBomb.SwitchState(pigThrowingBomb.chargeState);
            } else if (pigThrowingBomb.CheckForPlayer() && pigThrowingBomb.CheckForAttackRange()) {
                pigThrowingBomb.SwitchState(pigThrowingBomb.rangeAttackState);
            }
        }
           
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
        pigThrowingBomb.alert.SetActive(false);
    }
}
