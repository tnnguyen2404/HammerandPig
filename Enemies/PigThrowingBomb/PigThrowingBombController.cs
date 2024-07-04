using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PathBerserker2d;
using Unity.Mathematics;
using Unity.VisualScripting;

public class PigThrowingBombController : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb;
    public LayerMask whatIsGround, whatIsPlayer, whatIsDamageable, whatIsPickUp;
    public Transform groundCheck, wallCheck;
    public PigThrowingBombStatsSO stats;
    public PigThrowingBombBaseState currentState;
    public PigThrowingBombIdleState idleState;
    public PigThrowingBombPlayerDetectedState playerDetectedState;
    public PigThrowingBombMeleeAttackState meleeAttackState;
    public PigThrowingBombPickingUpBombState pickingUpBombState;
    public PigThrowingBombChargeState chargeState;
    public PigThrowingBombHoldingBombIdleState holdingBombIdleState;
    public PigThrowingBombFindingBombState findingBombState;
    public PigThrowingBombDeathState deathState;
    public PigThrowingBoxBackUpState backUpState;
    public PigThrowingBombGetHitState getHitState;
    public PigThrowingBombHoldingBombChargeState holdingBombChargeState;
    public PigThrowingBombRangeAttackState rangeAttackState;
    public GameObject alert;
    public Transform player;
    public Transform target;
    public NavAgent agent;
    public Transform holdSpot;
    public Transform attackHitBoxPos;
    public PlayerController playerController;
    public int facingDirection = -1;
    public float stateTime;
    public float timeOnLink;
    public float timeToCompleteLink;
    public Vector2 direction;
    public Vector3 pickUpDirection { get; set; }
    public int state = 0;
    public int numberOfBoxesLeft = 0;
    public float deltaDistance;
    public bool handleLinkMovement;
    public int minNumberOfLinkExecutions;
    public Vector2 storedLinkStart;
    public Vector2 startPos;
    public Transform goal = null;
    private int playerFacingDirection;

    [Header("Boolean")]
    public bool playerDetected;
    public bool boxHasBeenPickedUp = false;
    public bool isGrounded;
    public bool isTouchingWall;
    public bool isMoving;
    public bool isAlive;
    public bool isFacingRight = false;
    public bool playerInAttackRange;
    public bool isAttacking;
    public bool boxIsNearBy;
    public bool isInPickUpRange;
    public bool reachedEndofPath = false;
    public bool isJumping;
    public bool applyKnockBack = true;
    void Awake() {
        idleState = new PigThrowingBombIdleState(this, "Idle");
        playerDetectedState = new PigThrowingBombPlayerDetectedState(this, "PlayerDetected");
        pickingUpBombState = new PigThrowingBombPickingUpBombState(this, "PickingUpBomb");
        chargeState = new PigThrowingBombChargeState(this, "Charge");
        rangeAttackState = new PigThrowingBombRangeAttackState(this, "RangeAttack");
        holdingBombIdleState = new PigThrowingBombHoldingBombIdleState(this, "HoldingBombIdle");
        findingBombState = new PigThrowingBombFindingBombState(this, "FindingBomb");
        deathState = new PigThrowingBombDeathState(this, "Death");
        getHitState = new PigThrowingBombGetHitState(this, "GetHit");
        holdingBombChargeState = new PigThrowingBombHoldingBombChargeState(this, "HoldingBombCharge");
        meleeAttackState = new PigThrowingBombMeleeAttackState(this, "MeleeAttack");
        currentState = idleState;
        currentState.Enter();
    }
    void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavAgent>();
        startPos = transform.position;
        stats.curHealth = stats.maxHealth;
    }

    void Update() {
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
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < stats.playerDetectDistance) {
            playerDetected = true;
        } else {
            playerDetected = false;
        }
        return playerDetected;
    }

    public bool CheckForGround() {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, stats.groundCheckDistance, whatIsGround);
        return isGrounded;
    }

    public bool CheckForAttackRange() {  
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < stats.attackRange) {
            playerInAttackRange = true;
        } else {
            playerInAttackRange = false;
        }
        return playerInAttackRange;
    }
    public bool CheckForPickUpRange() {
        isInPickUpRange = Physics2D.Raycast(groundCheck.position, isFacingRight ? Vector2.right : Vector2.left, stats.pickingUpBoxRange, whatIsPickUp);
        return isInPickUpRange;
    }

    public bool CheckIfShouldDodge() {
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, facingDirection == 1 ? Vector2.right : Vector2.left, stats.dodgeDetectDistance , whatIsPlayer);
        bool aggressivePlayer = facingDirection > 0 && Input.GetAxis("Horizontal") < 0 || facingDirection < 0 && Input.GetAxis("Horizontal") > 0;
        if (hit && aggressivePlayer) {
            return true;
        } else {
            return false;
        }
    }

    private void TakeDamage(float[] attackDetails) {
        stats.curHealth -= attackDetails[0];
        playerFacingDirection = playerController.GetFacingDirection();
        if (stats.curHealth > 0.1f && applyKnockBack) {
            SwitchState(getHitState);
        } else {
            SwitchState(deathState);
        }
    }

    public void SwitchState(PigThrowingBombBaseState newState) {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        stateTime = Time.time;
    }

    public int GetFacingDirection() {
        return facingDirection;
    }

    public void InstantiateBomb() {
        GameObject pooledBomb = BombObjectPooling.SharedInstance.GetPooledObject();
        if (pooledBomb != null) {
            pooledBomb.SetActive(true);
            BombProjectile bomb = pooledBomb.GetComponent<BombProjectile>();
            bomb.transform.position = holdSpot.position;
            bomb.transform.rotation = holdSpot.rotation;
            bomb.InitializeProjectile(target, stats.boxSpeed, holdSpot);
        } else {
            Debug.LogWarning("No boxes available in the pool.");
        }
    }

    public void InstantiateItemDrop(GameObject prefab, float torque, float dropForce) {
        Rigidbody2D itemRb = Instantiate(prefab, transform.position, quaternion.identity).GetComponent<Rigidbody2D>();
        Vector2 dropVelocity = new Vector2(UnityEngine.Random.Range(0.5f,-0.5f), 1) * dropForce;
        itemRb.AddForce(dropVelocity, ForceMode2D.Impulse);
        itemRb.AddTorque(torque, ForceMode2D.Impulse);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x - 0.3f, wallCheck.position.y));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - 0.14f));
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(attackHitBoxPos.position, stats.meleeAttackRadius);
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
        bool reachedGoal = MoveAlongSegment(agent.Position, agent.PathSubGoal, agent.CurrentPathSegment.Point, agent.CurrentPathSegment.Tangent, Time.deltaTime * stats.runSpeed, out newPos);
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
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
    }
    public void AnimationAttackTrigger() {
        currentState.AnimationAttackTrigger();
    }

    public void AnimaitonFinishedTrigger() {
        currentState.AnimaitonFinishedTrigger();
    }
}
