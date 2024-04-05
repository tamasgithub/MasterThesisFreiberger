using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNChangeUISettingsTask : Task
{
    public NN network;
    public bool showValues = false;
    public bool colorEdges = false;
    public bool edgeHoveringEnabled = false;
    public bool nodeHoveringEnabled = false;
    public bool editingEnabled = false;

    public override void StartTask()
    {
        print("Setting to " + showValues + colorEdges + edgeHoveringEnabled + nodeHoveringEnabled + editingEnabled);
        network.SetUISettings(showValues, colorEdges, edgeHoveringEnabled, nodeHoveringEnabled, editingEnabled);
        TaskFinished();
    }
}
