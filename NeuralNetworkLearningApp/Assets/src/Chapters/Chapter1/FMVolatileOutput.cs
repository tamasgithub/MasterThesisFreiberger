using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FMVolatileOutput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FunctionMachine[] machines = GameObject.FindObjectsOfType<FunctionMachine>();
        foreach (FunctionMachine machine in machines)
        {
            // destroy this when a new input is processed
            machine.inputsProcessedEvent += DestroySelf;
        }
    }

    private void DestroySelf()
    {
        FunctionMachine[] machines = GameObject.FindObjectsOfType<FunctionMachine>();
        foreach (FunctionMachine machine in machines)
        {
            // destroy this when a new input is processed
            machine.inputsProcessedEvent -= DestroySelf;
        }
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
        
    }

}
