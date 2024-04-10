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
}
