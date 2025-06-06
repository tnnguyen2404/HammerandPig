using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator anim;
    private Rigidbody2D rb;
    private PlayerJump jump;
    
    private PlayerInputHandler inputHandler;
    private PlayerCombat combat;
    private PlayerHealth health;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<PlayerJump>();
        
        inputHandler = GetComponent<PlayerInputHandler>();
        combat = GetComponent<PlayerCombat>();
        health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", jump.IsGrounded());
        anim.SetBool("isMoving", inputHandler.isMoving);

        if (health.isAttacked)
        {
            anim.SetTrigger("GetHit");
            health.isAttacked = false;
        }
    }
}
