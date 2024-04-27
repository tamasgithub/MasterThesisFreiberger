using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[SerializeField] class LoadSceneInteraction : Interaction
{
    [SerializeField]
    private Sprite newTask_small;
    [SerializeField]
    private Sprite completedTask_small;
    [SerializeField]
    private Sprite newTask_large;
    [SerializeField]
    private Sprite completedTask_large;
    [SerializeField]
    private GameObject taskDescriptionUI;
    [SerializeField]
    private string sceneToLoad;
    [SerializeField]
    private string taskDescription;
    [SerializeField]
    private bool shortcut;
    protected override void Start()
    {
        base.Start();
        // start is also called when returning to the world from a task
        CheckTaskAndChapterCompletion();
    }

    protected override void Update()
    {
        base.Update();

        // shortcutting
        if (shortcut)
        {
            CheckTaskAndChapterCompletion();
        }
    }

    private void CheckTaskAndChapterCompletion()
    {
        bool taskCompleted = Progress.IsTaskCompleted(sceneToLoad);
        GetComponent<SpriteRenderer>().sprite = taskCompleted ? completedTask_small : newTask_small;
    }

    public override void StartInteraction()
    {
        if (!interacting)
        {
            interacting = true;
            taskDescriptionUI.SetActive(true);
            taskDescriptionUI.GetComponentInChildren<Text>().text = taskDescription.Replace("\\n", "\n");
            taskDescriptionUI.GetComponentInChildren<Image>().sprite = Progress.IsTaskCompleted(sceneToLoad) ? completedTask_large : newTask_large;
        }
        else
        {
            if (shortcut && Progress.CompleteTask(sceneToLoad))
            {
                GameObject.Find("AchievementManager").GetComponent<AchievementManager>().IncreaseRequirement(AchievementReqType.TASKS_COMPLETED, 1);
                return;
            }
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public override void StopInteraction()
    {
        taskDescriptionUI.GetComponentInChildren<Text>().text = "";
        taskDescriptionUI.SetActive(false);
        interacting = false;
    }

    public string GetSceneToLoad()
    {
        return sceneToLoad;
    }
}
