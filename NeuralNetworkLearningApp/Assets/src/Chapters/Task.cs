using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task : MonoBehaviour
{
    public event Action taskFinishedEvent;

    public abstract void StartTask();

    public virtual void TaskFinished()
    {
        if (taskFinishedEvent != null)
        {
            taskFinishedEvent();
            print("firing task finished event");
        }
        transform.parent.GetComponent<TaskManager>().TaskFinished();
    }
}
