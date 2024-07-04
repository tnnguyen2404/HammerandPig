using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using PathBerserker2d;

public class PigThrowingBoxFindingBoxState : PigThrowingBoxBaseState
{
    public PigThrowingBoxFindingBoxState(PigThrowingBoxController pigThrowing, string animName) : base(pigThrowing, animName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");

        if (boxes.Length == 0) {
            pigThrowing.SwitchState(pigThrowing.noBoxChargeState);
        }

        GameObject closestBox = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject box in boxes)
        {
            float distance = Vector2.Distance(pigThrowing.transform.position, box.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBox = box;
            }
        }

        pigThrowing.goal = closestBox.transform;
        Vector2 direction = pigThrowing.goal.position - pigThrowing.transform.position;
        if (direction.x > 0 && !pigThrowing.isFacingRight || direction.x < 0 && pigThrowing.isFacingRight) {
            pigThrowing.Flip();
        }

        if (Vector2.Distance(pigThrowing.goal.position, pigThrowing.agent.transform.position) > 0.5f && (pigThrowing.agent.IsIdle || pigThrowing.goal.hasChanged))
            {
                pigThrowing.goal.hasChanged = false;
                pigThrowing.agent.UpdatePath(pigThrowing.goal.position);
            }

        if (Vector2.Distance(pigThrowing.transform.position, closestBox.transform.position) <= pigThrowing.stats.pickingUpBoxRange)
        {
            pigThrowing.SwitchState(pigThrowing.pickingUpBoxState);
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
    
