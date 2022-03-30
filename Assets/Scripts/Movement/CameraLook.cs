using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 0.1f;

    [Header("Privates")]
    [GreyOut] public float xRotation = 0f;
    [GreyOut] public float yRotation = 0f;

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (Input.GetButton("Fire2"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yRotation += mouseY;
            xRotation -= mouseX;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
