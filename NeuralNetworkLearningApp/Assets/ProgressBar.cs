using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public GameObject[] chapterButtons;
    private int completedChapters = 0;
    // Start is called before the first frame update
    public void CompleteChapter(int chapterId)
    {
        if (chapterId == completedChapters + 1) { 
            completedChapters++;
            if (completedChapters < chapterButtons.Length)
            {
                chapterButtons[completedChapters].GetComponent<Button>().interactable = true;
            }
            transform.GetChild(0).GetComponent<RectTransform>().localScale += new Vector3(1, 0, 0);
        }
        
    }
}
