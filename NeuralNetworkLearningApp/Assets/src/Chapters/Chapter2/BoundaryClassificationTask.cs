using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoundaryClassificationTask : Task
{
    public CoordinateSystem coordSys;
    public int maxWrongClassifications = 0;
    public Button nextButton;
    public override void StartTask()
    {
        coordSys.classificationEvent += DataClassified;
    }

    private void DataClassified(int misclassifiedDataCount)
    {
        print("misclassified: " + misclassifiedDataCount);
        nextButton.interactable = (misclassifiedDataCount <= maxWrongClassifications);
    }

    public void Next()
    {
        nextButton.gameObject.SetActive(false);
        TaskFinished();
    }
}