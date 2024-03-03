using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class Interaction : MonoBehaviour
{
    public Transform player;
    public KeyCode keyToInteract;
    private float triggerDistance = 3f;
    public Vector3 interactUIOffset;
    protected Camera cam;
    public Transform interactUI;

    // not interactable in general, but in the sense of "could interaction in this exact frame trigger
    // on this object because this is the closest Interaction holding gameobject in the scene
    // that is subscribed to InteractionObserver"
    private bool interactable = false;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        cam = player.GetComponentInChildren<Camera>();
        interactUI.GetComponent<InteractionObserver>().Subscribe(this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (interactable && Input.GetKeyDown(keyToInteract))
        {
            StartInteraction();
        }   
    }

    public float GetDistanceFromPlayer()
    {
        return Vector3.Distance(player.position, transform.position);
    }

    public void DisplayUIToInteract()
    {
        print(transform.name + " displays interaction ui");
        interactUI.position = cam.WorldToScreenPoint(transform.TransformPoint(interactUIOffset));
        interactUI.GetComponentInChildren<Text>().text = keyToInteract.ToString();
        interactable = true;
    }

    public void RemoveUIToInteract()
    {
        interactable = false;
    }

    public abstract void StartInteraction();

    public virtual void StopInteraction() { }
}
