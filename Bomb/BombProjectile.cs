using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BombProjectile : MonoBehaviour
{
    private Transform throwPos;
    private Transform target;
    private float speed;
    private Rigidbody2D rb;
    private Animator anim;
    public LayerMask whatIsPlayer;
    private float damage = 2f;
    private float []attackDetails = new float [2];
    private bool isExploded = false;
    [SerializeField] private float explodeRadius;
    [SerializeField] private float xOffset, yOffset;
    [SerializeField] private float launchAngle;
    void Start() {
        rb = GetComponent<Rigidbody2D>();  
        anim = GetComponent<Animator>();
        anim.SetBool("Bomb On", true); 
        Vector3 targetPosition = target.position;
        Vector3 projectilePosition = transform.position;
        float gravity = Physics2D.gravity.y;
        float distance = Vector3.Distance(targetPosition, projectilePosition);
        float angle = launchAngle * Mathf.Deg2Rad;
        float velocity = Mathf.Sqrt(distance * Mathf.Abs(gravity) / Mathf.Sin(2 * angle));
        Vector2 direction = (targetPosition - projectilePosition).normalized;
        rb.velocity = new Vector2(direction.x * velocity * Mathf.Cos(angle), direction.y * velocity * speed * Mathf.Sin(angle));
    }
    void Update() {
        anim.SetBool("Hit", isExploded);
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Player")) {
            isExploded = true;
            StartCoroutine(DestroyThisGameObjAfterHitAnim(0.1f));
        } 
    }
    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(new Vector2(transform.position.x - xOffset, transform.position.y - yOffset), explodeRadius);
    }


    IEnumerator DestroyThisGameObjAfterHitAnim(float delay) {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    void AttackHitBox() {
        Collider2D[] detectedGameObj = Physics2D.OverlapCircleAll(transform.position, explodeRadius, whatIsPlayer);
        attackDetails[0] = damage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedGameObj) {
            collider.transform.SendMessage("TakeDamage", attackDetails);
        }
    }
    public void InitializeProjectile(Transform target, float speed, Transform holdSpot) {
        this.target = target;
        this.speed = speed;  
        throwPos = holdSpot;
    }
}
