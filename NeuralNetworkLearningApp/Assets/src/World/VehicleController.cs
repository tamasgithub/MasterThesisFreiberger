using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VehicleController : MonoBehaviour
{
    public event Action stopEvent;
    public event Action startEvent;

    protected int direction = -1;
    public float haltingTime = 5f;
    private float remainingHaltingTime;
    public float speed = 1;
    private bool halting = true;
    private float progress = 0;
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
            return;
        }
        progress += Time.deltaTime * speed * direction;
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

    protected virtual void ProgressTo(float progress)
    {
        return;
    }

    IEnumerator WaitToLeaveStation()
    {
        while (remainingHaltingTime > 0)
        {
            print(remainingHaltingTime);
            remainingHaltingTime -= Time.deltaTime;
            yield return null;
        }
        remainingHaltingTime = haltingTime;
        halting = false;
        direction *= -1;
        print(direction);
        if (startEvent != null)
        {
            startEvent();
        }
        
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
