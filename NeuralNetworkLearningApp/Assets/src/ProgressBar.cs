using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private List<int> completedChapters = new List<int>();
    // Start is called before the first frame update
    public void CompleteChapter(int chapterId)
    {
        if (!completedChapters.Contains(chapterId)) {
            completedChapters.Add(chapterId);
            transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(completedChapters.Count, 1, 0);
        }
        
    }
}
