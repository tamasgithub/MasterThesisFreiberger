using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task : MonoBehaviour
{
    public abstract void StartTask();

    public virtual void TaskFinished()
    {
        transform.parent.GetComponent<TaskManager>().TaskFinished();
    }
}
