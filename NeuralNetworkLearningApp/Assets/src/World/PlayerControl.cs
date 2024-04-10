using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerControl : MonoBehaviour
{
    public float heightAboveGround = 2;
    public float walkSpeed = 3.0f;
    private Vector3 input;
    public Camera cam;
    public float speedH = 1.0f;
    public float speedV = 1.0f;
    public GameObject menu;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    // mouse locked means camera moves with hidden mouse
    private bool mouseLocked = true;
    private Transform walkingArea;
    private TerrainData terrainData;
    private const string PLAYER_POS = "playerPos";
    private const string PLAYER_ROT = "playerRot";

    private void Start()
    {
        terrainData = Terrain.activeTerrain.terrainData;
        
        if (StaticData.Get<Vector3>(PLAYER_POS) != Vector3.zero)
        {
            transform.position = StaticData.Get<Vector3>(PLAYER_POS);
        }
        if (StaticData.Get<Vector3>(PLAYER_ROT) != Vector3.zero)
        {
            print("player rot " + StaticData.Get<Vector3>(PLAYER_ROT));
            transform.eulerAngles = StaticData.Get<Vector3>(PLAYER_ROT);
            yaw = transform.eulerAngles.y;
            pitch = transform.localEulerAngles.x;
        }
        Vector3 localPos = transform.localPosition;
        localPos.y = Terrain.activeTerrain.SampleHeight(transform.position) + heightAboveGround;
        transform.localPosition = localPos;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (mouseLocked)
        {
            LookAtMouse();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
        StaticData.Set(PLAYER_POS, transform.position);
        StaticData.Set(PLAYER_ROT, transform.eulerAngles);
        //print(StaticData.playerTransform.position);
    }

    private void Move()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (input == Vector3.zero)
        {
            return;
        }
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

    public void ToggleMenu()
    {
        bool menuActive = menu.activeSelf;
        menu.SetActive(!menuActive);
        Cursor.visible = !menuActive;
        this.mouseLocked = menuActive;
    }
}
