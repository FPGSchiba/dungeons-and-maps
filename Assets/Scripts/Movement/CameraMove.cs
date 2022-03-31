using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;

    [Header("Settings")]
    public float speed = 10f;
    public float scrollSpeed = 20f;
    public float maxHeight = 30f;
    public float minHeight = 10f;

    [Header("Privates")]
    [GreyOut] public float currentHeight;

    void Start()
    {
        currentHeight = transform.position.y;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Mouse ScrollWheel");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        currentHeight += y * scrollSpeed * Time.deltaTime;
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
    }
}
