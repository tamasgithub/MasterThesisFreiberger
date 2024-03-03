using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpeechBubbleHandler : TaskHandler, IPointerDownHandler
{ 
    public override void StartTask()
    {
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        TaskFinished();
    }

}
