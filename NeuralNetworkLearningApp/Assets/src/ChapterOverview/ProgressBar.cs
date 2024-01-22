using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    private void Start()
    {
        transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(Progress.GetCompletedChaptersCount(), 1, 0);
    }

    // Start is called before the first frame update
    public void CompleteChapter(int chapterId)
    {
        if (Progress.CompleteChapter(chapterId))
        {
            transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(Progress.GetCompletedChaptersCount(), 1, 0);
        }
        

    }
}
