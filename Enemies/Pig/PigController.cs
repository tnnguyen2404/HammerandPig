using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Random = UnityEngine.Random;
using PathBerserker2d;
using Unity.VisualScripting;

public class PigController : MonoBehaviour
{
    public EnemyTypeSO enemyType;
    public PlayerController playerController;
    
    public PigStateMachine StateMachine {get; private set;}
    
    public PigBaseState idleState;
    public PigBaseState chargeState;
    public PigBaseState detectPlayerState;
    public PigBaseState attackState;
    public PigBaseState getHitState;
    public PigBaseState deathState;
    
    public Rigidbody2D rb;
    public Animator anim;
    public LayerMask whatIsGround, whatIsPlayer, whatIsDamageable;
    public Transform groundCheck, wallCheck;
    public Transform attackHitBoxPos;
    public Transform player;
    public Transform target;
    public GameObject alert;
    public PigStatsSO stats;
    public int facingDirection = 1;
    public Vector2 startPos;
    public Vector2 curPos;
    [SerializeField] private UnityEngine.Vector3 offSet;
    public float curHealth;
    public int playerFacingDirection;
    public int numberOfBoxesLeft = 0;
    
    [Header("Boolean")]
    public bool isFacingRight;
    public bool playerInAttackRange;

    void Awake() 
    {
        
        idleState = new PigIdleState();
        detectPlayerState = new PigDetectPlayerState();
        chargeState = new PigChargeState();
        attackState = new PigAttackState();
        getHitState = new PigGetHitState();
        deathState = new PigDeathState();

        StateMachine = new PigStateMachine();
    }
    void Start()
    {
        StateMachine.Initialize(idleState, this);
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        StateMachine.Update(this);
    }

    void FixedUpdate() {
        StateMachine.FixedUpdate(this);
    }
    
    public bool CheckForAttackRange() {
        playerInAttackRange = Physics2D.Raycast(wallCheck.position, isFacingRight ? Vector2.right : Vector2.left, stats.attackRange, whatIsPlayer);
        return playerInAttackRange;
    }

    
    public void SwitchState(PigBaseState newState) {
        StateMachine.SwitchState(newState, this);
    }

    public int GetFacingDirection() {
        return facingDirection;
    }

    public void Instantiate(GameObject prefab, float torque, float dropForce) {
        Rigidbody2D itemRb = Instantiate(prefab, transform.position, quaternion.identity).GetComponent<Rigidbody2D>();
        Vector2 dropVelocity = new Vector2(Random.Range(0.5f,-0.5f), 1) * dropForce;
        itemRb.AddForce(dropVelocity, ForceMode2D.Impulse);
        itemRb.AddTorque(torque, ForceMode2D.Impulse);
    }
}
