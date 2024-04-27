using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// TODO: extract the showPopup method to a script only handling the popups, fire an event when an achievement is reached
// and make that script listen to those events, then this whole class can become static
public class AchievementManager : MonoBehaviour
{
    private static List<Achievement> achievements;
    private static Dictionary<AchievementReqType, float> requirements;
    public GameObject achievementPopupPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        print("Start");
        if (requirements == null)
        {
            requirements = new Dictionary<AchievementReqType, float>();
            InitRequirements();
        }

        if (achievements == null)
        {
            achievements = new List<Achievement>();
            InitAchievements();
        }        

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Achievement achievement in achievements)
        {
            if(achievement.CheckNewCompletion())
            {
                print("Achievement " + achievement.GetTitle() + " complete.");
                GameObject popup = Instantiate(achievementPopupPrefab, GameObject.Find("Canvas").transform);
                popup.transform.SetAsLastSibling();
                popup.transform.GetChild(0).GetComponent<Text>().text = "New achievement: " + achievement.GetTitle();
                StartCoroutine(ShowAchievementPopup(popup));
#if UNITY_WEBGL && !UNITY_EDITOR
                JSHook.SetCookie("achievements=" + GetAchievementsAsString() + ";SameSite=lax");
#endif
            }
        }
    }

    public void SetRequirement(AchievementReqType type, float value)
    {
        requirements[type] = value;
#if UNITY_WEBGL && !UNITY_EDITOR
        JSHook.SetCookie("achievements=" + GetAchievementsAsString() + ";SameSite=lax");
#endif
    }

    public void IncreaseRequirement(AchievementReqType type, float value)
    {
        requirements[type] += value;
#if UNITY_WEBGL && !UNITY_EDITOR
        JSHook.SetCookie("achievements=" + GetAchievementsAsString() + ";SameSite=lax");
#endif
    }

    public float GetRequirement(AchievementReqType type)
    {
        return requirements[type];
    }

    public static List<Achievement> GetAchievements()
    {
        return achievements;
    }

    public static void LoadAchievementsFromString(string achievementsString)
    {
        Debug.Log("Loading achievements from " + achievementsString);
        if (achievementsString == null || achievementsString == "")
        {
            return;
        }
        try
        {
            AchievementData data = JsonUtility.FromJson<AchievementData>(achievementsString);
            requirements = data.requirements.ToDictionary();
            //achievements = data.achievements;
            Debug.Log("Loading achievements successful " + GetAchievementsAsString());
        } catch
        {
            Debug.LogError("Loading achievements from cookies failed");
        }
    }

    private void InitRequirements()
    {
        foreach (AchievementReqType reqType in Enum.GetValues(typeof(AchievementReqType))) {
            requirements.Add(reqType, 0f);
        }
#if UNITY_WEBGL && !UNITY_EDITOR
            JSHook.SetCookie("achievements=" + GetAchievementsAsString() + ";SameSite=lax");
#endif
    }

    private void InitAchievements()
    {
        achievements.Add(new Achievement("First chapter completed", "Complete all tasks of any chapter.",
            (object o) => requirements[AchievementReqType.CHAPTERS_COMPLETED] > 0));
        achievements.Add(new Achievement("First task completed", "Complete a task of any chapter.",
            (object o) => requirements[AchievementReqType.TASKS_COMPLETED] > 0));
        achievements.Add(new Achievement("Master of connections", "Connect more than 20 nodes with an edge by hand.",
            (object o) => requirements[AchievementReqType.MANUAL_CONNECTIONS] > 20));
        achievements.Add(new Achievement("Layering specialist", "Create a layer with at least 4 nodes.",
            (object o) => requirements[AchievementReqType.MAX_NODES_IN_LAYER] >= 4));
        //not possible atm
        achievements.Add(new Achievement("Deep network user", "Create a network with at least 6 layers.",
            (object o) => requirements[AchievementReqType.MAX_LAYERS_IN_NETWORK] >= 6));
        /* Not yet implemented
        achievements.Add(new Achievement("Nice view", "Visit 3 different viewing platforms.",
            (object o) => requirements[AchievementReqType.VISITED_VIEWING_PLATFORMS] >= 3));
        achievements.Add(new Achievement("Not all hot air", "Travel from chapter 1 to 2 or back 3 times.",
            (object o) => requirements[AchievementReqType.TRAVELED_BY_BALLOON] >= 3));
        achievements.Add(new Achievement("Train lover", "Travel from chapter 2 to 3 or back 5 times.",
            (object o) => requirements[AchievementReqType.TRAVELED_BY_TRAIN] >= 5));
        */
        foreach (Achievement achievement in achievements)
        {
            // sets the completion according to loaded requirements without popup etc., which occurs on new completion
            achievement.CheckNewCompletion();
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

    private static string GetAchievementsAsString()
    {
        AchievementData data = new AchievementData();
        data.requirements = new SerializableDictionary<AchievementReqType, float>(requirements);
        //data.achievements = achievements;
        return JsonUtility.ToJson(data);
    }
}

[System.Serializable]
class AchievementData
{
    //public List<Achievement> achievements;
    [SerializeField]
    public SerializableDictionary<AchievementReqType, float> requirements;
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public SerializableDictionary(Dictionary<TKey, TValue> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        var dictionary = new Dictionary<TKey, TValue>();
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
        return dictionary;
    }

    private void Clear()
    {
        keys.Clear();
        values.Clear();
    }
}
