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

    public bool CheckNewCompletion()
    {
        if (complete)
        {
            return false;
        }

        complete = requirement.Invoke(null);
        return complete;
    }

    public string GetTitle()
    {
        return title;
    }
}
