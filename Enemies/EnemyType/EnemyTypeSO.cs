using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyType", menuName = "Enemies/Enemy Type")]

public class EnemyTypeSO : ScriptableObject
{
    [Header("Basic Info")] 
    public string enemyName;
    
    [Header("Stats")] 
    public int maxHealth;
    
    [Header("Boolean")]
    public bool isAlive;
    public bool isFacingRight;
    public bool isAttacked;
 
    [Header("Movement")] 
    public int facingDirection;
    public float moveSpeed;
    public float jumpForce;
    public float minJumpTime;
    public float maxJumpTime;
    public float jumpExtraHeight;

    [Header("GroundCheck")] 
    public float groundCheckRadius;
    public LayerMask groundLayer;
    
    [Header("Player Detection State")]
    public float detectRange;
    public float detectionWaitTime;

    [Header("Attack State")] 
    public LayerMask whatIsDamageable;
    public float knockBackForceX, knockBackForceY;
    public int damage;
    public float attackRange;
    public float attackRadius;
    
    [Header("Item Drop Variable")]
    public float dropForce;
    public float torque;
}
