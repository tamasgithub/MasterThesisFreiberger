using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Task_1_1_Handler : MonoBehaviour
{
    public GameObject[] texts;
    int step = 0;
    private void Start()
    {
        StartCoroutine(WaitForClickOnText(texts[0]));
    }

    private void Update()
    {
        if (step == 1) {
            print("hello");
        }
    }


    private IEnumerator WaitForClickOnText(GameObject text)
    {
        bool clicked = false;
        text.GetComponent<SpeechBubbleTask>().clickedEvent += (() => clicked = true);
        
        while (!clicked)
        {
            print(clicked);
            yield return new WaitForEndOfFrame();
        }
        step++;
    }
}
