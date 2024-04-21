using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Progress
{
    // e.g 1, 2, ...
    private static List<int> completedChapters = new List<int>();
    // e.g. 1.1, 1.2, 2.1, ...
    private static List<string> completedTasks = new List<string>();

    public static bool CompleteChapter(int chapterId)
    {
        if (IsChapterCompleted(chapterId))
        {
            return false;
        }
        completedChapters.Add(chapterId);
        // persistance for WegGL builds in form of cookies
        JSHook.SetCookie("progress=" + Progress.GetProgressAsString() + ";SameSite=None");
        return true;
    }

    public static bool CompleteTask(string task)
    {
        if (IsTaskCompleted(task))
        {
            return false;
        }
        completedTasks.Add(task);
        // persistance for WegGL builds in form of cookies
        JSHook.SetCookie("progress=" + Progress.GetProgressAsString() + ";SameSite=None");
        return true;
    }


    public static int GetCompletedChaptersCount()
    {
        return completedChapters.Count;
    }

    public static bool IsChapterCompleted(int chapterId) {
        return completedChapters.Contains(chapterId);
    }
    public static bool IsTaskCompleted(string task)
    {
        return completedTasks.Contains(task);
    }

    public static string GetProgressAsString()
    {
        string result = "Chapters:";
        foreach (int chapterId in completedChapters)
        {
            result += chapterId.ToString() + "-";
        }
        if (result[result.Length - 1] == '-')
        {
            result = result.Substring(0, result.Length - 1) + "_";
        }
        else
        {
            result += "_";
        }
        result += "Tasks:";
        foreach (string task in completedTasks)
        {
            result += task + "-";
        }
        if (result[result.Length - 1] == '-')
        {
            result = result.Substring(0, result.Length - 1);
        }
        return result;
    }

    public static void LoadProgressFromString(string progress)
    {

    }
}
