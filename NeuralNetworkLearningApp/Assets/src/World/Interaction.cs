using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class Interaction : MonoBehaviour
{
    protected Transform player;
    public KeyCode keyToInteract;
    public string interactionInfo;
    public float interanctionRange = 3f;
    public Vector3 interactUIOffset;
    protected Camera cam;
    protected Transform interactUI;

    // not interactable in general, but in the sense of "could interaction in this exact frame trigger
    // on this object because this is the closest Interaction holding gameobject in the scene
    // that is subscribed to InteractionObserver"
    private bool interactable = false;
    // Start is called before the first frame update
    private void Awake()
    {
        interactUI = GameObject.Find("InteractUI").transform;
    }
    private void OnEnable()
    {
        interactUI.GetComponent<InteractionObserver>().Subscribe(this);
    }

    private void OnDisable()
    {
        interactUI.GetComponent<InteractionObserver>().Unsubscribe(this);
    }

    protected virtual void Start()
    {
        player = GameObject.Find("Player").transform;
        cam = player.GetComponentInChildren<Camera>();
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
        //Vector3 localPos = player.TransformPoint(transform.position - player.position + interactUIOffset);
        interactUI.position = cam.WorldToScreenPoint(transform.position + player.TransformDirection(interactUIOffset));
        interactUI.GetComponentInChildren<Text>().text = keyToInteract.ToString() + interactionInfo;
        interactable = true;
    }

    public void RemoveUIToInteract()
    {
        interactable = false;
    }

    public abstract void StartInteraction();

    public virtual void StopInteraction() { }

    public float GetRange()
    {
        return interanctionRange;
    }
}
