using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Random = UnityEngine.Random;
using PathBerserker2d;

public class PigController : MonoBehaviour
{
    public PlayerController playerController;
    public PigBaseState currentState;
    public PigIdleState idleState;
    public PigChargeState chargeState;
    public PigPatrolState patrolState;
    public PigDetectPlayerState detectPlayerState;
    public PigAttackState attackState;
    public PigGetHitState getHitState;
    public PigDeathState deathState;
    public Rigidbody2D rb;
    public Animator anim;
    public LayerMask whatIsGround, whatIsPlayer, whatIsDamageable;
    public Transform groundCheck, wallCheck;
    public Transform attackHitBoxPos;
    public Transform player;
    public Transform target;
    public GameObject alert;
    public PigStatsSO stats;
    public NavAgent agent;
    public int facingDirection = 1;
    public Vector2 startPos;
    public Vector2 curPos;
    [SerializeField] private UnityEngine.Vector3 offSet;
    public float curHealth;
    public int playerFacingDirection;
    public int state = 0;
    public int numberOfBoxesLeft = 0;
    public float deltaDistance;
    public bool handleLinkMovement;
    public int minNumberOfLinkExecutions;
    public Vector2 storedLinkStart;
    public Vector2 direction;
    public Transform goal = null;
    public float timeOnLink;
    public float timeToCompleteLink;
    
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
    public bool applyKnockBack = true;
    
    [Header("State")]
    public float stateTime;

    void Awake() 
    {
        //patrolState = new PigPatrolState(this, "Patrol");
        idleState = new PigIdleState(this, "Idle");
        detectPlayerState = new PigDetectPlayerState(this, "Detection");
        chargeState = new PigChargeState(this, "Charge");
        attackState = new PigAttackState(this, "Attack");
        getHitState = new PigGetHitState(this, "GetHit");
        deathState = new PigDeathState(this, "Death");

        currentState = idleState;
        currentState.Enter();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavAgent>();
        startPos = transform.position;
        curHealth = stats.maxHealth;
    }
    void Update()
    {
        currentState.LogicUpdate();
    }

    void FixedUpdate() {
        currentState.PhysicsUpdate();
        agent.OnStartLinkTraversal += Agent_StartLinkTraversalEvent;
        agent.OnStartSegmentTraversal += Agent_OnStartSegmentTraversal;
        agent.OnLinkTraversal += Agent_OnLinkTraversal;
        agent.OnSegmentTraversal += Agent_OnSegmentTraversal;
    }
    public bool CheckForPlayer() {
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector2.Angle(isFacingRight ? transform.right : -transform.right, dirToPlayer);
        float disToPlayer = Vector2.Distance(player.position, transform.position);
        playerDetected = disToPlayer <= stats.playerDetectDistance && angleToPlayer <= stats.detectionAngle / 2;
        return playerDetected;
    }

