using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientSelector : MonoBehaviour
{
    public Transform selector;
    public PlottableData stone;
    public int selectedFeatureIndex;
    private bool dragging;
    public CoordinateSystem coordinateSystem;
    // this event has as parameter the class as which the single data point that caused the classification was classified
    public Action<int> classificationEvent;

    // Start is called before the first frame update
    void Start()
    {
        coordinateSystem.DisplayAndClassifyData(stone);
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
            stone.SetFeatureValue(selectedFeatureIndex, selector.localPosition.x + 0.5f);
            

        }
        // should not be neccessary to every frame, only when adjusting
        // but for some reason the scale is not updated when first called in Start, so
        // that the stone is huge until first adjustment has been done
        int classifiedAs = coordinateSystem.DisplayAndClassifyData(stone);
        if (classificationEvent != null)
        {
            classificationEvent(classifiedAs);
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
