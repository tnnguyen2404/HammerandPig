using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigAnimationController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PigController controller;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PigController>();
    }

    void Update()
    {
        anim.SetBool("Charge", controller.Movement.isMoving());
        anim.SetBool("Attack", controller.Combat.isAttacking);
        anim.SetBool("Death", controller.enemyType.isAlive);

        if (controller.enemyType.isAttacked)
            anim.SetTrigger("GetHit");
    }
}
