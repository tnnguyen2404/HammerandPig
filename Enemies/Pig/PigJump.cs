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
        RaycastHit2D hit = Physics2D.Raycast(controller.groundCheck.position, Vector2.down,
            controller.enemyType.groundCheckRadius, controller.enemyType.groundLayer);

        Debug.DrawLine(controller.groundCheck.position, controller.groundCheck.position + Vector3.down * controller.enemyType.groundCheckRadius, hit ? Color.green : Color.red);

        isGrounded = hit;
        return isGrounded;
    }

    public void Jump(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
