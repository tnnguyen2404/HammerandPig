using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PigController controller;

    private SetupFinding setupFinding;
    private Find finder;
    private List<Action> actions = new List<Action>();
    private int curIndex = 0;

    private Vector2 jumpTarget;
    private float jumpTolerance = 0.1f;
    private bool isFollowingPath = false;

    // Jump alignment/delay state
    private bool waitingForJump = false;
    private Coroutine jumpCoroutine;

    public void Initialize(SetupFinding setupFindingRef)
    {
        setupFinding = setupFindingRef != null ? setupFindingRef : SetupFinding.Instance;
        finder = setupFinding == null ? GetComponent<Find>() : setupFinding.GetComponent<Find>();
        if (finder == null)
            finder = FindObjectOfType<Find>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PigController>();
    }

    void Update()
    {
        if (!isFollowingPath) return;

        // If currently waiting to land after a jump, don't process next actions
        if (controller.Jump.isJumping)
        {
            // Only finish jump if both grounded and near target
            if (controller.Jump.isGrounded &&
                Mathf.Abs(transform.position.x - jumpTarget.x) < jumpTolerance &&
                Mathf.Abs(transform.position.y - jumpTarget.y) < 0.12f) // tighten for platformers
            {
                controller.Jump.isJumping = false;
                curIndex++;
            }
            return;
        }

        // Don't process next actions if currently in the jump alignment delay
        if (waitingForJump)
            return;

        if (curIndex >= actions.Count)
        {
            StopPath();
            return;
        }

        ExecuteCurrentAction();
    }

    public void MoveTo(Vector2 destination)
    {
        Vector2 start = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        actions = finder.Findd(start, destination);
        curIndex = 0;
        isFollowingPath = true;
        controller.Jump.isJumping = false;
        waitingForJump = false;
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
        }
    }

    public void StopPath()
    {
        isFollowingPath = false;
        actions.Clear();
        curIndex = 0;
        controller.Jump.isJumping = false;
        waitingForJump = false;
        if (jumpCoroutine != null)
        {
            StopCoroutine(jumpCoroutine);
            jumpCoroutine = null;
        }
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public bool isMoving()
    {
        return isFollowingPath;
    }

    void ExecuteCurrentAction()
    {
        if (curIndex >= actions.Count)
        {
            StopPath();
            return;
        }

        var action = actions[curIndex];

        switch (action.StateAction)
        {
            case StateAction.move:
                // Move horizontally towards target.x
                float dir = Mathf.Sign(action.Target.x - transform.position.x);
                rb.velocity = new Vector2(dir * controller.enemyType.moveSpeed, rb.velocity.y);
                FlipSprite(dir);

                // Arrived at target X?
                if (Mathf.Abs(transform.position.x - action.Target.x) < 0.05f)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    transform.position = new Vector3(action.Target.x, transform.position.y, transform.position.z);
                    curIndex++;
                }
                break;

            case StateAction.jump:
                // 1. Move to precise jump start X (action.Position.x)
                if (!controller.Jump.isJumping && controller.Jump.isGrounded)
                {
                    if (Mathf.Abs(transform.position.x - action.Position.x) > 0.05f)
                    {
                        // Move to align with jump start X
                        float moveDir = Mathf.Sign(action.Position.x - transform.position.x);
                        rb.velocity = new Vector2(moveDir * controller.enemyType.moveSpeed, rb.velocity.y);
                        FlipSprite(moveDir);
                    }
                    else
                    {
                        // Snap X exactly to jump start
                        transform.position = new Vector3(action.Position.x, transform.position.y, transform.position.z);
                        rb.velocity = Vector2.zero;

                        // Wait a moment for stability, then jump
                        if (!waitingForJump)
                        {
                            jumpCoroutine = StartCoroutine(DelayedJump(action.ForceJump, action.Target, 0.1f)); // 0.1–0.2s for best results
                        }
                    }
                }
                break;

            case StateAction.fall:
                // Wait until landed at target position
                if (Mathf.Abs(transform.position.y - action.Target.y) < 0.1f && controller.Jump.isGrounded)
                {
                    curIndex++;
                }
                else
                {
                    // You can move horizontally if needed to reach the fall target X
                    float fallDir = Mathf.Sign(action.Target.x - transform.position.x);
                    rb.velocity = new Vector2(fallDir * controller.enemyType.moveSpeed, rb.velocity.y);
                    FlipSprite(fallDir);
                }
                break;

            default:
                curIndex++;
                break;
        }
    }

    private IEnumerator DelayedJump(Vector2 force, Vector2 target, float delay)
    {
        waitingForJump = true;
        yield return new WaitForSeconds(delay);
        controller.Jump.Jump(force);
        controller.Jump.isJumping = true;
        jumpTarget = target;
        waitingForJump = false;
        jumpCoroutine = null;
    }

    private void FlipSprite(float directionX)
    {
        if (Mathf.Abs(directionX) < 0.01f) return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX < 0 ? 1 : -1);
        transform.localScale = scale;
    }
}
