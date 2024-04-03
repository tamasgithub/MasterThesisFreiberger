using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TalkInteraction : Interaction
{
    public Sprite portrait;
    private string[] texts;
    private int currentTextIndex = 0;
    public GameObject textUI;
    private bool interacting = false;
    public bool hasNewText;
    public int characterIndex = 0;
    public GameObject chapterEntryPoints;
    private int questState = 0;
    public GameObject exclamationMark;

    // TODO: import this from a JSON or other text file
    string[][][] allTexts = new string[][][]
    {
        new string[][] // texts of Alice
        {
            new string[] { "Hello, I'm Alice! How can I help you?",
                "You want to climb the mountain? I see. I guess you heard about the rumor that\nwhoever climbs to the top gains the most secret knowledge about Neural Networks.",
                "By foot, it would take ages.... Of couse you could use my balloons!\nBut you have to do me a favour first I'm affraid.",
                "I want to build a function machine that helps me estimate the weight limit of my balloons.",
                "I practised with my function machines that you can see here. But I just can't get it right.",
                "If you would do the tasks too, maybe I could learn a thing\nor too by looking over your shoulder.",
                "It would also benefit you in your quest, as Neural Networks are also just just functions.",
                "However, these functions convert inputs into outputs in\nhigher dimensions, if you are not familiar, then the tasks will help you a lot!",
                "You can also learn about encoding and decoding techniques\nthat are essential for Neural networks.",
                "Don't worry, I'll share my knowledge with you as you go."},
            new string[] { "Good job there in the last task. Keep going and complete the remaining ones!"},
            new string[] { "I understand! So just add this and multiply with that,\nthe radius, the materials... Sorry thats not important.",
                "I'm sure you don't exceed the weight limit if you travel alone. Thank you for your help.",
                "You can now use the balloons. But they won't take you up to the top,\nyou have to get off as soon as they land.",
                "They take off automatically so make sure you get off in time,\nor else you'll have to come back all the way to here!",
                "Go now! I'm sure you will find many interesting people on your\nway to the top, who can teach you a lot about Neural Networks!"}
        },
        new string[][] // texts of Bob
        {
            new string[] { "Greetings! I'm Bob, the train driver along these tracks.",
                "You want to go higher? You are not the only one! But the coal and\ngravel rocks got mixed together. No idea how that happened.",
                "I am doing my best to separate them so that my old steam locomotive\ncan be on her way again, but I have a hard time telling the rocks apart.",
                "You can help me with that. In the end, classifying\nobjects is one of the most common use cases for Neural Networks.",
                "If you make yourself familiar with the problem, you will understand\nbetter why and how Neural Networks can solve it.",
                "Come back when you have separated some coal for me." },
            new string[] {"Not bad, with these skills, you would make a somewhat decent train driver.",
                "But the coal is not enough, we need more. Complete the remainnig tasks!"},
            new string[] { "Great! with this much coal, we can make it to the next station.\nWhich is the only other station, the terrain gets too steep after that.",
                "What is that face? You thought I'll take you all the way to the top?",
                "You have to earn things like that. Like in my younger days. Ah you wouldn't understand.",
                "Now get in, or I leave you here. Some of us have to make a living." }
        }
    };
    private const string hasNewTextKey = "hasNewText";
    private const string questStateKey = "questState";

    private void Start()
    {
        base.Start();
        if (StaticData.HasKey(hasNewTextKey + characterIndex));
        {
            hasNewText = StaticData.Get<bool>(hasNewTextKey + characterIndex);
        }
        CheckTaskCompletion();
        
        exclamationMark.SetActive(hasNewText);
        texts = allTexts[characterIndex][questState];
    }

    protected override void Update()
    {
        base.Update();
        if (interacting && (GetDistanceFromPlayer() > 3f || Input.GetKeyDown(KeyCode.Escape)))
        {
            StopInteraction();
        }
        
    }
    public override void StartInteraction()
    {

        transform.LookAt(player.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        interacting = true;
        print("start interaction " + transform.name);
        if (texts.Length == currentTextIndex)
        {
            hasNewText = false;
            StaticData.Set(hasNewTextKey + characterIndex, false);
            exclamationMark.SetActive(false);
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
        interacting = false;
    }

    // this method increases the quest state if task have been completed that would do so
    private void CheckTaskCompletion()
    {
        questState = StaticData.Get<int>(questStateKey + characterIndex);
        if (questState == 0)
        {
            bool anyTaskCompleted = false;
            foreach (LoadSceneInteraction loadScene in chapterEntryPoints.GetComponentsInChildren<LoadSceneInteraction>())
            {
                if (Progress.IsTaskCompleted(loadScene.sceneToLoad))
                {
                    anyTaskCompleted = true;
                    break;
                }
            }
            if (anyTaskCompleted)
            {
                IncreaseQuestState();
            }
        }
        if (questState == 1)
        {
            bool allTasksCompleted = true;
            foreach (LoadSceneInteraction loadScene in chapterEntryPoints.GetComponentsInChildren<LoadSceneInteraction>())
            {
                if (!Progress.IsTaskCompleted(loadScene.sceneToLoad))
                {
                    allTasksCompleted = false;
                    break;
                }
            }
            if (allTasksCompleted)
            {
                IncreaseQuestState();
            }
        }
    }

    private void IncreaseQuestState()
    {
        questState++;
        StaticData.Set(questStateKey + characterIndex, questState);
        hasNewText = !(questState >= allTexts[characterIndex].Length);
        if (hasNewText)
        {
            exclamationMark.SetActive(true);
        }
    }
}
