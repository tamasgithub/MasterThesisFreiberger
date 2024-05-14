using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class TalkInteraction : Interaction
{
    public Sprite portrait;
    private string[] sentences;
    private int currentSentenceIndex = 0;
    public GameObject textUI;
    public int characterIndex = 0;
    public GameObject chapterEntryPoints;
    public GameObject exclamationMark;

    // TODO: import this from a JSON or other text file
    string[][][] allTexts = new string[][][]
    {
        new string[][] // texts of Alice
        {
            new string[] { "Hello, I'm Alice! How can I help you?",
                ///////////////////////////////////////////////////////////////
                "You want to climb the mountain? I see. I guess you heard\n" +
                "about the rumor that whoever climbs to the top gains the\n" +
                "most secret knowledge about Neural Networks.",
                "By foot, it would take ages.... Of couse you could use one of\n" +
                "my balloons! But you have to do me a favour first I'm afraid.",
                "I want to build a function machine that helps\n" +
                "me estimate the weight limit of my balloons.",
                "I practised with my function machines that you can see here.\n" +
                "But I just can't get it right.",
                "If you would do the tasks too, maybe I could learn\n" +
                "a thing or too by looking over your shoulder.",
                "It would also benefit you in your quest, as\n" +
                "Neural Networks are also just functions.",
                "However, these functions convert inputs into outputs in\n" +
                "higher dimensions, if you are not familiar, then the\n" +
                "tasks will help you a lot!",
                "You can also learn about encoding and decoding techniques\n" +
                "that are essential for Neural networks.",
                "Don't worry, I'll share my knowledge with you as you go."},
            new string[] { "I understand! So just add this and multiply with\n" +
                "that, the radius, the materials... Sorry thats not important.",
                "Thank you, you did wonderfully. You have helped me a lot!",
                "You can use the balloons now. I'm sure you don't\n" +
                "exceed the weight limit since you are travelling alone.",
                "But they won't take you all the way up to the top,\n" +
                "you have to get off as soon as they land.",
                "They take off automatically so make sure you get off in time,\n" +
                "or else you're coming back all the way to here!",
                "Go now! I'm sure you will find many interesting people on your\n" +
                "way to the top, who can teach you a lot about Neural Networks!"}
        },
        new string[][] // texts of Bob
        {
            new string[] { "Greetings! I'm Bob, the train driver along these tracks.",
                "You want to go higher? You are not the only one!",
                "But the coal and the gravel rocks got mixed together.\n" +
                "No idea how that happened.",
                "I am doing my best to separate them so that my old\n" +
                "steam locomotive can be on her way again, but I\n" +
                "have a hard time telling them apart.",
                "You can help me with that. In the end, classifying objects\n" +
                "is one of the most common use cases for Neural Networks.",
                "If you make yourself familiar with the problem, you will\n" +
                "understand better why and how Neural Networks can solve it.",
                "Come back when you have separated enough coal for me." },
            new string[] { "Great! with this much coal, we can make it to the next\n" +
                "station... which is the only other station to be honest.\n" +
                "The terrain gets too steep afterwards.",
                "What is that face? You thought I'll\n" +
                "take you all the way to the top?",
                "You have to earn things like that. Like in my\n" +
                "younger days... Ah you wouldn't understand.",
                "Now get in, or I leave you here,\n" +
                "some of us have to make a living." }
        },
        new string[][] // texts of Charlie
        {
            new string[] { "Hello traveller! You must be on your way to\n" +
                "the top of the mountain. Rest here for a moment!",
                "Seeking knowledge about Neural Networks?\n" +
                "Well, you are in good company here.",
                "You see, the structure of Neural Networks, the endless\n" +
                "possibilites of connections, fascinates me.",
                "That's why in my spare time, I come to this spot and\n" +
                "build networks for fun, while enjoying the amazing view.",
                "If you'd like, I can show you everything I know.",
                "Take a closer look at the networks that I've built\n" +
                "at the clearing. I'll help you build your own!" },
            new string[] { "I'm afraid that's all I know. There is no way to get higher on\n" +
                "the mountain so far. But I'm sure that changes in the near future!",
                "If you completed all chapters, you can take the questionnaire on the\n" +
                "website below the game. Thank you for participating!"}
        }
    };


    // TODO: move all quest related stuff to ChapterQuestHandler and only leave talk related stuff here
    // chapter -> queststate
    private int[][] allTalkQuests = new int[][] { new int[] { 0, 3 }, new int[] { 0, 3 }, new int[] { 0, 3 }};
    private int[] talkQuests;
    private int questState = 0;
    private const string questStateKey = "questState";

    [SerializeField]
    private GameObject[] chapterCompletionVehicles;
    [SerializeField]
    private GameObject[] taskDescriptions;

    protected override void Start()
    {
        base.Start();
        talkQuests = allTalkQuests[characterIndex];
        UpdateQuestState();
    }

    private void UpdateQuestState()
    {
        questState = StaticData.Get<int>(questStateKey + characterIndex);

        CheckTaskCompletion();

        int monologIndex = -1;
        for (int i = 0; i < allTexts[characterIndex].Length; i++)
        {
            if (questState >= talkQuests[i])
            {
                monologIndex++;
                exclamationMark.SetActive(questState == talkQuests[i]);
            }
            else
            {
                break;
            }
        }
        sentences = allTexts[characterIndex][monologIndex];

        foreach (GameObject taskDescription in taskDescriptions)
        {
            taskDescription.SetActive(questState > 0);
        }
    }

    protected override void Update()
    {
        base.Update();
        
    }
    public override void StartInteraction()
    {
        transform.LookAt(player.position);

        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        interacting = true;
        if (sentences.Length == currentSentenceIndex)
        {
            if (exclamationMark.activeSelf)
            {
                IncreaseQuestState();
                exclamationMark.SetActive(false);
            }
            return;
        }
        
        textUI.SetActive(true);
        textUI.GetComponentInChildren<Text>().text = sentences[currentSentenceIndex++].Replace("\\n", "\n");
        textUI.transform.GetChild(0).GetComponentInChildren<Image>().sprite = portrait;
    }

    public override void StopInteraction()
    {
        textUI.SetActive(false);
        currentSentenceIndex = 0;
        interacting = false;
    }

    // this method increases the quest state if task have been completed that would do so
    // the hard-codedness is insane, but no more time to do something better
    private void CheckTaskCompletion()
    {
        questState = StaticData.Get<int>(questStateKey + characterIndex);

        if (questState == 0 || questState == 1)
        {
            bool anyTaskCompleted = false;
            foreach (LoadSceneInteraction loadScene in chapterEntryPoints.GetComponentsInChildren<LoadSceneInteraction>())
            {
                if (Progress.IsTaskCompleted(loadScene.GetSceneToLoad()))
                {
                    anyTaskCompleted = true;
                    break;
                }
            }
            if (anyTaskCompleted)
            {
                if (questState == 0)
                {
                    questState++;
                }
                IncreaseQuestState();
            }
        }
        if (questState == 2)
        {
            bool allTasksCompleted = true;
            foreach (LoadSceneInteraction loadScene in chapterEntryPoints.GetComponentsInChildren<LoadSceneInteraction>())
            {
                if (!Progress.IsTaskCompleted(loadScene.GetSceneToLoad()))
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
        if (questState > talkQuests[talkQuests.Length - 1])
        {
            UnlockVehicles();
        }
    }
    public int GetQuestState()
    {
        return questState;
    }

    private void IncreaseQuestState()
    {
        questState++;
        StaticData.Set(questStateKey + characterIndex, questState);

        // assuming the last quest is always talking to the npc
        if (questState > talkQuests[talkQuests.Length - 1])
        {
            Progress.CompleteChapter(characterIndex+1);
            print("Chapter complete!");
            GameObject.Find("AchievementManager").GetComponent<AchievementManager>().IncreaseRequirement(AchievementReqType.CHAPTERS_COMPLETED, 1);

            UnlockVehicles();
        } else
        {
            UpdateQuestState();
        }
    }

    private void UnlockVehicles()
    {
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
