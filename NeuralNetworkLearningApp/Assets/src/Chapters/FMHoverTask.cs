using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMHoverTask : FMTask
{
    private void Start()
    {
        foreach (FunctionMachine machine in machines)
        {
            machine.hoverEvent += OnHover;
        }
    }
    private void OnDisable()
    {
        foreach (FunctionMachine machine in machines)
        {
            machine.hoverEvent -= OnHover;
        }
    }

    public override void StartTask()
    {
        base.StartTask();
    }

    private void OnHover()
    {
        TaskFinished();
    }
}
