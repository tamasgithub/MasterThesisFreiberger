using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkInteraction : Interaction
{
    public Sprite portrait;
    public string[] texts;
    private int currentTextIndex = 0;
    public GameObject textUI;
    private bool interacting = false;

    protected override void Update()
    {
        base.Update();
        if (interacting && GetDistanceFromPlayer() > 3f)
        {
            StopInteraction();
        }
    }
    public override void StartInteraction()
    {
        interacting = true;
        print("start interaction " + transform.name);
        if (texts.Length <= currentTextIndex)
        {
            return;
        }
        textUI.SetActive(true);
        textUI.GetComponentInChildren<Text>().text = texts[currentTextIndex++].Replace("\\n", "\n");
        
        textUI.transform.GetChild(0).GetComponentInChildren<Image>().sprite = portrait;
    }

    public override void StopInteraction()
    {
        print("stop interaction " + transform.name);
        textUI.SetActive(false);
        currentTextIndex = 0;

    }
}
