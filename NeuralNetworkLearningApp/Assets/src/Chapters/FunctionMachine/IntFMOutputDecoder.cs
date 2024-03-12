using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntFMOutputDecoder : MonoBehaviour
{
    public event Action decodedEvent;

    private DataPrefabHolder dataPrefabHolder;
    private FMInput input;
    private void Start()
    {
        dataPrefabHolder = FindObjectOfType<DataPrefabHolder>();
        input = GetComponent<FMInput>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int value = (int)input.GetValue();
            

            GameObject charPrefab = dataPrefabHolder.GetDataPrefabForType(typeof(char));
            GameObject charInput = Instantiate(charPrefab, transform.position, Quaternion.identity);
            charInput.GetComponentInChildren<TextMesh>().text = ((char)(value + 96)).ToString();
            if (decodedEvent != null)
            {
                decodedEvent();
            }
            Destroy(gameObject);
        }
    }
}
