using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public float horizontalInput { get; private set; }
    public bool isMoving { get; private set; }
    public bool jumpInput { get; private set; }
    public bool isAttacking { get; private set; }

    public void ReadInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        isMoving = Input.GetAxis("Horizontal") != 0;
        jumpInput = Input.GetKey(KeyCode.Space);
        isAttacking = Input.GetMouseButton(0);
    }
}
