using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyType", menuName = "Enemies/Enemy Type")]

public class EnemyTypeSO : ScriptableObject
{
    [Header("Basic Info")] 
    public string enemyName;
    public RuntimeAnimatorController animController;

    [Header("Stats")] 
    public int maxHealth;
    public float moveSpeed;
    public float jumpForce;
    public int damage;
    public float attackRange;
    public float attackRadius;
    
    [Header("Boolean")]
    public bool isGrounded;
    public bool isTouchingWall;
    public bool isMoving;
    public bool isJumping;
    public bool isAlive;
    public bool isAttacking;
    public bool isCharging = false;
    public bool isFacingRight;
    public bool playerDetected;
    public bool playerInAttackRange;

    [Header("Movement")] 
    public int facingDirection;

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
    public float detectionPauseTime;

    [Header("Attack State")] 
    public Transform attackHitBox;
    public LayerMask whatIsDamageable;
    public float knockBackForceX, knockBackForceY;
}
