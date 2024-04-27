using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoInteraction : Interaction
{
    [SerializeField]
    GameObject infoText;

    public override void StartInteraction()
    {
        interacting = true;
        infoText.SetActive(true);
    }

    public override void StopInteraction()
    {
        interacting = false;
        infoText.SetActive(false);
    }

}
