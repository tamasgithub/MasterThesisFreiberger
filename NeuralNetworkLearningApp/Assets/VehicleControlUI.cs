using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleControlUI : MonoBehaviour
{
    [SerializeField]
    private GameObject[] stationsAndVehicles;
    [SerializeField]
    private GameObject speedUp;
    [SerializeField]
    private GameObject startNow;
    private bool insideVehicle = false;
    private bool travellingInVehicle = false;

    [SerializeField]
    private float uiActivationDistance = 7;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }
    private void Update()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        foreach (GameObject obj in stationsAndVehicles)
        {
            if (Vector3.Distance(obj.transform.position, player.transform.position) < uiActivationDistance) {
                transform.GetChild(0).gameObject.SetActive(true);
                break;
            }
        }
    }
}
