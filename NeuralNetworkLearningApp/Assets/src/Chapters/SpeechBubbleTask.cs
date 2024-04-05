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
        if (clickedEvent != null)
        {
            clickedEvent();
        }
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
        // this doesn't work if the speech bubble waits for the very last task to finish, to deactivate
        // just put in a last final toughts speech bubble task
        if (removedAfterFinishedTasks > 0 && transform.parent.
            GetChild(siblingIndex + removedAfterFinishedTasks + 1).gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
    }

}
