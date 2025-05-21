using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform AttackHitPosBox;
    public LayerMask whatIsDamageable;
    
    public float attackTimer = 1.1f;
    public float attackCd = 1f;
    
    private float attackRadius = 0.45f;
    private int damage = 1;

    void Update()
    {
        attackTimer += Time.deltaTime;
    }

    public void AttackHitBox()
    {
        Collider2D detectedObject = Physics2D.OverlapCircle(AttackHitPosBox.position, attackRadius, whatIsDamageable);

        if (detectedObject != null)
        {
            IDamageable damageable = detectedObject.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(damage);
        }
        
        
    }
}
