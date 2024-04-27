using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;

public class FMProcessInputsTask : FMTask
{
    public int numberOfInputsToProcess = 1;
    public FMInput[] inputs;
    public bool processExactlyTheSpecifiedInputs;
    
    public GameObject outputBuckets;

    private int inputsProcessed = 0;

    private void OnDisable()
    {
        foreach (FunctionMachine machine in machines)
        {
            machine.inputsProcessedEvent -= InputsProcessed;
        }
    }


    private void InputsProcessed()
    {
        inputsProcessed++;

        if (processExactlyTheSpecifiedInputs)
        {
            bool allProcessed = true;
            foreach(FMInput input in inputs)
            {
                if (input != null)
                {
                    allProcessed = false;
                    break;
                }
            }
            if (allProcessed)
            {
                TaskFinished();
            }
        }
        else
        {
            if (inputsProcessed == numberOfInputsToProcess)
            {
                TaskFinished();
            }
        }
    }


    public override void StartTask()
    {
        base.StartTask();
        foreach (FMInput input in inputs)
        {
            if (input != null)
            {
                input.gameObject.SetActive(true);
            }
        }
        outputBuckets.SetActive(true);
        foreach (FunctionMachine machine in machines)
        {
            machine.inputsProcessedEvent += InputsProcessed;
        }
        if (processExactlyTheSpecifiedInputs)
        {
            // this variable is not used in this case and is set to -1 to indicate this
            numberOfInputsToProcess = -1;
        }
    }

    public override void TaskFinished()
    {
        base.TaskFinished();
    }

    public override void HideObjects()
    {
        base.HideObjects();
        outputBuckets.SetActive(false);
    }
}
