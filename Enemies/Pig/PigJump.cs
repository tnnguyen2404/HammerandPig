using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigJump : MonoBehaviour
{
    private PigController controller;
    private Rigidbody2D rb;
    
    public bool isGrounded;
    public bool isJumping;

    void Awake()
    {
        controller = GetComponent<PigController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckForGround();
    }
    
    public bool CheckForGround()
    {
        isGrounded = Physics2D.Raycast(controller.groundCheck.position, Vector2.down,
            controller.enemyType.groundCheckRadius, controller.enemyType.groundLayer);
        return isGrounded;
    }

    public void Jump(Vector2 force)
    {
        if (isGrounded)
            rb.AddForce(force);
    }
}
