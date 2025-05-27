using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigCombat : MonoBehaviour
{
    private PigController controller;

    void Awake()
    {
        controller = GetComponent<PigController>();
    }

    void AttackHitBox()
    {
        Collider2D detectedObject = Physics2D.OverlapCircle(
            controller.enemyType.attackHitBox.position, controller.enemyType.attackRadius,
            controller.enemyType.whatIsDamageable);

        if (detectedObject != null)
        {
            IDamageable damageable = detectedObject.GetComponent<IDamageable>();
            damageable.TakeDamage(controller.enemyType.damage);
            
            IKnockBackable knockBackable = detectedObject.GetComponent<IKnockBackable>();
            knockBackable.ApplyKnockBack(new Vector2(controller.enemyType.knockBackForceX
            , controller.enemyType.knockBackForceY));
        }
    }
}
