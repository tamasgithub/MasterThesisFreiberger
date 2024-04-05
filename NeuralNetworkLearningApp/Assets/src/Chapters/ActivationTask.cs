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
    //public bool activateAllBehavioursOfTypes;
    //public bool activateHierarchyAboveBehaviours;
    public override void StartTask()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }
        foreach (Behaviour behaviour in behavioursToActivate)
        {
            behaviour.enabled = true;
            /*if (activateAllBehavioursOfTypes)
            {
                foreach(Behaviour sameTypedBehaviour in Resources.FindObjectsOfTypeAll(behaviour.GetType())) {
                    sameTypedBehaviour.enabled = true;
                }
            } else
            {
                behaviour.enabled = true;
            }

            if (activateHierarchyAboveBehaviours)
            {
                GameObject parent = behaviour.gameObject;
                while (!parent.activeInHierarchy && !parent.activeSelf)
                {
                    parent.SetActive(true);
                    parent = parent.transform.parent.gameObject;
                }
            }*/
            
        }
        foreach (Behaviour behaviour in behavioursToDeactivate)
        {
            behaviour.enabled = false;
        }
        TaskFinished();
    }
}
