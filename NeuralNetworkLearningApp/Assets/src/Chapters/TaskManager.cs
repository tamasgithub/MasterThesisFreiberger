using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    public int chapterIndex = 0;
    private int taskIndex = 0;
    //private Dictionary<, GameObject> instructions;
    
    public void TaskFinished()
    {
        taskIndex++;
        if (taskIndex < transform.childCount)
        {
            transform.GetChild(taskIndex).gameObject.SetActive(true);
            transform.GetChild(taskIndex).GetComponent<TaskHandler>().StartTask();
        } else
        {
            if (Progress.CompleteChapter(chapterIndex))
            {
                GameObject.Find("AchievementManager").GetComponent<AchievementManager>().IncreaseRequirement(AchievementReqType.CHAPTERS_COMPLETED, 1);
            }
            SceneManager.LoadScene("World");
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<TaskHandler>().StartTask();
    }

    public void BackToChapterOverview()
    {
        SceneManager.LoadScene("World");
    }
}