using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    float horizontal;
    float vertical;
    private GameObject cameraObject;
    private Vector3 lastMousePos;
    [SerializeField] float mouseSensitivity = 0.03f;
    // Start is called before the first frame update

    private void Update()
    {
        cameraObject = transform.Find("Main Camera").gameObject;
        MoveCameraPosition();
    }

    private void MoveCameraPosition()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);//transform.forward * vertical + transform.right * horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection *= moveSpeed;
        moveDirection *= Time.deltaTime;
        //transform.position += moveDirection;
        transform.Translate(moveDirection, transform);
        
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            cameraObject.transform.position += cameraObject.transform.TransformDirection(Vector3.forward);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            cameraObject.transform.position -= cameraObject.transform.TransformDirection(Vector3.forward);
        }

        if (Input.GetMouseButtonDown(2))
        {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            Vector3 newVec = new Vector3(-delta.x * mouseSensitivity, 0f, -delta.y * mouseSensitivity);
            
            transform.Translate(newVec, transform);
            lastMousePos = Input.mousePosition;
        }
    }
}
