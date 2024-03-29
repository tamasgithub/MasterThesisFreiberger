using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Progress
{
    private static List<int> completedChapters = new List<int>();
    private static List<string> completedTasks = new List<string>();

    public static bool CompleteChapter(int chapterId)
    {
        if (IsChapterCompleted(chapterId))
        {
            return false;
        }
        completedChapters.Add(chapterId);
        return true;
    }

    public static bool CompleteTask(string task)
    {
        if (IsTaskCompleted(task))
        {
            return false;
        }
        completedTasks.Add(task);
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
}
