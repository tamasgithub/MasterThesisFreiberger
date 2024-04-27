using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public abstract class VehicleController : MonoBehaviour
{
    public event Action stopEvent;
    public event Action startEvent;

    protected int direction = -1;
    [SerializeField]
    private float haltingTime = 5f;
    private float remainingHaltingTime;
    [SerializeField]
    private float speed = 1;
    private bool halting = true;
    private float progress = 0;
    [SerializeField]
    private KeyCode speedUpKey = KeyCode.X;
    [SerializeField]
    private KeyCode startTravelKey = KeyCode.Y;
    [SerializeField]
    private int speedUpMulti = 5;

    // Start is called before the first frame update

    protected virtual void Start()
    {
        remainingHaltingTime = haltingTime;
        StartCoroutine(WaitToLeaveStation());
        VehicleInteraction interaction = transform.GetComponent<VehicleInteraction>();
        if (interaction != null )
        {
            startEvent += () => interaction.enabled = false;
            stopEvent += () => interaction.enabled = true;
        }
    }


    private void Update()
    {
        if (halting)
        {
            if (Input.GetKey(startTravelKey))
            {
                remainingHaltingTime = 0;
            }
            return;
        }
        
        int speedUp = Input.GetKey(speedUpKey) ? speedUpMulti : 1;
        progress += Time.deltaTime * speed * direction * speedUp; ;
        if (progress < 0 || progress >= 1)
        {
            // reset the track progress to last frame's value so that the wagons reading the value
            // and then using it to get their position, don't get an OOBE
            progress = Mathf.Clamp01(progress);
            halting = true;
            if (stopEvent != null)
            {
                stopEvent();
            }
            
            StartCoroutine(WaitToLeaveStation());
            return;
        }
        ProgressTo(progress);
    }

    public bool IsPlayerInside()
    {
        return transform.GetChild(0).GetChild(transform.GetChild(0).childCount - 1).childCount > 1;
    }

    protected virtual void ProgressTo(float progress)
    {
        return;
    }

    IEnumerator WaitToLeaveStation()
    {
        while (remainingHaltingTime > 0)
        {
            remainingHaltingTime -= Time.deltaTime;
            yield return null;
        }
        remainingHaltingTime = haltingTime;
        halting = false;
        direction *= -1;
        if (startEvent != null)
        {
            startEvent();
        }
        
    }

    public float GetRemainingHaltingTime()
    {
        return remainingHaltingTime;
    }

    public float GetProgress()
    {
        return progress;
    }

    protected void SetProgress(float progress)
    {
        this.progress = progress;
    }

    public bool IsHalting()
    {
        return halting;
    }
}
