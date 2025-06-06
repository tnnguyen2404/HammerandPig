using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public int facingDirection = 1;
    private bool isFacingRight = true;
    [SerializeField] private float moveSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float horizontal)
    {
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        
        if (!isFacingRight && horizontal > 0)
            FlipSprite();
        else if (isFacingRight && horizontal < 0) 
            FlipSprite();
    }
    
    void FlipSprite() {
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180f, 0);
    }
    
}
