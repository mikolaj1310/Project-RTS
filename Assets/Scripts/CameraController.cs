using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    float horizontal;
    float vertical;
    // Start is called before the first frame update

    private void Update()
    {
        MoveCameraPosition();
    }

    private void MoveCameraPosition()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical") * 2;

        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
        moveDirection.y = 0;
        moveDirection *= moveSpeed;
        moveDirection *= Time.deltaTime;
        transform.position += moveDirection;
    }
}