    public bool CheckForWall() {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, isFacingRight ? Vector2.right : Vector2.left, stats.wallCheckDistance, whatIsGround);
        return isTouchingWall;
    }

    public bool CheckForGround() {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, stats.groundCheckDistance, whatIsGround);
        return isGrounded;
    }

    public bool CheckForAttackRange() {
        playerInAttackRange = Physics2D.Raycast(wallCheck.position, isFacingRight ? Vector2.right : Vector2.left, stats.attackRange, whatIsPlayer);
        return playerInAttackRange;
    }

    private void TakeDamage(float[] attackDetails) {
        curHealth -= attackDetails[0];
        playerFacingDirection = playerController.GetFacingDirection();
        if (curHealth > 0.1f && applyKnockBack) {
            SwitchState(getHitState);
        } else {
            SwitchState(deathState);
        }
    }

    #region Other Functions
    public void SwitchState(PigBaseState newState) {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        stateTime = Time.time;
    }

    public void AnimationAttackTrigger() {
        currentState.AnimationAttackTrigger();
    }

    public void AnimaitonFinishedTrigger() {
        currentState.AnimaitonFinishedTrigger();
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

    public void Agent_StartLinkTraversalEvent(NavAgent agent)
    {
        string linkType = agent.CurrentPathSegment.link.LinkTypeName;
        bool unknownLinkType = linkType != "jump" && linkType != "fall";

        handleLinkMovement = linkType == "jump" || linkType == "fall";

        if (!handleLinkMovement)
            return;

        timeOnLink = 0;
        Vector2 delta = agent.PathSubGoal - agent.CurrentPathSegment.LinkStart;
        deltaDistance = delta.magnitude;
        direction = delta / deltaDistance;
        minNumberOfLinkExecutions = 1;
        storedLinkStart = agent.CurrentPathSegment.LinkStart;

        if (direction.x > 0 && !isFacingRight || direction.x < 0 && isFacingRight) {
            Flip();
        }

        float speed = 1;

        switch (agent.CurrentPathSegment.link.LinkTypeName)
        {
            case "fall":
                speed = stats.fallSpeed;
                break;
            case "jump":
                speed = stats.jumpSpeed;
                break;
        }

        timeToCompleteLink = deltaDistance / speed;
    }

    public void Agent_OnLinkTraversal(NavAgent agent)
    {
        if (!handleLinkMovement)
        {
            return;
        }

        timeOnLink += Time.deltaTime;
        timeOnLink = Mathf.Min(timeToCompleteLink, timeOnLink);

        switch (agent.CurrentPathSegment.link.LinkTypeName)
        {
            case "jump":
                Jump(agent);
                break;
            case "fall":
                Fall(agent);
                break;
            default:
                Jump(agent);
                break;
        }
        minNumberOfLinkExecutions--;

        if (timeOnLink >= timeToCompleteLink && minNumberOfLinkExecutions <= 0)
        {
            agent.CompleteLinkTraversal();
            return;
        }
    }

    public void Agent_OnStartSegmentTraversal(NavAgent agent)
    {

    }

    public void Agent_OnSegmentTraversal(NavAgent agent)
    {
        Vector2 newPos;
        bool reachedGoal = MoveAlongSegment(agent.Position, agent.PathSubGoal, agent.CurrentPathSegment.Point, agent.CurrentPathSegment.Tangent, Time.deltaTime * stats.chargeSpeed, out newPos);
        agent.Position = newPos;

        if (reachedGoal)
        {
            agent.CompleteSegmentTraversal();
        }
    }

    private void Jump(NavAgent agent)
    {
        Vector2 newPos = storedLinkStart + direction * timeOnLink * stats.jumpSpeed;
        newPos.y += deltaDistance * 0.3f * Mathf.Sin(Mathf.PI * timeOnLink / timeToCompleteLink);
        agent.Position = newPos;
    }

    private void Fall(NavAgent agent)
    {
        Vector2 newPos = storedLinkStart + direction * timeOnLink * stats.fallSpeed;
        agent.Position = newPos;
    }

    private static bool MoveAlongSegment(Vector2 pos, Vector2 goal, Vector2 segPoint, Vector2 segTangent, float amount, out Vector2 newPos)
    {
        pos = Geometry.ProjectPointOnLine(pos, segPoint, segTangent);
        goal = Geometry.ProjectPointOnLine(goal, segPoint, segTangent);
        return MoveTo(pos, goal, amount, out newPos);
    }

    private static bool MoveTo(Vector2 pos, Vector2 goal, float amount, out Vector2 newPos)
    {
        Vector2 dir = goal - pos;
        float distance = dir.magnitude;
        if (distance <= amount)
        {
            newPos = goal;
            return true;
        }

        newPos = pos + dir * amount / distance;
        return false;
    }

    public Vector2 GetTargetPosition() {
        Vector2 tpos = target.position;

        if (stats.targetPredictionTime > 0)
        {
            IVelocityProvider velocityProvider = target.GetComponent<IVelocityProvider>();
            if (velocityProvider != null)
                return tpos + velocityProvider.WorldVelocity * stats.targetPredictionTime;

            Rigidbody2D rigidbody = target.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
                return tpos + rigidbody.velocity * stats.targetPredictionTime;
        }
            return tpos;
    }

    public void Flip()
    {
        UnityEngine.Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
    }

    void OnDrawGizmos() {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - 0.14f));
        Gizmos.DrawWireSphere(attackHitBoxPos.position, stats.attackRadius);
        UnityEngine.Vector3 forward = -transform.right;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offSet, stats.detectionRadius);
        Gizmos.color = Color.blue;
    
        int segments = 20; // Number of segments to draw the cone
        for (int i = 0; i <= segments; i++)
        {
            float angle = -stats.detectionAngle / 2 + stats.detectionAngle * (i / (float)segments);
            UnityEngine.Vector3 segment = UnityEngine.Quaternion.Euler(0, 0, angle) * forward * stats.detectionRadius;
            Gizmos.DrawLine(transform.position + offSet, transform.position + offSet + segment);
        }
    }
    #endregion
}
