using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMFillInputTask : FMTask
{
    public int countOfInputsToFill = 1;
    public FMInput[] inputs;
    private int inputsFilled = 0;

    private void OnDisable()
    {
        foreach (FunctionMachine machine in machines)
        {
            machine.inputsAcceptedEvent -= InputsAccepted;
        }
    }

    public override void StartTask()
    {
        base.StartTask();
        foreach (FMInput input in inputs)
        {
            input.gameObject.SetActive(true);
        }
        foreach (FunctionMachine machine in machines)
        {
            machine.inputsAcceptedEvent += InputsAccepted;
        }
    }

    private void InputsAccepted()
    {
        inputsFilled++;

        if (inputsFilled == countOfInputsToFill)
        {
            TaskFinished();
        }
    }
}
