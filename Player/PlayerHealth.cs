using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject[] hearts;
    public Animator[] heartAnim;

    public bool isAttacked = false;
    
    private int maxHealth = 3;
    private int curHealth;

    void Start()
    {
        curHealth = maxHealth;
        InitializeHeartAnim();
    }

    void TakeDamage(int damage)
    {
        curHealth -= damage;
        isAttacked = true;
        UpdateHeartsUI();
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
    
    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Heart")) {
            GainSmallHeart();
            Destroy(other.gameObject);
        } else if (other.gameObject.CompareTag("BigHeart")) {
            GainBigHeart();
            Destroy(other.gameObject);
        }
    }
}
