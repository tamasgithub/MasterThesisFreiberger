using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoordinateSystem : MonoBehaviour
{
    public float dataDisplayDuration = 3f;
    public float offset = 0.1f;
    private bool drawingBoundary;
    public DecisionBoundary decisionBoundary;
    private List<PlottableData> data;
    public Color[] colorOfClass;
    // Start is called before the first frame update
    void Start()
    {
        data = new List<PlottableData> ();
    }


    // Update is called once per frame
    void Update()
    {
        if (drawingBoundary)
        {
            decisionBoundary.SetSecondAnchor(ScreenToLocalPoint(Input.mousePosition));
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlottableData data = collider.transform.GetComponent<PlottableData>();
        if (data != null)
        {
            StartCoroutine(DisplayData(data));
        }
    }

    private IEnumerator DisplayData(PlottableData data)
    {
        
        Vector3 startPos = data.transform.position;
        Vector3 endPos = transform.TransformPoint(new Vector3(offset + data.featureValues[0] * 8/10f - 0.5f, offset + data.featureValues[1] * 8 / 10f - 0.5f, data.transform.position.z));
        Vector3 startScale = data.transform.localScale;
        Vector2 endScale = Vector3.one * 0.05f;
        float duration = 0f;
        while (duration < dataDisplayDuration)
        {
            data.transform.localScale = Vector3.Lerp(startScale, endScale, duration / dataDisplayDuration);
            data.transform.localPosition = Vector3.Lerp(startPos, endPos, duration / dataDisplayDuration);
            duration += Time.deltaTime;
            yield return null;
        }
        data.transform.parent = transform;
        data.SwapToPlotSprite(colorOfClass[data.GetDataClass()]);
    }


    public void OnMouseDown()
    { 
        if (!decisionBoundary.gameObject.activeSelf)
        {
            return;
        }
        decisionBoundary.SetFirstAnchor(ScreenToLocalPoint(Input.mousePosition));
        drawingBoundary = true;
    }

    public void OnMouseUp()
    {
        drawingBoundary = false;
    }

    public void HighlightWronglyClassified(float[] coeffs)
    {
        foreach (PlottableData d in data)
        {
            float[] featureValues = d.GetFeatureValues();
            if (featureValues.Length != 2 || coeffs.Length != 3)
            {
                Debug.LogError("Dimension other than 2 not implemented yet");
                return;
            }
            float discriminant = coeffs[0] * featureValues[0] + coeffs[1] * featureValues[1] + coeffs[2];
            if ((discriminant / 2f + 1) != d.GetDataClass())
            {
                HighlightData(d);
            }
        }
    }

    private void HighlightData(PlottableData data)
    {
        Debug.LogError("Not implemented yet.");
    }

    private Vector3 ScreenToLocalPoint(Vector3 screenPoint)
    {
        return transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(screenPoint));
    }

    
}
