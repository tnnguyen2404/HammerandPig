using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoxProjectile : MonoBehaviour
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
    [SerializeField] public Vector3 explodeRadius;
    [SerializeField] public float dropForce;
    [SerializeField] public float torque;
    public GameObject[] itemsDrop;

    void Start() {
        rb = GetComponent<Rigidbody2D>();  
        anim = GetComponent<Animator>();
    }

    void Update() {
        Vector3 direction = (target.position - throwPos.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        if (isExploded) {
            anim.SetTrigger("Hit");
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Player")) {
            isExploded = true;
            DropItems();
            StartCoroutine(DestroyThisGameObjAfterHitAnim(0.15f));
        } 
    }

    void DropItems() {
        foreach (var item in itemsDrop) {
            Instantiate(item, torque, dropForce);
        }
    }

    void Instantiate(GameObject prefab, float torque, float dropForce) {
        Rigidbody2D itemRb = Instantiate(prefab, transform.position, quaternion.identity).GetComponent<Rigidbody2D>();
        Vector2 dropVelocity = new Vector2(Random.Range(0.5f,-0.5f), 1) * dropForce;
        itemRb.AddForce(dropVelocity, ForceMode2D.Impulse);
        itemRb.AddTorque(torque, ForceMode2D.Impulse);
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, explodeRadius);
    }


    IEnumerator DestroyThisGameObjAfterHitAnim(float delay) {
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
    }

    void AttackHitBox() {
        Collider2D[] detectedGameObj = Physics2D.OverlapBoxAll(transform.position, explodeRadius, whatIsPlayer);
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
