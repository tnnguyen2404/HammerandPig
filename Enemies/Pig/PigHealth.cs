using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigHealth : MonoBehaviour
{
    private int curHealth;
    
    private PigController controller;

    void Awake()
    {
        controller = GetComponent<PigController>();
    }

    void Start()
    {
        curHealth = controller.enemyType.maxHealth;
    }

    void TakeDamage(int damage)
    {
        curHealth -= damage;
        controller.enemyType.isAttacked = true;
    }
}
