using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private GameObject[] checkmarks;
    [SerializeField]
    private int maxChapters = 3;

    private void Start()
    {
        for (int i = 0; i < checkmarks.Length; i++) {
            if (Progress.IsChapterCompleted(i+1))
            {
                checkmarks[i].SetActive(true);
                transform.GetChild(0).GetComponent<RectTransform>().localScale += Vector3.right * 1.0f/maxChapters;
            }    
        }
        
    }
}
