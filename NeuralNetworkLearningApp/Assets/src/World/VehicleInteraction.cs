using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VehicleInteraction : Interaction
{
    private bool insideVehicle;
    GameObject timerUI;
    [SerializeField]
    private VehicleController controller;
    

    protected override void Start()
    {
        base.Start();
        timerUI = interactUI.GetChild(1).gameObject;
    }

    public override void DisplayUIToInteract()
    {
        base.DisplayUIToInteract();
        interacting = true;
        timerUI.SetActive(true);
        timerUI.transform.GetChild(1).GetComponent<Text>().text = Mathf.CeilToInt(controller.GetRemainingHaltingTime()).ToString();
    }

    public override void RemoveUIToInteract()
    {
        base.RemoveUIToInteract();
        if (timerUI != null)
        {
            timerUI.SetActive(false);
        }
        
    }

    public override void StartInteraction()
    {
        interacting = true;
        PlayerControl control = player.GetComponent<PlayerControl>();
        if (insideVehicle)
        {
            print("leave vehicle");
            control.LeaveVehicle();
            transform.GetComponentInChildren<Collider>().enabled = true;
            insideVehicle = false;
        } else
        {
            print("enter vehicle");
            transform.GetComponentInChildren<Collider>().enabled = false;
            control.EnterVehicle(transform.GetChild(0));
            insideVehicle = true;
        }
        
    }

    public override void StopInteraction()
    {
        interacting = false;
        RemoveUIToInteract();
    }

    public VehicleController GetController()
    {
        return controller;
    }
}
