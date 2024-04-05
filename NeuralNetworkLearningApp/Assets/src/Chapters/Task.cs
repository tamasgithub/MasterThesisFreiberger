using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task : MonoBehaviour
{
    public abstract void StartTask();

    public virtual void TaskFinished()
    {
        print("Task finished!");
        transform.parent.GetComponent<TaskManager>().TaskFinished();
    }
}
