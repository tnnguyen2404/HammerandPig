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
    
    public bool isCharging;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PigController>();
        pathFinder = GetComponent<Find>();
        jump = GetComponent<PigJump>();
    }

    public void FollowPath(Vector2 target)
    {
        StartCoroutine(FollowPathCoroutine(target));
    }

    IEnumerator FollowPathCoroutine(Vector2 target)
    {
        currentPath.Clear();
        var actionList = pathFinder.Findd(transform.position, target);
        if (actionList.Count == 0)
        {
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
                    yield return MoveToX(current.Tagert);
                    break;
                case StateAction.jump:
                    jump.Jump();
                    yield return new WaitForSeconds(1f);
                    break;
                case StateAction.fall:
                    yield return new WaitUntil(() => jump.CheckForGround());
                    break;
            }
        }
    }

    IEnumerator MoveToX(Vector2 target)
    {
        while (Vector2.Distance(target, transform.position) > 0.05f)
        {
            float direction = Mathf.Sign(target.x - transform.position.x);
            rb.velocity = new Vector2(direction * controller.enemyType.moveSpeed, rb.velocity.y);
            yield return null;
        }
    }
    

    public void StopMoving(Vector2 target)
    {
        StopCoroutine(FollowPathCoroutine(target));
        rb.velocity = Vector2.zero;
    }

    public void FlipSprite()
    {
        controller.enemyType.isFacingRight = !controller.enemyType.isFacingRight;
        controller.enemyType.facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
}
