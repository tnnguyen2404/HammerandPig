using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    public PigController pigEnemy;
    private float inputRaw;
    private bool isAlive = true;
    private bool isMoving;
    private bool isJumping;
    private bool isAttacking;
    private bool isAttacked;
    private bool isFacingRight;
    public float maxHealth = 3;
    public float curHealth;
    public float numOfHeart;
    public GameObject[] hearts;
    public Animator[] heartAnim;
    private int attackDamage = 1;
    private float[] attackDetails = new float[2];
    private int enemyFacingDirection;
    public int facingDirection = 1;
    public float groundCheckRadius;
    public float attackCooldown;
    public float attackTimer = 1.1f;
    public Transform GroundCheck;
    public Transform AttackHitPosBox;
    public LayerMask whatIsGround, whatIsDamageable;
    [SerializeField] bool isGrounded;
    [SerializeField] bool applyKnockBack;
    [SerializeField] private float attackRadius;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float knockBackSpeedX, knockBackSpeedY;
    [SerializeField] Vector2 deathKick = new Vector2(15f,15f);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        curHealth = maxHealth;
        InitializeHeartAnim();
    }

    void Update()
    {
        Die();
        HandleAnimation();
        if (!isAlive) {return;}
        CheckInput();
        attackTimer += Time.deltaTime;
    }

    void FixedUpdate() {
        Move(inputRaw);
        CheckSurrounding();
    }

    void CheckInput() {
        inputRaw = Input.GetAxisRaw("Horizontal");
        
        if(Input.GetKey(KeyCode.Space) && isGrounded) {
            Jump();
        }

        if(Input.GetMouseButtonDown(0)) {
            isAttacking = true;
        } else {
            isAttacking = false;
        }
    }

    void HandleAnimation() {
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("yVelocity", rb.velocity.y);       
        anim.SetBool("isGrounded", isGrounded);
        if (isAttacking && attackTimer >= attackCooldown) {
            anim.SetTrigger("Attack");
            attackTimer = 0;
        }

        if (!isAlive) {
            anim.SetTrigger("Dead");
        }

        if (isAttacked) {
            anim.SetTrigger("GetHit");
            isAttacked = false;
        }
    }

    void CheckSurrounding() {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, whatIsGround);
        isJumping = !isGrounded;
    }

    void Move(float dir) {
        if (Mathf.Abs(dir) > 0) {
            isMoving = true;
        } else if (dir == 0) {
            isMoving = false;
        }

        if (isFacingRight && dir > 0) {
            FlipSprite();
        } else if (!isFacingRight && dir < 0) {
            FlipSprite();
        }

        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);
    }

    void Jump() {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void CheckAttackHitBox() {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(AttackHitPosBox.position, attackRadius, whatIsDamageable);
        attackDetails[0] = attackDamage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects) {
            collider.transform.SendMessage("TakeDamage", attackDetails);
        }
    }

    void TakeDamage(float[] attackDetails) {
        enemyFacingDirection = pigEnemy.GetFacingDirection();
        isAttacked = true;
        if (applyKnockBack && curHealth > 0.0f) {  
            curHealth -= attackDetails[0];
            KnockBack();
        } 
        UpdateHeartsUI();
    }

    void KnockBack() {
        rb.velocity = new Vector2(knockBackSpeedX * -enemyFacingDirection, knockBackSpeedY);
    }

    void Die() {
        if (curHealth <= 0) {
            isAlive = false;
            //rb.velocity = deathKick;
            StartCoroutine(DestroyPlayerGameObjAfterAnimation(1.0f));
        }
    }
        

    void FlipSprite() {
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180f, 0);
    }

    public int GetFacingDirection() {
        return facingDirection;
    }

    private void InitializeHeartAnim() {
        heartAnim = new Animator[hearts.Length];
        for (int i = 0; i < hearts.Length; i++) {
            heartAnim[i] = hearts[i].GetComponent<Animator>();
        }
    }

    private void UpdateHeartsUI() {
        for (int i = 0; i < hearts.Length; i++) {
            if (i < curHealth) {
                hearts[i].SetActive(true);
                if (!heartAnim[i].gameObject.activeSelf) {
                    heartAnim[i].gameObject.SetActive(true);
                    heartAnim[i].Play("Idle");
                }
            } else {
                heartAnim[i].SetTrigger("Disappear");
                StartCoroutine(DeactivateHeartAfterAnimation(0.15f, i));
            }
        }
    }

    private void GainSmallHeart() {
        if (curHealth < maxHealth) {
            curHealth++;
            UpdateHeartsUI();
        }
    }

    private void GainBigHeart() {
        if (curHealth < maxHealth) {
            curHealth += 2;
            UpdateHeartsUI();
        }
    }

    private IEnumerator DeactivateHeartAfterAnimation(float delay, int index) {
        yield return new WaitForSeconds(delay);
        hearts[index].SetActive(false);
    }

    private IEnumerator DestroyPlayerGameObjAfterAnimation(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Heart")) {
            GainSmallHeart();
            Destroy(other.gameObject);
        } else if (other.gameObject.CompareTag("BigHeart")) {
            GainBigHeart();
            Destroy(other.gameObject);
        }
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(AttackHitPosBox.position, attackRadius);
    }
}
