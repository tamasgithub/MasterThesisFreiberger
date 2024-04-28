using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoundaryClassificationTask : Task
{
    [SerializeField]
    private CoordinateSystem coordSys;
    [SerializeField]
    private int maxWrongClassifications = 0;
    [SerializeField]
    private Button nextButton;
    public override void StartTask()
    {
        coordSys.classificationEvent += DataClassified;
    }

    private void DataClassified(int misclassifiedDataCount)
    {
        nextButton.interactable = (misclassifiedDataCount <= maxWrongClassifications);
    }

    public void Next()
    {
        nextButton.gameObject.SetActive(false);
        coordSys.classificationEvent -= DataClassified;
        TaskFinished();
    }
}
