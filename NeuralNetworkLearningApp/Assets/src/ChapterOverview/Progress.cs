using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

public static class Progress
{
    // e.g 1, 2, ...
    private static List<int> completedChapters = new List<int>();
    // e.g. Task1.1, Task1.2, Task2.1, ...
    private static List<string> completedTasks = new List<string>();

    public static bool CompleteChapter(int chapterId)
    {
        if (IsChapterCompleted(chapterId))
        {
            return false;
        }
        completedChapters.Add(chapterId);
        // persistance for WegGL builds in form of cookies
#if UNITY_WEBGL && !UNITY_EDITOR
        JSHook.SetCookie("progress=" + GetProgressAsString() + ";SameSite=lax;max-age=31536000");
#endif
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
#if UNITY_WEBGL && !UNITY_EDITOR
        JSHook.SetCookie("progress=" + GetProgressAsString() + ";SameSite=lax;max-age=31536000");
#endif
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
        ProgressData data = new ProgressData();
        data.completedTasks = completedTasks;
        data.completedChapters = completedChapters;
        return JsonUtility.ToJson(data);
    }

    public static void LoadProgressFromString(string progress)
    {
        Debug.Log("Loading progress from " + progress);
        if (progress == null || progress == "")
        {
            return;
        }
        try
        {
            ProgressData data = JsonUtility.FromJson<ProgressData>(progress);
            Progress.completedTasks = data.completedTasks;
            Progress.completedChapters = data.completedChapters;
            Debug.Log("Loading progress successful " + Progress.GetProgressAsString());
        } catch
        {
            Debug.LogError("Loading progress from cookies failed");
        }
    }
}

[System.Serializable]
class ProgressData
{
    public List<int> completedChapters;
    public List<string> completedTasks;
}
