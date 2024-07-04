using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigThrowingBombIdleState : PigThrowingBombBaseState
{
    public PigThrowingBombIdleState(PigThrowingBombController pigThrowingBomb, string animName) : base (pigThrowingBomb, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (pigThrowingBomb.CheckForPlayer()) {
            pigThrowingBomb.SwitchState(pigThrowingBomb.playerDetectedState);
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
