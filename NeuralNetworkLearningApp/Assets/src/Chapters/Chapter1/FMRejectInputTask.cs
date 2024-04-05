using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMRejectInputTask : FMTask
{
    public FMInput[] inputs;
    public int countOfInputsToReject = 1;
    private int inputsRejected = 0;
    
    public override void StartTask()
    {
        base.StartTask();
        foreach (FMInput input in inputs)
        {
            input.gameObject.SetActive(true);
        }
        foreach (FunctionMachine machine in machines)
        {
            foreach(FMInputHole inputHole in machine.transform.parent.GetComponentsInChildren<FMInputHole>())
            {
                inputHole.inputRejectedEvent += InputRejected;
            }
        }
    }
    private void OnDisable()
    {
        foreach (FunctionMachine machine in machines)
        {
            machine.inputsProcessedEvent -= InputRejected;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InputRejected()
    {
        inputsRejected++;
        if (inputsRejected == countOfInputsToReject)
        {
            TaskFinished();
        }
    }
}
