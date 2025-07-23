using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PigController controller;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PigController>();
    }
}
