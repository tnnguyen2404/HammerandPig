using System.Collections.Generic;
using UnityEngine;

public class PigChargeState : PigBaseState
{
    private float repathTimer;
    private const float repathInterval = 0.5f;

    public override void EnterState(PigController controller)
    {
        UpdatePath(controller);
        repathTimer = repathInterval;
    }

    public override void ExitState(PigController controller) { }

    public override void UpdateState(PigController controller)
    {
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            UpdatePath(controller);
            repathTimer = repathInterval;
        }

        controller.Movement.FollowPath();
    }

    public override void FixedUpdateState(PigController controller) { }

    private void UpdatePath(PigController controller)
    {
        Node start = AStarManager.Instance.GetClosestNode(controller.transform.position);
        Node end = AStarManager.Instance.GetClosestNode(controller.player.transform.position);
        List<Node> path = AStarManager.Instance.GeneratePath(start, end);
        controller.Movement.SetPlayer(controller.player.transform);
        controller.Movement.SetPath(path);
    }
}