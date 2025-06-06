using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour, IDamageable
{
    [SerializeField] private float knockBackDuration;
    [SerializeField] private float knockBackDeathSpeedX, knockBackDeathSpeedY, deathTorque;
    [SerializeField] private int maxHealth;
    [SerializeField] private GameObject hitParticle;
    private float knockBackStart;
    public int currentHealth;
    private bool knockBack;
    private GameObject brokenTopGO, brokenBottomGO;
    private Rigidbody2D rbBrokenTop, rbBrokenBottom;
    private Animator anim;
    private Rigidbody2D rb;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        brokenBottomGO = transform.Find("BrokenBottom").gameObject;
        brokenTopGO = transform.Find("BrokenTop").gameObject;
        
        rbBrokenTop = brokenTopGO.GetComponent<Rigidbody2D>();
        rbBrokenBottom = brokenBottomGO.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        
        gameObject.SetActive(true);
        brokenBottomGO.SetActive(false);
        brokenTopGO.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, true);
    }

    public void TakeDamage(int damage, bool playerOnLeft)
    {
        currentHealth -= damage;
        
        Instantiate(hitParticle, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        
        anim.SetBool("PlayerOnLeft", playerOnLeft);
        anim.SetTrigger("Damage");

        if (currentHealth <= 0) {
            Die();
        } 
    }
    
    private void ApplyKnockBack(Vector2 knockBackDirection)
    {
        rb.velocity = knockBackDirection;
    }

    private void Die() 
    {
        gameObject.SetActive(false);
        brokenBottomGO.SetActive(true);
        brokenTopGO.SetActive(true);

        brokenBottomGO.transform.position = transform.position;
        brokenTopGO.transform.position = transform.position;

        rbBrokenBottom.velocity = new Vector2(knockBackDeathSpeedX , knockBackDeathSpeedY);
        rbBrokenTop.velocity = new Vector2(knockBackDeathSpeedX , knockBackDeathSpeedY);
        rbBrokenTop.AddTorque(deathTorque, ForceMode2D.Impulse);
    }
}
