using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used for tasks that are already finished by enabling the gameobject e.g. just displaying an image
public class EmptyTaskHandler : TaskHandler
{
    public override void StartTask()
    {
        TaskFinished();
    }
}
