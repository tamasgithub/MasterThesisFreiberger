using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    private List<Achievement> achievementList;
    private Dictionary<AchievementReqType, float> requirements;
    public GameObject achievementPopupPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        achievementList = new List<Achievement>();
        requirements = new Dictionary<AchievementReqType, float>();
        initRequirements();
        // some test requirements
        achievementList.Add(new Achievement("First quiz solved", "Answer a quiz correctly.",
            (object o) => requirements[AchievementReqType.QUIZES_SOLVED] > 0));
        achievementList.Add(new Achievement("First chapter completed", "Complete a chapter.",
            (object o) => requirements[AchievementReqType.CHAPTERS_COMPLETED] > 0));
        achievementList.Add(new Achievement("First task completed", "Complete a task.",
            (object o) => requirements[AchievementReqType.TASKS_COMPLETED] > 0));
        achievementList.Add(new Achievement("First connection", "Connect two nodes of a network with an edge by hand.", 
            (object o) => requirements[AchievementReqType.MANUAL_CONNECTIONS] > 0));
        achievementList.Add(new Achievement("Large layer", "Create a layer with at least 5 nodes.",
            (object o) => requirements[AchievementReqType.MAX_NODES_IN_LAYER] >= 5));
        achievementList.Add(new Achievement("Deep network", "Create a network with at least 5 layers.",
            (object o) => requirements[AchievementReqType.MAX_LAYERS_IN_NETWORK] >= 5));

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Achievement achievement in achievementList)
        {
            if(achievement.CheckNewCompletion())
            {
                print("Achievement " + achievement.GetTitle() + " complete.");
                GameObject popup = Instantiate(achievementPopupPrefab, GameObject.Find("Canvas").transform);
                popup.transform.SetAsLastSibling();
                popup.transform.GetChild(0).GetComponent<Text>().text = "New achievement complete: " + achievement.GetTitle();
                StartCoroutine(ShowAchievementPopup(popup));
            }
        }
    }

    public void SetRequirement(AchievementReqType type, float value)
    {
        requirements[type] = value;
    }

    public void IncreaseRequirement(AchievementReqType type, float value)
    {
        requirements[type] += value;
    }

    private void initRequirements()
    {
        foreach (AchievementReqType reqType in Enum.GetValues(typeof(AchievementReqType))) {
            requirements.Add(reqType, 0f);
        }
    }

    IEnumerator ShowAchievementPopup(GameObject popup)
    {
        RectTransform rect = popup.GetComponent<RectTransform>();
        while (rect.anchoredPosition.y < 150)
        {
            rect.anchoredPosition += Vector2.up * Time.deltaTime * 100;
            yield return null;
        }
        float upTime = Time.time;
        while (Time.time < upTime + 1f)
        {
            yield return null;
        }
        while (rect.anchoredPosition.y > 0)
        {
            rect.anchoredPosition += Vector2.down * Time.deltaTime * 100;
            yield return null;
        }
    }
}
