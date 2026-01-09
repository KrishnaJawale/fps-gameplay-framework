using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Mouse Sensitivites")]
    [SerializeField]
    private float sensX;
    [SerializeField]
    private float sensY;

    [Header("Player Orientation and Direction")]
    [SerializeField]
    private Transform orientation;

    private float xRot;
    private float yRot;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRot += mouseX;
        xRot -= mouseY;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

        //rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRot, yRot, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRot, 0f);
    }
}
