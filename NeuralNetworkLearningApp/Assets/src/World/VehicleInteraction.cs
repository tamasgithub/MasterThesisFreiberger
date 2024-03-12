using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInteraction : Interaction
{
    private bool insideVehicle;
    protected override void Update()
    {
        base.Update();
        if (GetDistanceFromPlayer() > 3f)
        {
            StopInteraction();
        }
    }

    // Start is called before the first frame update
    // TODO: interactUI.GetComponent<InteractionObserver>().Unsubscribe(this);
    // while flying and subscribing again when landing
    public override void StartInteraction()
    {
        PlayerControl control = player.GetComponent<PlayerControl>();
        if (insideVehicle)
        {
            control.LeaveVehicle();
            insideVehicle = false;
        } else
        {
            control.EnterVehicle(transform.GetChild(0));
            insideVehicle = true;
        }
        
    }
}
