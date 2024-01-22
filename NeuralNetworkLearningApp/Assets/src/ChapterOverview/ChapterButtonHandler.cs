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

    public void StartChapter(int i)
    {
        SceneManager.LoadScene("Chapter" + i);
    }
}
