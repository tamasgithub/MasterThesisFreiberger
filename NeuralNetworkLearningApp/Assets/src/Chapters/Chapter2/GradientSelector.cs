using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientSelector : MonoBehaviour
{
    public Transform selector;
    public GameObject stone;
    public int selectedFeatureIndex;
    private bool dragging;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float clampedX = Mathf.Clamp(transform.InverseTransformPoint(mouseWorldPos).x, -0.5f, 0.5f);
            selector.localPosition = new Vector3(clampedX, 0, selector.localPosition.z);
            selector.GetChild(0).GetComponent<TextMesh>().text = (selector.localPosition.x + 0.5f).ToString("0.0");
            stone.GetComponent<PlottableData>().SetFeatureValue(selectedFeatureIndex, selector.localPosition.x + 0.5f);
        }
    }

    private void OnMouseDown()
    {
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }

}
