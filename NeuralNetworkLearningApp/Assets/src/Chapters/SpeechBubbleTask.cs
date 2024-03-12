using System;
using UnityEngine.EventSystems;

public class SpeechBubbleTask: Task, IPointerDownHandler
{
    public event Action clickedEvent;
    public bool clickRequiredToFinish = true;
    public bool removedByClick = true;
    public int removedAfterFinishedTasks;
    private int siblingIndex;
    public override void StartTask()
    {
        if (!clickRequiredToFinish)
        {
            TaskFinished();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (removedByClick)
        {
            TaskFinished();
        }
    }

    public override void TaskFinished()
    {
        base.TaskFinished();
        if (removedAfterFinishedTasks == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        siblingIndex = transform.GetSiblingIndex();
    }

    private void Update()
    {
        if (removedAfterFinishedTasks > 0 && transform.parent.
            GetChild(siblingIndex + removedAfterFinishedTasks + 1).gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
    }

}
