using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    private int taskIndex = 0;
    //private Dictionary<, GameObject> instructions;
    
    public void TaskFinished()
    {
        taskIndex++;
        if (taskIndex < transform.childCount)
        {
            transform.GetChild(taskIndex).gameObject.SetActive(true);
            transform.GetChild(taskIndex).GetComponent<TaskHandler>().StartTask();
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<TaskHandler>().StartTask();
    }
}
