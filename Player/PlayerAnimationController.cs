using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerJump jump;
    
    private PlayerInputHandler inputHandler;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<PlayerJump>();
        
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", jump.IsGrounded());
        anim.SetBool("isMoving", inputHandler.isMoving);
    }
}
