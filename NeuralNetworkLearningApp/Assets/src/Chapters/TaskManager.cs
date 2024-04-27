using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    // for the tasks inside this task
    private int taskIndex = 0;
    //private Dictionary<, GameObject> instructions;
    
    public void TaskFinished()
    {
        taskIndex++;
        if (taskIndex < transform.childCount)
        {
            transform.GetChild(taskIndex).gameObject.SetActive(true);
            transform.GetChild(taskIndex).GetComponent<Task>().StartTask();
        } else
        {
            if (Progress.CompleteTask(SceneManager.GetActiveScene().name))
            {
                GameObject.Find("AchievementManager").GetComponent<AchievementManager>().IncreaseRequirement(AchievementReqType.TASKS_COMPLETED, 1);
            }
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<Task>().StartTask();
    }

    public void BackToWorld()
    {
        SceneManager.LoadScene("World");
    }
}
