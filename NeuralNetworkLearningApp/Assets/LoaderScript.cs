using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class LoaderScript : MonoBehaviour
{

    [SerializeField]
    private Text text;

    // // persistance for WegGL builds in form of cookies (on load is before this script's Start)
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
        string result = "progress cookie not found";
        cookies.TryGetValue("progress", out result);
        text.text = result;
    }

    private void Start()
    {
        print("progress string: " + Progress.GetProgressAsString());
        JSHook.SetCookie("progress="+ Progress.GetProgressAsString() + ";SameSite=None");
    }
}