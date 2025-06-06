using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class Box : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    public LayerMask whatIsPlayer;
    private PlayerController playerController;
    public GameObject[] itemsDrop;
    public PigThrowingBoxController pigThrowing;
    [SerializeField] public float dropForce;
    [SerializeField] public float torque;
    [SerializeField] public float knockBackSpeedX, knockBackSpeedY;
    public bool applyKnockBack = true;
    private float maxHealth = 3;
    private float curHealth;
    private int playerFacingDirection;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        curHealth = maxHealth;
    }

    void  Update() {

    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PickUpHitBox")) {
            Debug.Log("BoxPickedUp");
            gameObject.SetActive(false);
        }
    }

    private void TakeDamage(float[] attackDetails) {
        curHealth -= attackDetails[0];
        //playerFacingDirection = playerController.GetFacingDirection();
        if (curHealth > 0.1f && applyKnockBack) {
            KnockBack();
        } else {
            DropItems();
            gameObject.SetActive(false);   
        }
    }

    void KnockBack() {
        rb.velocity = new Vector2 (knockBackSpeedX * playerFacingDirection, knockBackSpeedY);
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

    IEnumerator DestroyThisGameObjAfterHitAnim(float delay) {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
