using System;
using UnityEngine.EventSystems;

public class SpeechBubbleTask: Task, IPointerDownHandler
{
    public event Action clickedEvent;
    public bool clickRequiredToFinish = true;
    public bool removedByClick = true;
    public int removedAfterFinishedTasks;
    private int siblingIndex;

    private void Start()
    {
        print("start with sibling index " + transform.GetSiblingIndex());
    }

    public override void StartTask()
    {
        print("starttask with sibling index " + transform.GetSiblingIndex());
        if (!clickRequiredToFinish)
        {
            TaskFinished();
            // remove arrow
            transform.GetChild(transform.childCount - 1).gameObject.SetActive(false);
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
        } else
        {
            print("setting listener to " + transform.parent.GetChild(transform.GetSiblingIndex() + removedAfterFinishedTasks).name);
            transform.parent.GetChild(transform.GetSiblingIndex() + removedAfterFinishedTasks)
                .GetComponent<Task>().taskFinishedEvent += () =>
                {
                    gameObject.SetActive(false);
                    print("set to false");
                };
        }
    }
}
