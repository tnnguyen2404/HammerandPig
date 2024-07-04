using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PigThrowingBombRangeAttackState : PigThrowingBombBaseState
{
    public PigThrowingBombRangeAttackState(PigThrowingBombController pigThrowingBomb, string animName) : base (pigThrowingBomb, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();

        pigThrowingBomb.target = pigThrowingBomb.player.transform;
        pigThrowingBomb.stats.timer = 0;
        pigThrowingBomb.boxHasBeenPickedUp = false;
        Vector3 direction = pigThrowingBomb.player.position - pigThrowingBomb.transform.position;
        Debug.Log(direction);
        if (direction.x > 0 && !pigThrowingBomb.isFacingRight) {
            pigThrowingBomb.Flip();
        } else if (direction.x < 0 && pigThrowingBomb.isFacingRight) {
            pigThrowingBomb.Flip();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        pigThrowingBomb.stats.timer += Time.deltaTime;

        if (pigThrowingBomb.numberOfBoxesLeft > 0) {
            pigThrowingBomb.InstantiateBomb();
            pigThrowingBomb.numberOfBoxesLeft--;
        } else if (pigThrowingBomb.numberOfBoxesLeft <= 0 && pigThrowingBomb.CheckForPlayer()) {
            pigThrowingBomb.SwitchState(pigThrowingBomb.playerDetectedState);
        } else if (!pigThrowingBomb.CheckForPlayer()) {
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
