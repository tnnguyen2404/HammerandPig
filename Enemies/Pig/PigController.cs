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
    
    public PigMovement Movement {get; private set;}
    public PigCombat Combat {get; private set;}
    public PigJump Jump {get; private set;}
    
    public PigBaseState idleState;
    public PigBaseState chargeState;
    public PigBaseState detectPlayerState;
    public PigBaseState attackState;
    public PigBaseState deathState;

    private Rigidbody2D rb;
    private Animator anim;
    
    [SerializeField] private SetupFinding setupFinding;
    [SerializeField] private Find finder;
    public GameObject player;
    public GameObject alert;
    public Transform groundCheck;
    public Transform attackHitBox;
    public GameObject[] itemDrops;
    
    [SerializeField] private UnityEngine.Vector3 offSet;

    void Awake() 
    {
        
        idleState = new PigIdleState();
        detectPlayerState = new PigDetectPlayerState();
        chargeState = new PigChargeState();
        attackState = new PigAttackState();
        deathState = new PigDeathState();
        
        Movement = GetComponent<PigMovement>();
        Combat = GetComponent<PigCombat>();
        Jump = GetComponent<PigJump>();

        StateMachine = new PigStateMachine();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        setupFinding = GetComponent<SetupFinding>();
        finder = GetComponent<Find>();
    }
    void Start()
    {
        StateMachine.Initialize(idleState, this);
        Movement.Initialize(setupFinding, player);
    }
    void Update()
    {
        StateMachine.Update(this);
    }

    void FixedUpdate() {
        StateMachine.FixedUpdate(this);
    }
    
    public void SwitchState(PigBaseState newState) {
        StateMachine.SwitchState(newState, this);
    }

    public void OnEnemyAttackFinished()
    {
        if (StateMachine.currentState == attackState)
            SwitchState(idleState);
    }

    public bool DetectPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= enemyType.detectRange;
    }

    public bool CheckForAttackRange()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= enemyType.attackRange;
    }

    public void Instantiate(GameObject prefab, float torque, float dropForce) {
        Rigidbody2D itemRb = Instantiate(prefab, transform.position, quaternion.identity).GetComponent<Rigidbody2D>();
        Vector2 dropVelocity = new Vector2(Random.Range(0.5f,-0.5f), 1) * dropForce;
        itemRb.AddForce(dropVelocity, ForceMode2D.Impulse);
        itemRb.AddTorque(torque, ForceMode2D.Impulse);
    }
}
