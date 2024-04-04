using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotTask : Task
{
    public CoordinateSystem coordSys;
    public int numberOfDataToPlot;
    private int plottedData = 0;
    public override void StartTask()
    {
        coordSys.dataPlottedEvent += DataPlotted;
        foreach (PlottableData data in GameObject.FindObjectsOfType<PlottableData>())
        {
            data.GetComponent<PlottableData>().SetDraggable(true);
        }
    }

    private void DataPlotted()
    {
        plottedData++;
        if (plottedData == numberOfDataToPlot)
        {
            TaskFinished();
        }
    }

}
