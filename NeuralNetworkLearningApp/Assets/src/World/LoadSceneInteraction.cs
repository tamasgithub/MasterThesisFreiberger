using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneInteraction : Interaction
{
    public Sprite newTask;
    public Sprite completedTask;
    public GameObject taskDescriptionUI;
    public string sceneToLoad;
    public string taskDescription;
    private bool displayingDescription;
    public bool shortcut;
    public GameObject[] chapterCompletionVehicles;
    protected override void Start()
    {
        base.Start();
        CheckTaskAndChapterCompletion();
    }

    protected override void Update()
    {
        base.Update();
        if (displayingDescription && GetDistanceFromPlayer() > 3f)
        {
            StopInteraction();
        }

        // shortcutting
        if (shortcut)
        {
            CheckTaskAndChapterCompletion();
        }
    }

    private void CheckTaskAndChapterCompletion()
    {
        bool taskCompleted = Progress.IsTaskCompleted(sceneToLoad);
        GetComponent<SpriteRenderer>().sprite = taskCompleted ? completedTask : newTask;
        if (transform.parent.GetSiblingIndex() == transform.parent.parent.childCount - 1)
        {
            foreach (LoadSceneInteraction loadScene in transform.parent.parent.GetComponentsInChildren<LoadSceneInteraction>())
            {
                if (!Progress.IsTaskCompleted(loadScene.sceneToLoad))
                {
                    return;
                }
            }

            foreach (GameObject vehicle in chapterCompletionVehicles)
            {
                if (vehicle.GetComponent<VehicleController>() != null)
                {
                    vehicle.GetComponent<VehicleController>().enabled = true;
                }
                if (vehicle.GetComponent<Interaction>() != null)
                {
                    vehicle.GetComponent<Interaction>().enabled = true;
                }
            }
        }
    }
    public override void StartInteraction()
    {
        if (!displayingDescription)
        {
            taskDescriptionUI.SetActive(true);
            taskDescriptionUI.GetComponentInChildren<Text>().text = taskDescription.Replace("\\n", "\n");
            displayingDescription = true;
        }
        else
        {
            if (shortcut && Progress.CompleteTask(sceneToLoad))
            {
                //GameObject.Find("AchievementManager").GetComponent<AchievementManager>().IncreaseRequirement(AchievementReqType.TASKS_COMPLETED, 1);
                return;
            }
            
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public override void StopInteraction()
    {
        taskDescriptionUI.GetComponentInChildren<Text>().text = "";
        taskDescriptionUI.SetActive(false);
        displayingDescription = false;
    }
}
