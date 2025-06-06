using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class Bomb : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    public LayerMask whatIsPlayer;
    private PlayerController playerController;
    public PigThrowingBombController pigThrowingBomb;
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
            anim.SetTrigger("Hit");
            StartCoroutine(DestroyThisGameObjAfterHitAnim(0.15f));
        }
    }

    void KnockBack() {
        rb.velocity = new Vector2 (knockBackSpeedX * playerFacingDirection, knockBackSpeedY);
    }

    IEnumerator DestroyThisGameObjAfterHitAnim(float delay) {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
