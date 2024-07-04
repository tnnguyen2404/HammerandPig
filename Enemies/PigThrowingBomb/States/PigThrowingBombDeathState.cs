using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigThrowingBombDeathState : PigThrowingBombBaseState
{
    public PigThrowingBombDeathState(PigThrowingBombController pigThrowingBomb, string animName) : base (pigThrowingBomb, animName) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        DropItems();
        pigThrowingBomb.gameObject.SetActive(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void DropItems() {
        foreach (var item in pigThrowingBomb.stats.itemDrops) {
            pigThrowingBomb.InstantiateItemDrop(item, pigThrowingBomb.stats.dropForce, pigThrowingBomb.stats.torque);
        }
    }
}
