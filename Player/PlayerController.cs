using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerJump Jump { get; private set; }
    public PlayerCombat Combat { get; private set; }
    public PlayerAnimationController AnimationController { get; private set; }

    public PlayerState idleState;
    public PlayerState runningState;
    public PlayerState jumpState;
    public PlayerState attackingState;
    public PlayerState deathState;
    
    public PlayerStateMachine StateMachine { get; private set; }
    
    public Rigidbody2D rb;

    void Awake()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
        Movement = GetComponent<PlayerMovement>();
        Jump = GetComponent<PlayerJump>();
        Combat = GetComponent<PlayerCombat>();
        AnimationController = GetComponent<PlayerAnimationController>();
        
        StateMachine = new PlayerStateMachine();

        idleState = new IdleState();
        runningState = new RunningState();
        jumpState = new JumpState();
        attackingState = new AttackingState();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StateMachine.Initialize(idleState, this);
    }

    void Update()
    {
        InputHandler.ReadInput();

        if (InputHandler.isAttacking)
        {
            SwitchState(attackingState);
            return;
        }

        if (InputHandler.jumpInput)
        {
            SwitchState(jumpState);
            return;
        }
        
        StateMachine.Update(this);
        StateMachine.FixedUpdate(this);
    }
    
    public void SwitchState(PlayerState newState)
    {
        StateMachine.SwitchState(newState, this);
    }
    
}
