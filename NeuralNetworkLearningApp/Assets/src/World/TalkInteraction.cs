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
        print("starting talk interaction");
        if (texts.Length <= currentTextIndex)
        {
            return;
        }
        textUI.SetActive(true);
        textUI.GetComponentInChildren<Text>().text = texts[currentTextIndex++].Replace("\\n", "\n");
        
        textUI.GetComponentInChildren<Image>().sprite = portrait;
    }

    public override void StopInteraction()
    {
        textUI.SetActive(false);
        currentTextIndex = 0;

    }
}
