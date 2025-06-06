using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigAttackState : PigBaseState
{
    public override void EnterState(PigController controller)
    {
        controller.Combat.isAttacking = true;
    }

    public override void UpdateState(PigController controller)
    {
        
    }

    public override void FixedUpdateState(PigController controller)
    {
        
    }

    public override void ExitState(PigController controller)
    {
        controller.Combat.isAttacking = false;
    }
}
