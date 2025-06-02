using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyType", menuName = "Enemies/Enemy Type")]

public class EnemyTypeSO : ScriptableObject
{
    [Header("Basic Info")] 
    public string enemyName;
    public RuntimeAnimatorController animController;
    public GameObject alert;

    [Header("Stats")] 
    public int maxHealth;
    public float jumpForce;
    
    [Header("Boolean")]
    public bool isGrounded;
    public bool isTouchingWall;
    public bool isMoving;
    public bool isJumping;
    public bool isAlive;
    public bool isAttacking;
    public bool isCharging;
    public bool isFacingRight;
    public bool isProcessing;
    public bool isAttacked;

    [Header("Movement")] 
    public int facingDirection;
    public float moveSpeed;

    [Header("SurroundingsCheck")] 
    public float groundCheckDistance;
    public float wallCheckDistance;
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform wallCheck;
    
    [Header("Player Detection State")]
    public float detectRange;
    public float detectionWaitTime;

    [Header("Attack State")] 
    public Transform attackHitBox;
    public LayerMask whatIsDamageable;
    public float knockBackForceX, knockBackForceY;
    public int damage;
    public float attackRange;
    public float attackRadius;
}
