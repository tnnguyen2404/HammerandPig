using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovement : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rb;
    private PigController controller;
    private PigJump jump;

    private Find pathFinder;
    private Queue<Action> currentPath = new Queue<Action>();
    private Coroutine pathCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PigController>();
        pathFinder = GetComponent<Find>();
        jump = GetComponent<PigJump>();
    }

    public Coroutine FollowPath(Vector2 target)
    {
        if (pathCoroutine != null)
            StopCoroutine(pathCoroutine);
        
        pathCoroutine = StartCoroutine(FollowPathCoroutine(target));
        return pathCoroutine;
    }

    IEnumerator FollowPathCoroutine(Vector2 target)
    {
        controller.enemyType.isProcessing = true;
        
        currentPath.Clear();
        var actionList = pathFinder.Findd(transform.position, target);
        if (actionList.Count == 0)
        {
            controller.enemyType.isProcessing = false;
            yield break;
        }

        foreach (var action in actionList)
        {
            currentPath.Enqueue(action);
        }

        while (currentPath.Count > 0)
        {
            Action current = currentPath.Dequeue();

            switch (current.SateAction)
            {
                case StateAction.move:
                    yield return MoveToX(current.Tagert.x);
                    break;
                case StateAction.jump:
                    jump.Jump();
                    yield return new WaitUntil(() => jump.isGrounded());
                    break;
                case StateAction.fall:
                    yield return new WaitUntil(() => Mathf.Abs(transform.position.y - target.y) < 0.1f);
                    break;
            }
        }
    }

    IEnumerator MoveToX(float targetX)
    {
        while (Mathf.Abs(transform.position.x - targetX) > 0.05f)
        {
            float direction = Mathf.Sign(targetX - transform.position.x);
            rb.velocity = new Vector2(direction * controller.enemyType.moveSpeed, rb.velocity.y);
            yield return null;
        }
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    bool WallCheck()
    {
        return Physics2D.Raycast(controller.enemyType.wallCheck.position,
            controller.enemyType.isFacingRight ? Vector2.right : Vector2.left,
            controller.enemyType.wallCheckDistance, controller.enemyType.wallLayer);
    }

    public void StopMoving()
    {
        if (pathCoroutine != null)
            StopCoroutine(pathCoroutine);
        
        rb.velocity = Vector2.zero;
    }

    public void FlipSprite()
    {
        controller.enemyType.isFacingRight = !controller.enemyType.isFacingRight;
        controller.enemyType.facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
}
