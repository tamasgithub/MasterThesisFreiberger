using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject chapterDescription;

    public void OnPointerEnter(PointerEventData eventData)
    {
        chapterDescription.GetComponentInChildren<I18NText>()
            .SetKey("DescriptionChapter" + GetComponentInChildren<Text>().text.Substring(GetComponentInChildren<Text>().text.Length-1));
        chapterDescription.SetActive(true);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        chapterDescription.SetActive(false);
    }
}
