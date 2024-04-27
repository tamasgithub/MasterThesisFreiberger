using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class Interaction : MonoBehaviour
{
    protected Transform player;
    [SerializeField]
    private KeyCode keyToInteract;
    [SerializeField]
    private string interactionInfo;
    [SerializeField]
    private float interactionRange = 3f;
    [SerializeField]
    private Vector3 interactUIOffset;
    protected Camera cam;
    protected Transform interactUI;
    

    // not interactable in general, but in the sense of "could interaction in this exact frame trigger
    // on this object because this is the closest Interaction holding gameobject in the scene
    // that is subscribed to InteractionObserver"
    protected bool interactable = false;
    protected bool interacting = false;
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
        if (interactUI != null && interactUI.GetComponent<InteractionObserver>() != null)
        {
            interactUI.GetComponent<InteractionObserver>().Unsubscribe(this);
        }
        
        RemoveUIToInteract();
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
        if (interacting && Vector3.Distance(transform.position, player.position) > interactionRange)
        {
            StopInteraction();
        }
    }

    

    public float GetDistanceFromPlayer()
    {
        if (!this.enabled || player == null)
        {
            return float.PositiveInfinity;
        }
        return Vector3.Distance(player.position, transform.position);
    }

    public virtual void DisplayUIToInteract()
    {
        interactUI.position = cam.WorldToScreenPoint(transform.position + player.TransformDirection(interactUIOffset));
        interactUI.GetComponentsInChildren<Text>()[0].text = keyToInteract.ToString();
        interactUI.GetComponentsInChildren<Text>()[1].text = interactionInfo;
        interactable = true;
    }

    public virtual void RemoveUIToInteract()
    {
        interactable = false;
    }

    public abstract void StartInteraction();

    public virtual void StopInteraction() { }

    public float GetRange()
    {
        return interactionRange;
    }
}
