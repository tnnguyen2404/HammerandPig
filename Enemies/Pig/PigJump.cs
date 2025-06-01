using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigJump : MonoBehaviour
{
    private PigController controller;
    private Rigidbody2D rb;

    void Awake()
    {
        controller = GetComponent<PigController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public bool isGrounded()
    {
        return Physics.Raycast(controller.enemyType.groundCheck.position, Vector2.down,
            controller.enemyType.groundCheckDistance, controller.enemyType.groundLayer);
    }

    public void Jump()
    {
        if (isGrounded())
            rb.AddForce(new Vector2(rb.velocity.x, controller.enemyType.jumpForce), ForceMode2D.Impulse);
    }
}
