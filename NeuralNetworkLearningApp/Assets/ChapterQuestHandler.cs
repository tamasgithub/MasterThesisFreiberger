using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterQuestHandler : MonoBehaviour
{
    private int currentChapter = -1;
    [SerializeField]
    private Transform[] npcs;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private GameObject questPrefab;
    [SerializeField]
    private Text chapterLabel;

    // chapter -> quest state -> quest string
    private string[][] quests = new string[][] {
        new string[] { "Talk to Alice", "Complete one task on functions" , "Complete all four tasks on functions" , "Talk to Alice"},
        new string[] { "Talk to Bob", "Complete one task on decision boundaries" , "Complete both tasks on decision boundaries" , "Talk to Bob"},
        new string[] { "Talk to Charlie", "Complete one task on NN structure" , "Complete both tasks on NN structure", "Talk to Charlie" },
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int minDistanceChapter = 0;
        float minDistance = (player.position - npcs[0].position).magnitude;
        for (int i = 1; i  < npcs.Length; i++)
        {
            if ((player.position - npcs[i].position).magnitude < minDistance)
            {
                minDistanceChapter = i;
                minDistance = (player.position - npcs[i].position).magnitude;
            }
        }
        if (minDistanceChapter != currentChapter)
        {
            currentChapter = minDistanceChapter;
        }
        UpdateQuests();
    }

    private void UpdateQuests()
    {
        chapterLabel.text = "Chapter " + (currentChapter + 1) + ":";
        int questState = npcs[currentChapter].GetComponent<TalkInteraction>().GetQuestState();
        string[] currentChaptersQuests = quests[currentChapter];
        int numQuestsToShow = Math.Min(questState + 1, currentChaptersQuests.Length);
        if (transform.childCount < numQuestsToShow)
        {
            for (int i = transform.childCount; i < numQuestsToShow; i++)
            {
                Instantiate(questPrefab, transform).GetComponent<RectTransform>().anchoredPosition = Vector2.down * i * 45;

            }
        } else if (transform.childCount > numQuestsToShow)
        {
            for (int i = transform.childCount; i > numQuestsToShow; i--)
            {
                Destroy(transform.GetChild(i-1).gameObject);
            }
        }

        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponentInChildren<Text>().text = currentChaptersQuests[i];
            transform.GetChild(i).GetComponentInChildren<Toggle>().isOn = (i < questState) ;
        }
        

    }
}
