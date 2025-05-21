using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerJump jump;
    
    private PlayerInputHandler inputHandler;
    private PlayerCombat combat;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<PlayerJump>();
        
        inputHandler = GetComponent<PlayerInputHandler>();
        combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", jump.IsGrounded());
        anim.SetBool("isMoving", inputHandler.isMoving);

        if (inputHandler.isAttacking && combat.attackTimer >= combat.attackCd)
        {
            anim.SetTrigger("Attack");
            combat.attackTimer = 0;
        }
    }
}
