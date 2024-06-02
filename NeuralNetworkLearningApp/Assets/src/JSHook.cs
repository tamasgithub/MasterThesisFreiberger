using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class JSHook
{

    [DllImport("__Internal")]
    public static extern void SetCookie(string cookie);
}
