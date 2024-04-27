using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// child UI GameObjects are implicitly activated when the holding GO is activated
// other GOs and Components (Behaviours) can be explicitly activated/enabled and deactivated/disabled
public class ActivationTask: Task
{
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;
    public Behaviour[] behavioursToActivate;
    public Behaviour[] behavioursToDeactivate;
    public bool activateAllBehavioursOfTypes;
    public bool deactivateAllBehavioursOfTypes;
    public bool enableBehaviourGOs;
    public bool disableBehaviourGOs;

    //public bool activateHierarchyAboveBehaviours;
    public override void StartTask()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            //obj.SetActive(true);
            ActivateHierarchy(obj);
        }
        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }
        foreach (Behaviour behaviour in behavioursToActivate)
        {
            behaviour.enabled = true;
            if (enableBehaviourGOs)
            {
                behaviour.gameObject.SetActive(true);
            }
            if (activateAllBehavioursOfTypes)
            {
                // need Resources.Find..All to activate deactivated ones
                foreach(Behaviour sameTypedBehaviour in Resources.FindObjectsOfTypeAll(behaviour.GetType())) {
                    sameTypedBehaviour.enabled = true;
                    if (enableBehaviourGOs)
                    {
                        ActivateHierarchy(sameTypedBehaviour.gameObject);
                    }
                }
            }            
        }
        foreach (Behaviour behaviour in behavioursToDeactivate)
        {
            behaviour.enabled = false;
            if (disableBehaviourGOs)
            {
                behaviour.gameObject.SetActive(false);
            }
            if (deactivateAllBehavioursOfTypes)
            {
                // don't use Resources.Find..All !!! bc it disables stuff in prefabs
                foreach (Behaviour sameTypedBehaviour in GameObject.FindObjectsOfType(behaviour.GetType()))
                {
                    sameTypedBehaviour.enabled = false;
                    if (disableBehaviourGOs)
                    {
                        sameTypedBehaviour.gameObject.SetActive(false);
                    }
                }
            }
            
        }
        TaskFinished();
    }

    private void ActivateHierarchy(GameObject go)
    {
        go.SetActive(true);
        GameObject parent = go;
        while (!go.activeInHierarchy)
        {
            parent = parent.transform.parent.gameObject;
            parent.SetActive(true);

        }
    }
}
