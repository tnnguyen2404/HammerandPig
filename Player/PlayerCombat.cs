using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackHitPosBox;
    public LayerMask whatIsDamageable;
    
    [SerializeField] private float knockBackSpeedX, knockBackSpeedY;
    
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
        Collider2D detectedObject = Physics2D.OverlapCircle(attackHitPosBox.position, attackRadius, whatIsDamageable);

        if (detectedObject != null)
        {
            bool playerOnLeft = transform.position.x < detectedObject.transform.position.x;
            
            IDamageable damageable = detectedObject.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(damage);
            
                
            
            /*IKnockBackable knockBackable = detectedObject.GetComponent<IKnockBackable>();
            if (knockBackable != null)
                knockBackable.ApplyKnockBack(new Vector2(knockBackSpeedX, knockBackSpeedY));*/
        }
    }
}
