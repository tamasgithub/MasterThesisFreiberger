using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFMInputEncoder : MonoBehaviour
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
        if (Input.GetMouseButtonDown(1))
        {
            char value = (char)input.GetValue();
            

            GameObject intPrefab = dataPrefabHolder.GetDataPrefabForType(typeof(int));
            GameObject intInput = Instantiate(intPrefab, transform.position, Quaternion.identity);
            intInput.GetComponentInChildren<TextMesh>().text = ((int)value - 96).ToString();
            Destroy(gameObject);
        }
    }
}
