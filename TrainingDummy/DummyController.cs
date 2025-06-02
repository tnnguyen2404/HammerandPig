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
    private int playerFacingDirection;
    private bool playerOnLeft, knockBack;
    private PlayerController pm;
    private GameObject aliveGO, brokenTopGO, brokenBottomGO;
    private Rigidbody2D rbAlive, rbBrokenTop, rbBrokenBottom;
    private Animator anim;

    private void Start()
    {
        currentHealth = maxHealth;
        pm = GetComponent<PlayerController>();

        aliveGO = transform.Find("Alive").gameObject;
        brokenBottomGO = transform.Find("BrokenBottom").gameObject;
        brokenTopGO = transform.Find("BrokenTop").gameObject;

        anim = aliveGO.GetComponent<Animator>();
        rbAlive = aliveGO.GetComponent<Rigidbody2D>();
        rbBrokenTop = brokenTopGO.GetComponent<Rigidbody2D>();
        rbBrokenBottom = brokenBottomGO.GetComponent<Rigidbody2D>();

        aliveGO.SetActive(true);
        brokenBottomGO.SetActive(false);
        brokenTopGO.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Dummy took damage: " + damage);
        currentHealth -= damage;
        playerFacingDirection = pm.GetFacingDirection();
        
        Instantiate(hitParticle, aliveGO.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

        if (playerFacingDirection == 1) {
            playerOnLeft = true;
        } else {
            playerOnLeft = false;
        }

        anim.SetBool("PlayerOnLeft",playerOnLeft);
        anim.SetTrigger("Damage");

        if (currentHealth < 0.0f) {
            Die();
        } 
    }
    
    private void ApplyKnockBack(Vector2 knockBackDirection)
    {
        rbAlive.velocity = knockBackDirection;
    }

    private void Die() 
    {
        aliveGO.SetActive(false);
        brokenBottomGO.SetActive(true);
        brokenTopGO.SetActive(true);

        brokenBottomGO.transform.position = aliveGO.transform.position;
        brokenTopGO.transform.position = aliveGO.transform.position;

        rbBrokenBottom.velocity = new Vector2(knockBackDeathSpeedX * playerFacingDirection, knockBackDeathSpeedY);
        rbBrokenTop.velocity = new Vector2(knockBackDeathSpeedX * playerFacingDirection, knockBackDeathSpeedY);
        rbBrokenTop.AddTorque(deathTorque * -playerFacingDirection, ForceMode2D.Impulse);
    }
}
