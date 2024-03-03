using System;
using System.Collections.Generic;
using UnityEngine;

public class DataPrefabHolder : MonoBehaviour
{
    public GameObject intDataPrefab;
    public GameObject stringDataPrefab;
    public GameObject charDataPrefab;

    public GameObject GetDataPrefabForType(Type t)
    {
        switch (t.FullName)
        {
            case "System.Int32": return intDataPrefab;
            case "System.String": return stringDataPrefab;
            case "System.Char": return charDataPrefab;
        }
        Debug.LogError("No prefab for specified type " + t.FullName + " stored.");
        return null;
    }
}
