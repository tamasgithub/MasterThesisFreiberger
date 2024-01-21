using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskHandler : MonoBehaviour
{
    public abstract void StartTask();

    public void TaskFinished()
    {
        transform.parent.GetComponent<ChapterManager>().TaskFinished();
    }
}
