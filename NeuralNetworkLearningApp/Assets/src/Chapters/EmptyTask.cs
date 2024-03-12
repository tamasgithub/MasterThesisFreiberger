using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used for tasks that are already finished by enabling the UI gameobject e.g. just displaying an image
public class EmptyTask : Task
{
    public override void StartTask()
    {
        TaskFinished();
    }
}
