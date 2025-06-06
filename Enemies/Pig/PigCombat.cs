using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigCombat : MonoBehaviour
{
    private PigController controller;

    public bool isAttacking;

    void Awake()
    {
        controller = GetComponent<PigController>();
    }

    public void AttackHitBox()
    {
        Collider2D detectedObject = Physics2D.OverlapCircle(
            controller.attackHitBox.position, controller.enemyType.attackRadius,
            controller.enemyType.whatIsDamageable);

        if (detectedObject != null)
        {
            IDamageable damageable = detectedObject.GetComponent<IDamageable>();
            
            if (damageable != null)
                damageable.TakeDamage(controller.enemyType.damage);
            
            //IKnockBackable knockBackable = detectedObject.GetComponent<IKnockBackable>();
            //knockBackable.ApplyKnockBack(new Vector2(controller.enemyType.knockBackForceX
            //, controller.enemyType.knockBackForceY));
        }
    }
}
