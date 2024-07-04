using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] public float explodeRadius;
    [SerializeField] public float xOffset, yOffset;
    void Start() {
        rb = GetComponent<Rigidbody2D>();  
        anim = GetComponent<Animator>();
        anim.SetBool("Bomb On", true);
    }

    void Update() {
        Vector3 direction = (target.position - throwPos.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
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
        this.gameObject.SetActive(false);
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
