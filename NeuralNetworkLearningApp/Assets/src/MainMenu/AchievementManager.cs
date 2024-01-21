using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private List<Achievement> achievementList;
    private Dictionary<AchievementReqTypes, float> requirements;
    
    // Start is called before the first frame update
    void Start()
    {
        achievementList = new List<Achievement>();
        requirements = new Dictionary<AchievementReqTypes, float>();
        initRequirements();
        // some test requirements
        achievementList.Add(new Achievement("First quiz solved", "Answer a quiz correctly.",
            (object o) => requirements[AchievementReqTypes.QUIZES_SOLVED] > 0));
        achievementList.Add(new Achievement("First connection", "Connect two nodes of a network with an edge by hand.", 
            (object o) => requirements[AchievementReqTypes.MANUAL_CONNECTIONS] > 0));
        achievementList.Add(new Achievement("Large layer", "Create a layer with at least 5 nodes.",
            (object o) => requirements[AchievementReqTypes.MAX_NODES_IN_LAYER] >= 5));
        achievementList.Add(new Achievement("Deep network", "Create a network with at least 5 layers.",
            (object o) => requirements[AchievementReqTypes.MAX_LAYERS_IN_NETWORK] >= 5));

    }

    // Update is called once per frame
    void Update()
    {
        requirements[AchievementReqTypes.MANUAL_CONNECTIONS]++;
        foreach (Achievement achievement in achievementList)
        {
            achievement.CheckCompletion();
        }
    }

    public void SetRequirement(AchievementReqTypes type, float value)
    {
        requirements[type] = value;
    }

    public void IncreaseRequirement(AchievementReqTypes type, float value)
    {
        requirements[type] += value;
    }

    private void initRequirements()
    {
        foreach (AchievementReqTypes reqType in Enum.GetValues(typeof(AchievementReqTypes))) {
            requirements.Add(reqType, 0f);
        }
    }
}
