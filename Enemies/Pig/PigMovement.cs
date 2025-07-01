using System.Collections.Generic;
using UnityEngine;

public class PigMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public bool smartJump = false; // Only jump if needed

    [Header("Jump Settings")]
    public float minJumpTime = 0.3f;
    public float maxJumpTime = 0.7f;
    public float jumpExtraHeight = 0.5f;
    public float jumpForce = 12f; // Fallback force if needed

    private Rigidbody2D rb;
    private List<Node> curPath = null;
    private int curNodeIndex = 0;
    private Transform player;
    private PigController controller;
    private bool isJumping = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PigController>();
    }

    public void SetPath(List<Node> path)
    {
        curPath = path;
        curNodeIndex = (path != null && path.Count > 1) ? 1 : 0;
        isJumping = false;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void FollowPath()
    {
        if (curPath == null || curNodeIndex >= curPath.Count) return;

        Node targetNode = curPath[curNodeIndex];
        if (targetNode == null) return;

        // Use explicit Vector2 construction to avoid ambiguity
        Vector2 pigPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 nodePos = new Vector2(targetNode.transform.position.x, targetNode.transform.position.y);
        Vector2 toNode = nodePos - pigPos;

        // If this is not the last node, and we're close, immediately advance
        if (curNodeIndex < curPath.Count - 1 && toNode.magnitude < 0.2f)
        {
            curNodeIndex++;
            isJumping = false; // Ready for next jump if needed
            targetNode = curPath[curNodeIndex];
            nodePos = new Vector2(targetNode.transform.position.x, targetNode.transform.position.y);
            toNode = nodePos - pigPos;
        }

        // SMART JUMP: Only jump if needed
        if (smartJump && IsPlayerOnSamePlatform())
        {
            Walk(toNode.x);
            return;
        }

        // Jump if needed
        if (curNodeIndex > 0 && curPath[curNodeIndex - 1].isJumpPoint)
        {
            if (IsGrounded() && !isJumping)
            {
                RealisticJumpTo(targetNode.transform.position);
                isJumping = true;
            }
        }
        else
        {
            Walk(toNode.x);
        }

        FlipSprite(player.transform.position.x - transform.position.x);
    }

    private void Walk(float xDir)
    {
        rb.velocity = new Vector2(Mathf.Sign(xDir) * moveSpeed, rb.velocity.y);
    }

    /// <summary>
    /// Launch the pig with a velocity that will land on the target node, with robust zero-checking.
    /// </summary>
    public void RealisticJumpTo(Vector3 targetPos)
    {
        Vector2 startPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 endPos = new Vector2(targetPos.x, targetPos.y);
        float gravity = Mathf.Abs(Physics2D.gravity.y);

        float dx = endPos.x - startPos.x;
        float dy = (endPos.y + jumpExtraHeight) - startPos.y;

        float time = Mathf.Clamp(Mathf.Abs(dx) / 5f, minJumpTime, maxJumpTime);

        // Avoid division by zero or near-zero
        if (Mathf.Approximately(time, 0f) || Mathf.Abs(dx) < 0.01f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // fallback simple jump
            return;
        }

        float vx = dx / time;
        float vy = (dy + 0.5f * gravity * time * time) / time;

        // Avoid NaN/Infinity velocities
        if (float.IsNaN(vx) || float.IsInfinity(vx) || float.IsNaN(vy) || float.IsInfinity(vy))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            return;
        }

        rb.velocity = new Vector2(vx, vy);
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    /// <summary>
    /// Checks if player is on the same platform (same Y, and no gap between pig and player)
    /// </summary>
    private bool IsPlayerOnSamePlatform()
    {
        if (player == null) return false;
        float yDiff = Mathf.Abs(transform.position.y - player.position.y);
        if (yDiff > 0.15f) return false;

        // Check for ground under every step between pig and player
        Vector2 dir = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;
        Vector2 checkPos = new Vector2(transform.position.x, transform.position.y);
        float distance = Mathf.Abs(transform.position.x - player.position.x);
        int steps = Mathf.CeilToInt(distance / 0.2f);

        for (int i = 0; i < steps; i++)
        {
            checkPos += dir * 0.2f;
            Vector2 below = new Vector2(checkPos.x, checkPos.y - 0.1f);
            if (!Physics2D.OverlapCircle(below, 0.15f, groundLayer))
                return false; // found a gap!
        }
        return true; // all ground between pig and player
    }

    private void FlipSprite(float directionX)
    {
        if (Mathf.Abs(directionX) < 0.01f) return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX < 0 ? 1 : -1);
        transform.localScale = scale;
    }
}
