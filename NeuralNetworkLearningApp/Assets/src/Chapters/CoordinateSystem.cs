using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class CoordinateSystem : MonoBehaviour
{
    public float dataDisplayDuration = 3f;
    public float offset = 0.1f;
    private bool draggingBoundary;
    public DecisionBoundary[] decisionBoundaries;
    public GameObject highlightPrefab;
    public List<GameObject> highlights;
    public List<PlottableData> data;
    public Color[] colorOfClass;
    private int numOfClasses;
    private GameObject unseenDataCopy;
    private DecisionBoundary closestToDrag;
    // Start is called before the first frame update
    void Start()
    {
        if (data == null)
        {
            data = new List<PlottableData>();
        } else if (data.Count > 0)
        {
            dataDisplayDuration = 0;
            foreach (PlottableData plottableData in data)
            {
                DisplayData(plottableData);
                Destroy(plottableData.GetComponent<Rigidbody2D>());
                plottableData.GetComponent<Collider2D>().enabled = false;
                plottableData.GetComponent<PlottableData>().enabled = false;
            }
        } 
        highlights = new List<GameObject> ();
        numOfClasses = colorOfClass.Length;
    }


    // Update is called once per frame
    void Update()
    {
        if (draggingBoundary)
        {
            if (numOfClasses <= 2)
            {
                decisionBoundaries[0].SetSecondAnchor(ScreenToSystemPoint(Input.mousePosition), false);
            }
            else
            {
                closestToDrag.SetSecondAnchor(ScreenToSystemPoint(Input.mousePosition), true);
            }
        } 
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlottableData data = collider.transform.GetComponent<PlottableData>();
        if (data != null)
        {
            StartCoroutine(DisplayDataWithAnimation(data));
        }
    }
    private void DisplayData(PlottableData data)
    {
        Vector3 endPos = CalculateDataPointWorldPos(data);
        Vector2 endScale = Vector3.one * 0.05f;
        data.transform.localScale = endScale;
        data.transform.localPosition = endPos;
        data.transform.parent = transform;
        data.SwapToPlotSprite(colorOfClass[data.GetDataClass()]);
    }
    private IEnumerator DisplayDataWithAnimation(PlottableData data)
    {
        this.data.Add(data);
        Vector3 startPos = data.transform.position;
        Vector3 endPos = CalculateDataPointWorldPos(data);
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

    public void DisplayAndClassifyData(PlottableData dataToDisplay)
    {
        if (unseenDataCopy == null)
        {
            unseenDataCopy = Instantiate(dataToDisplay.gameObject);
            // I don't know why with a smaller local scale, this point is still bigger than those
            // that get dropped onto the coord system
            unseenDataCopy.transform.parent = transform;
            
        }
        unseenDataCopy.transform.localScale = Vector3.one * 0.02f;
        unseenDataCopy.transform.position = CalculateDataPointWorldPos(dataToDisplay);
        if (numOfClasses > 2)
        {
            Debug.LogError("This method was not modified for more than 2 classes. The classification will not be correct.");
        }
        // TODO: classify using multiple decision boundaries if there are more than 2 classes
        float[] decisionBoundaryCoeffs = decisionBoundaries[0].GetCoefficients();
        float[] featureValues = dataToDisplay.GetFeatureValues();
        if (featureValues.Length != 2 || decisionBoundaryCoeffs.Length != 3)
        {
            Debug.LogError("Dimension other than 2 not supported.");
            return;
        }
        float discriminant = decisionBoundaryCoeffs[0] * featureValues[0] + decisionBoundaryCoeffs[1] * featureValues[1] + decisionBoundaryCoeffs[2];
        int classifiedAs = discriminant <= 0 ? 0 : 1;
        unseenDataCopy.GetComponent<PlottableData>().SwapToPlotSprite(colorOfClass[classifiedAs]);

    }

    private Vector3 CalculateDataPointWorldPos(PlottableData data)
    {
        float localX = offset + data.featureValues[0] * 8 / 10f - 0.5f;
        float localY = offset + data.featureValues[1] * 8 / 10f - 0.5f;
        Vector3 localPos = new Vector3(localX, localY, -1);
        return transform.TransformPoint(localPos);
    }

    public void OnMouseDown()
    { 
        // assuming that all decision boundaries are active or inactive at the same time
        if (!decisionBoundaries[0].gameObject.activeSelf)
        {
            return;
        }
        Vector3 systemPoint = ScreenToSystemPoint(Input.mousePosition);
        // should never be <2, unless wrongly initialized
        if (numOfClasses <= 2) {
            decisionBoundaries[0].SetFirstAnchor(systemPoint);
            draggingBoundary = true;
        } else
        {
            FindClosestDecisionBoundary(systemPoint);
            draggingBoundary = true;
        }
        
        
    }

    public void OnMouseUp()
    {
        draggingBoundary = false;
        draggingBoundary = false;
    }

    public Vector2 LocalToSystemPoint(Vector3 localPos)
    {
        Vector2 scaledPos = localPos / (1 - 2 * offset);
        Vector2 shiftedPos = scaledPos + 0.5f * Vector2.one;
        return shiftedPos;
    }

    public Vector3 SystemToLocalPoint(Vector2 systemPos)
    {
        Vector2 shiftedPos = systemPos - 0.5f * Vector2.one;
        Vector2 scaledPos = shiftedPos * (1 - 2 * offset);
        return scaledPos;
    }

    public Vector2 ScreenToSystemPoint(Vector2 screenPos)
    {
        return LocalToSystemPoint(ScreenToLocalPoint(screenPos));
    }

    public void HighlightWronglyClassified()
    {
        foreach(GameObject highlight in highlights)
        {
            Destroy(highlight);
        }
        highlights.Clear();
        if (numOfClasses > 2)
        {
            Debug.LogError("This method was not adjusted for more than 2 classes. The higlighting will be incorrect!");
        }
        //TODO: implement for more than 2 classes
        float[] coeffs = decisionBoundaries[0].GetCoefficients();
        if (data == null)
        {
            return;
        }
        foreach (PlottableData d in data)
        {
            float[] featureValues = d.GetFeatureValues();
            if (featureValues.Length != 2 || coeffs.Length != 3)
            {
                Debug.LogError("Dimension other than 2 not supported.");
                return;
            }
            float discriminant = coeffs[0] * featureValues[0] + coeffs[1] * featureValues[1] + coeffs[2];
            if ((discriminant >= 0 ? 1 : 0) != d.GetDataClass())
            {
                HighlightData(d);
            }
        }
    }

    private void HighlightData(PlottableData data)
    {
        GameObject newHighlight = Instantiate(highlightPrefab, transform);
        highlights.Add(newHighlight);
        newHighlight.transform.position = CalculateDataPointWorldPos(data);
        newHighlight.transform.SetParent(transform);
    }

    private Vector3 ScreenToLocalPoint(Vector3 screenPoint)
    {
        return transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(screenPoint));
    }

    private void FindClosestDecisionBoundary(Vector2 systemPos)
    {
        float closestDistance = float.PositiveInfinity;
        DecisionBoundary closest = null;
        foreach (DecisionBoundary boundary in decisionBoundaries)
        {
            float[] coefficients = boundary.GetCoefficients();
            if (coefficients.Length != 3)
            {
                Debug.LogError("Dimension other than 2 not supported.");
                return;
            }
            // the coefficients should already be normalized but doesn't hurt to make sure
            float normalizationFactor = new Vector2(coefficients[0], coefficients[1]).magnitude;
            float distance = Mathf.Abs((coefficients[0] * systemPos.x + coefficients[1] * systemPos.y + coefficients[2]) / normalizationFactor);
            if (distance < closestDistance)
            {
                closest = boundary;
                closestDistance = distance;
            }
        }
        closestToDrag = closest;
    }


}
