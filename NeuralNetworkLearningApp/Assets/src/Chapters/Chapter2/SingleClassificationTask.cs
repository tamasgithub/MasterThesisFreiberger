using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleClassificationTask : Task
{
    public GradientSelector[] selectors;
    public int requiredClassChanges;
    private bool firstEvent = true;
    private int lastClass;
    private int classChanges;

    public override void StartTask()
    {
        foreach (GradientSelector selector in selectors)
        {
            selector.classificationEvent += SingleDataClassified;
        }
    }

    private void SingleDataClassified(int classifiedAs)
    {

        if (firstEvent)
        {
            firstEvent = false;
        } else
        {
            if (classifiedAs != lastClass)
            {
                lastClass = classifiedAs;
                classChanges++;
            }
            if (classChanges == requiredClassChanges)
            {
                TaskFinished();
                foreach (GradientSelector selector in selectors)
                {
                    selector.classificationEvent -= SingleDataClassified;
                }
            }
        }
        lastClass = classifiedAs;
    }

}
