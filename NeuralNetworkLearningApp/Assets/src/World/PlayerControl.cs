using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float heightAboveGround = 2;
    public float walkSpeed = 3.0f;
    private Vector3 input;
    public Camera cam;
    public float speedH = 1.0f;
    public float speedV = 1.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    // Update is called once per frame
    void Update()
    {
        Move();
        LookAtMouse();
    }

    private void Move()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        input = input.normalized * Time.deltaTime * walkSpeed;
        transform.Translate(input);

        // keep same hight above terrain
        Vector3 pos = transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(transform.position) + heightAboveGround;
        transform.position = pos;
    }

    private void LookAtMouse()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(0, yaw, 0.0f);
        cam.transform.localEulerAngles = new Vector3(pitch, 0.0f, 0.0f);
        
    }
}
