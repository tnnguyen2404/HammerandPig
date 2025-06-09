using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovement : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private PigController controller;

    private SetupFinding pathFinder;
    private List<Action> currentPath;
    
    public bool isCharging;
    public bool isFacingRight = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PigController>();
    }

    public void Initialize(SetupFinding finder, GameObject target)
    {
        pathFinder = finder;
        player = target;
    }

    public void StartPath()
    {
        controller.Jump.isJumping = false;
    }

    public void HandlePath()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            UpdatePath();
            return;
        }

        MoveAlongPath();
    }

    private void UpdatePath()
    {
        Vector2 start = RoundVec(transform.position);
        Vector2 target = RoundVec(player.transform.position);
        
        if (pathFinder == null)
        {
            Debug.LogError("Pathfinder is NULL — did you assign SetupFinding?");
            return;
        }

        if (pathFinder.FindPlatForm(start) == 1) return;
        
        currentPath = pathFinder.Findd(start, target);
    }

    private void MoveAlongPath()
    {
        if (currentPath.Count == 0) return;
        
        Vector2 curPos = RoundVec(transform.position);

        if (currentPath[0].Target == curPos)
        {
            currentPath.RemoveAt(0);
            return;
        }
        
        Action currentAction = currentPath[0];

        switch (currentAction.SateAction)
        {
            case StateAction.move:
                float dir = currentAction.Target.x < transform.position.x ? -1 : 1;
                rb.velocity = new Vector2(dir * controller.enemyType.moveSpeed, rb.velocity.y);
                
                if (dir != 1 && isFacingRight)
                    FlipSprite();
                else if (dir == 1 && !isFacingRight)
                    FlipSprite();
                
                break;
            case StateAction.fall:
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;
            case StateAction.jump:
                StartCoroutine(DoJump());
                break;
        }
    }

    IEnumerator DoJump()
    {
        yield return new WaitForSeconds(0.1f);
        controller.Jump.Jump();
        yield return new WaitForSeconds(1f);
    }

    private Vector2 RoundVec(Vector2 v)
    {
        return new Vector2(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    }

    public void FlipSprite()
    {
        controller.enemyType.isFacingRight = !controller.enemyType.isFacingRight;
        controller.enemyType.facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
}
