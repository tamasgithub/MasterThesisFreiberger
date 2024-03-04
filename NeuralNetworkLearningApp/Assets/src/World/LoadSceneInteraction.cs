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
    public string chapterToLoad;
    public string taskDescription;
    private bool displayingDescription;
    protected override void Start()
    {
        base.Start();
        // TODO: load info about task being completed and set sprite accordingly
        GetComponent<SpriteRenderer>().sprite = newTask;
    }

    protected override void Update()
    {
        base.Update();
        if (displayingDescription && GetDistanceFromPlayer() > 3f)
        {
            StopInteraction();
        }
    }
    public override void StartInteraction()
    {
        print("StartInteraction of " + transform.name);
        if (!displayingDescription)
        {
            print("taskDescriptionUI activating");
            taskDescriptionUI.SetActive(true);
            taskDescriptionUI.GetComponentInChildren<Text>().text = taskDescription.Replace("\\n", "\n");
            displayingDescription = true;
        }
        else
        {
            SceneManager.LoadScene(chapterToLoad);
        }
    }

    public override void StopInteraction()
    {
        print("StopInteraction of " + transform.name);
        taskDescriptionUI.GetComponentInChildren<Text>().text = "";
        taskDescriptionUI.SetActive(false);
        displayingDescription = false;
    }
}
