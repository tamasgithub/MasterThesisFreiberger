using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class FMTask : Task
{
    public FunctionMachine[] machines;
    public bool replaceOtherFMs = false;
    public override void StartTask()
    {
        if (replaceOtherFMs)
        {
            
            for (int i = 0; i < transform.parent.childCount; i++) {
                Transform child = transform.parent.GetChild(i);
                if (child.GetComponent<FMTask>() != null && 
                    child.GetSiblingIndex() < transform.GetSiblingIndex())
                {
                    child.GetComponent<FMTask>().HideObjects();
                }
            }
        }

        foreach (FunctionMachine machine in machines)
        {
            machine.transform.parent.gameObject.SetActive(true);
        }
    }

    public virtual void HideObjects()
    {
        foreach (FunctionMachine machine in machines)
        {
            machine.transform.parent.gameObject.SetActive(false);
        }
    }

    public override void TaskFinished()
    {
        base.TaskFinished();
        gameObject.SetActive(false);
    }
}
