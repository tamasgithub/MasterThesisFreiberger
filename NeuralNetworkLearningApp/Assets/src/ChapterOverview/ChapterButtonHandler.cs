using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChapterButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject chapterDescription;
    private const string PLAYER_POS = "playerPos";
    private const string PLAYER_ROT = "playerRot";

    public void OnPointerEnter(PointerEventData eventData)
    {
        string chapterText = GetComponentsInChildren<Text>()[0].text;
        chapterDescription.GetComponentInChildren<I18NText>()
            .SetKey("DescriptionChapter" + chapterText.Substring(chapterText.Length - 1));
        chapterDescription.SetActive(true);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        chapterDescription.SetActive(false);
    }

    public void LoadWorldAtChapter(int i)
    {
        switch (i)
        {
            case 1:
                // go to Alice in Chapter 1
                StaticData.Set(PLAYER_POS, new Vector3(983, 1, 933));
                StaticData.Set(PLAYER_ROT, new Vector3(0, -40, 0));
                break;
            case 2:
                // go to Bob in Chapter 2
                StaticData.Set(PLAYER_POS, new Vector3(587, 55, 905));
                StaticData.Set(PLAYER_ROT, new Vector3(0, 65, 0));
                break;
            case 3:
                // go to Charlie in Chapter 3
                StaticData.Set(PLAYER_POS, new Vector3(800, 135, 365));
                StaticData.Set(PLAYER_ROT, new Vector3(0, 0, 0));
                break;
        }
        
        SceneManager.LoadScene("World");
    }
}
