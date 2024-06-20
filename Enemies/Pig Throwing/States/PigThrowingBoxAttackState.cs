using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PigThrowingBoxAttackState : PigThrowingBoxBaseState
{
    public PigThrowingBoxAttackState(PigThrowingBoxController pigThrowing, string animName) : base (pigThrowing, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();

        pigThrowing.stats.timer = 0;
        pigThrowing.boxHasBeenPickedUp = false;
        Vector3 direction = pigThrowing.player.position - pigThrowing.transform.position;
        Debug.Log(direction);
        if (direction.x > 0 && !pigThrowing.isFacingRight) {
            pigThrowing.Flip();
        } else if (direction.x < 0 && pigThrowing.isFacingRight) {
            pigThrowing.Flip();
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        pigThrowing.stats.timer += Time.deltaTime;

        if (pigThrowing.stats.numberOfBoxesLeft > 0) {
            pigThrowing.InstantiateBox();
            pigThrowing.stats.numberOfBoxesLeft--;
        } else if (pigThrowing.stats.numberOfBoxesLeft <= 0 && pigThrowing.CheckForPlayer()) {
            pigThrowing.SwitchState(pigThrowing.findingBoxState);
        } else if (!pigThrowing.CheckForPlayer()) {
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
