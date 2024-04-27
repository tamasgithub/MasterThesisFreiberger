using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StringFMInputSplitter : MonoBehaviour
{
    private DataPrefabHolder dataPrefabHolder;
    private FMInput input;
    private void Start()
    {
        dataPrefabHolder = FindObjectOfType<DataPrefabHolder>();
        input = GetComponent<FMInput>();
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1)){
            string value = (string)input.GetValue();
            Type remainingType = value.Length > 2 ? typeof(string) : typeof(char);
            Destroy(gameObject);
            GameObject firstCharPrefab = dataPrefabHolder.GetDataPrefabForType(typeof(char)); 
            GameObject firstChar = Instantiate(firstCharPrefab, transform.position + Vector3.left * 0.1f, Quaternion.identity);
            
            firstChar.GetComponentInChildren<TextMesh>().text = value[0].ToString();
            firstChar.AddComponent<CharFMInputEncoder>();
            GameObject remainingPrefab = dataPrefabHolder.GetDataPrefabForType(remainingType);
            GameObject remainder = Instantiate(remainingPrefab, transform.position + Vector3.right * 0.1f, Quaternion.identity);
            remainder.GetComponentInChildren<TextMesh>().text = value.Substring(1);
            if (remainingType == typeof(string)) {
                remainder.AddComponent<StringFMInputSplitter>();
                AdjustRemainderScale(value, remainder);
            } else
            {
                remainder.AddComponent<CharFMInputEncoder>();
            }
            Destroy(gameObject);

        }
    }

    private void AdjustRemainderScale(string valueBeforeSplit, GameObject remainder)
    {
        float retractingFactor = (valueBeforeSplit.Length - 1) / (float)valueBeforeSplit.Length;
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Max(1, localScale.x * retractingFactor);
        remainder.transform.localScale = localScale;
        remainder.GetComponent<CircleCollider2D>().radius = 0.5f / localScale.x;
        Vector3 textLocalScale = transform.GetChild(0).transform.localScale;
        textLocalScale.x = Mathf.Min(.1f, textLocalScale.x / retractingFactor);
        remainder.transform.GetChild(0).transform.localScale = textLocalScale;
    }
}
