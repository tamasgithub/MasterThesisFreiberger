using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableActiveDataTask : Task
{
    // Start is called before the first frame update
    public override void StartTask()
    {
        foreach(FMInput input in FindObjectsOfType(typeof(FMInput)))
        {
            input.gameObject.SetActive(false);
        }
        TaskFinished();
    }
    
}
