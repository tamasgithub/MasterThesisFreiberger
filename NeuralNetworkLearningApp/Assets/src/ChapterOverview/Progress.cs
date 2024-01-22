using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Progress
{
    private static List<int> completedChapters = new List<int>();

    // Start is called before the first frame update
    public static bool CompleteChapter(int chapterId)
    {
        if (completedChapters.Contains(chapterId))
        {
            return false;
        }
        completedChapters.Add(chapterId);
        return true;
    }

    public static int GetCompletedChaptersCount()
    {
        return completedChapters.Count;
    }
}
