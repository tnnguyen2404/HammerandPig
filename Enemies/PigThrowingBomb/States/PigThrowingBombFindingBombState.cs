using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using PathBerserker2d;

public class PigThrowingBombFindingBombState : PigThrowingBombBaseState
{
    public PigThrowingBombFindingBombState(PigThrowingBombController pigThrowingBomb, string animName) : base(pigThrowingBomb, animName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");

        if (bombs.Length == 0) {
            pigThrowingBomb.SwitchState(pigThrowingBomb.chargeState);
        }

        GameObject closestBomb = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject bomb in bombs)
        {
            float distance = Vector2.Distance(pigThrowingBomb.transform.position, bomb.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBomb = bomb;
            }
        }

        pigThrowingBomb.goal = closestBomb.transform;
        Vector2 direction = pigThrowingBomb.goal.position - pigThrowingBomb.transform.position;
        if (direction.x > 0 && !pigThrowingBomb.isFacingRight || direction.x < 0 && pigThrowingBomb.isFacingRight) {
            pigThrowingBomb.Flip();
        }

        if (Vector2.Distance(pigThrowingBomb.goal.position, pigThrowingBomb.agent.transform.position) > 0.5f && (pigThrowingBomb.agent.IsIdle || pigThrowingBomb.goal.hasChanged))
            {
                pigThrowingBomb.goal.hasChanged = false;
                pigThrowingBomb.agent.UpdatePath(pigThrowingBomb.goal.position);
            }

        if (Vector2.Distance(pigThrowingBomb.transform.position, closestBomb.transform.position) <= pigThrowingBomb.stats.pickingUpBoxRange)
        {
            pigThrowingBomb.SwitchState(pigThrowingBomb.pickingUpBombState);
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
    
