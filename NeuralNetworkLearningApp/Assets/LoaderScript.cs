using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class LoaderScript : MonoBehaviour
{
    [SerializeField]
    private TextAsset questionnaireResults;
    private string path = @"C:\Users\Tamás Freiberger\Desktop\results.txt";


    // persistance for WegGL builds in form of cookies (on load is before this script's Start)
    public void CookiesReadOnLoad(string allCookies)
    {
        // CAUTION!!! this input could be manipulated by users! Be careful when using in SQL queries etc.!
        string[] cookiesArray = allCookies.Split(';');
        Dictionary<string, string> cookies = new Dictionary<string, string>();
        for (int i = 0; i < cookiesArray.Length; i++)
        {
            if (cookiesArray[i].Count(x => x == '=') != 1) continue;

            string key = cookiesArray[i].Split("=")[0].Trim();
            string value = cookiesArray[i].Split("=")[1].Trim();
            cookies[key] = value;
        }
        string progress;
        cookies.TryGetValue("progress", out progress);
        Progress.LoadProgressFromString(progress);
        string achievements;
        cookies.TryGetValue("achievements", out achievements);
        AchievementManager.LoadAchievementsFromString(achievements);
    }

    public void SaveQuestionnaireResult(string result)
    {
        try
        {
            Debug.Log("Recieved result: " + result);
            // Write the text to the file
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(result);
                writer.Close();
            }

            Debug.Log("Result stored succesfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error writing to file: " + e.Message);
        }
    }
}
