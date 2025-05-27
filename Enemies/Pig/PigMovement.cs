using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PigController controller;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PigController>();
    }

    bool WallCheck()
    {
        return Physics2D.Raycast(controller.enemyType.wallCheck.position,
            controller.enemyType.isFacingRight ? Vector2.right : Vector2.left,
            controller.enemyType.wallCheckDistance, controller.enemyType.wallLayer);
    }

    public void Move()
    {
        rb.velocity = new Vector2(rb.velocity.x * controller.enemyType.moveSpeed, rb.velocity.y);
    }

    public void FlipSprite()
    {
        controller.enemyType.isFacingRight = !controller.enemyType.isFacingRight;
        controller.enemyType.facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
}
