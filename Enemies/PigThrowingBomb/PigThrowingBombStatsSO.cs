using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PigThrowingBombStatsSO")]
public class PigThrowingBombStatsSO : ScriptableObject
{
    [Header("Prefabs")]
    public GameObject deathParticle;

    [Header("Item Drop Variable")]
    public GameObject[] itemDrops;
    public float dropForce;
    public float torque;

    [Header("General Stats")]
    public float maxHealth;
    public float curHealth;
    public float jumpSpeed;
    public float fallSpeed;

    [Header("CheckSurrounding")]
    public float groundCheckDistance;
    public float wallCheckDistance;

    [Header("Range Attack State")]
    public float attackRange;
    public float boxSpeed;
    public float timer;

    [Header("Box Charge State")]
    public float chargeSpeed;
    public float chargeTime;
    public float chargeDuration;
    public float closeEnoughRadius;
    public float travelStopRadius;
    public float targetPredictionTime;

    [Header("Player Detection State")]
    public float playerDetectDistance;
    public float playerDetectedWaitTime;
    public float detectionPauseTime;  

    [Header("Picking Up Box State")]
    public float pickingUpBoxWaitTime;

    [Header("Picking Up Box Idle State")]
    public float idleTime;
    public float pickingUpBoxRange;

    [Header("Finding Box State")]
    public float findingBoxDistance;
    public float runSpeed;
    public float jumpRange;
    
    [Header("Get Hit State")]
    public float knockBackSpeedX;
    public float knockBackSpeedY;

    [Header("Back Up State")]
    public float dodgeForce;
    public Vector2 dodgeAngle;
    public float dodgeDetectDistance;

    [Header("No Box Charge State")]
    public float noBoxChargeSpeed;

    [Header("Melee Attack State")]
    public float meleeAttackRange;
    public float meleeAttackRadius;
    public float meleeAttackDamage;
    public float[] attackDetails = new float[2];
}
