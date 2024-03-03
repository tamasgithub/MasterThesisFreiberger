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
    public Vector3 taskDescriptionOffset;
    public string chapterToLoad;
    public string taskDescription;
    protected override void Start()
    {
        base.Start();
        // TODO: load info about task being completed and set sprite accordingly
        GetComponent<SpriteRenderer>().sprite = newTask;
    }

    protected override void Update()
    {
        base.Update();
        if (GetDistanceFromPlayer() > 3f)
        {
            StopInteraction();
        }
    }
    public override void StartInteraction()
    {
        if (!taskDescriptionUI.activeSelf)
        {
            taskDescriptionUI.SetActive(true);
            taskDescriptionUI.GetComponentInChildren<Text>().text = taskDescription.Replace("\\n", "\n");
        }
        else
        {
            SceneManager.LoadScene(chapterToLoad);
        }
    }

    public override void StopInteraction()
    {
        taskDescriptionUI.SetActive(false);
    }
}
