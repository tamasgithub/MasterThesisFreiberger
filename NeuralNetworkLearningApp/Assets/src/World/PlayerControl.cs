using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    private Transform walkingArea;
    private TerrainData terrainData;

    private void Start()
    {
        terrainData = Terrain.activeTerrain.terrainData;
    }

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

        float scaleX;
        float scaleZ;
        Vector3 localPos = transform.localPosition;
        // keep same hight above terrain
        if (walkingArea == null)
        {
            localPos.y = Terrain.activeTerrain.SampleHeight(transform.position) + heightAboveGround;
            scaleX = terrainData.size.x;
            scaleZ = terrainData.size.z;
        } else
        {
            localPos.y = heightAboveGround;
            scaleX = walkingArea.GetChild(0).localScale.x * 10;
            scaleZ = walkingArea.GetChild(0).localScale.z * 10;
        }

        
        localPos.x = Mathf.Clamp(localPos.x, 0, scaleX);
        localPos.z = Mathf.Clamp(localPos.z, 0, scaleZ);
        transform.localPosition = localPos;



    }

    private void LookAtMouse()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        transform.eulerAngles = new Vector3(0, yaw, 0.0f);
        cam.transform.localEulerAngles = new Vector3(pitch, 0.0f, 0.0f);
        
    }

    public void EnterVehicle(Transform vehicle)
    {
        walkingArea = vehicle.GetChild(vehicle.childCount - 1);
        transform.parent = walkingArea;
        transform.localPosition = Vector3.zero + heightAboveGround * Vector3.up;
    }

    public void LeaveVehicle()
    {
        transform.parent = null;
        walkingArea = null;
        transform.Translate(6 * Vector3.forward);
        transform.Rotate(Vector3.up * 90);
    }
}
