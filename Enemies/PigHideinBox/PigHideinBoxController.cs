using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PigHideinBoxController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerController playerController;
    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject[] itemDrop;
    [SerializeField] private float torque;
    [SerializeField] private float dropForce;
    private float curHealth;
    private int playerFacingDirection;
    public bool isDead;
    

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        curHealth = maxHealth;
    }

    void Update() {
        
    }

    void FixedUpdate() {
    
    }

    private void TakeDamage(float[] attackDetails) {
        curHealth -= attackDetails[0];
        if (curHealth < 0.1f) {
            isDead = true;
            InstantiatePig();
            DropItems();
            gameObject.SetActive(false);
        }  
    }

    void InstantiatePig() {
        GameObject poolPig = PigObjectPooling.SharedInstance.GetPooledObject();
        poolPig.SetActive(true);
        poolPig.transform.position = transform.position;
        poolPig.transform.rotation = transform.rotation;
    }

    void DropItems() {
        foreach (var item in itemDrop) {
            InstantiateItemDrop(item, torque, dropForce);
        }
    }

     void InstantiateItemDrop(GameObject prefab, float torque, float dropForce) {
        Rigidbody2D itemRb = Instantiate(prefab, transform.position, quaternion.identity).GetComponent<Rigidbody2D>();
        Vector2 dropVelocity = new Vector2(UnityEngine.Random.Range(0.5f,-0.5f), 1) * dropForce;
        itemRb.AddForce(dropVelocity, ForceMode2D.Impulse);
        itemRb.AddTorque(torque, ForceMode2D.Impulse);
    }
}
