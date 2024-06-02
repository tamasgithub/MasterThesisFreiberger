using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsList : MonoBehaviour
{
    [SerializeField]
    Transform contents;
    [SerializeField]
    GameObject achievementPrefab;
    [SerializeField]
    Color completedColor;
    // Start is called before the first frame update
    void Start()
    {
        List<Achievement> list = AchievementManager.GetAchievements();
        foreach (Achievement achievement in list)
        {
            GameObject newObject = Instantiate(achievementPrefab, contents);
            Text[] texts = newObject.GetComponentsInChildren<Text>();
            texts[0].text = achievement.GetTitle();
            texts[1].text = achievement.GetInstruction();
            if (achievement.IsComplete())
            {
                newObject.transform.SetAsFirstSibling();
                newObject.GetComponent<Image>().color = completedColor;
                texts[0].color = Color.white;
                texts[1].color = Color.white;
            } else
            {
                newObject.transform.SetAsLastSibling();
                
            }
        }

        foreach (Transform achievementElement in contents) {
            RectTransform rect = achievementElement.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector3.down * achievementElement.GetSiblingIndex() * rect.sizeDelta.y * 1.05f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
