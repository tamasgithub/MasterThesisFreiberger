using System;
using UnityEngine;

public class Achievement
{
    private string title;
    private string instruction;
    private Predicate<object> requirement;
    private bool complete;
    public Achievement(string title, string instruction, Predicate<object> requirement)
    {
        this.title = title;
        this.instruction = instruction;
        this.requirement = requirement;
    }

    public bool CheckCompletion()
    {
        if (complete)
        {
            return true;
        }

        if (requirement.Invoke(null))
        {
            // show completion on screen,
            Debug.Log("Achievement " + title + " complete.");
            complete = true;
        }
        return complete;
    }
}
