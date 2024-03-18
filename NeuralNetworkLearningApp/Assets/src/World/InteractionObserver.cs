using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObserver : MonoBehaviour
{
    private List<Interaction> subscribers;


    public void Subscribe(Interaction interaction)
    {
        if (subscribers == null)
        {
            subscribers = new List<Interaction>();
        }
        if (!subscribers.Contains(interaction))
        {
            subscribers.Add(interaction);
            print("interaction subscribed");
        }
        
    }

    public void Unsubscribe(Interaction interaction)
    {
        if (subscribers == null)
        {
            subscribers = new List<Interaction>();
        }
        subscribers.Remove(interaction);
    }


    // Update is called once per frame
    void Update()
    {
        
        Interaction minDistanceInteraction = null;
        float minDistance = float.MaxValue;
        foreach (Interaction interaction in subscribers)
        {
            float distance = interaction.GetDistanceFromPlayer();
            if (distance < minDistance)
            {
                minDistance = distance;
                minDistanceInteraction = interaction;
            }
        }
        transform.GetChild(0).gameObject.SetActive(minDistance < minDistanceInteraction.GetRange());
        
        if (minDistance < minDistanceInteraction.GetRange())
        {
            minDistanceInteraction.DisplayUIToInteract();
        }
        foreach (Interaction interaction in subscribers)
        {
            if (interaction == minDistanceInteraction)
            {
                continue;
            }
            interaction.RemoveUIToInteract();
        }
    }
}
