using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateSystem : MonoBehaviour
{
    public float dataDisplayDuration = 3f;
    public float offset = 0.1f;
    private bool draggingBoundary;
    public DecisionBoundary[] decisionBoundaries;
    public GameObject highlightPrefab;
    public List<GameObject> highlights = new List<GameObject>();
    public List<PlottableData> data;
    public Color[] colorOfClass;
    private int numOfClasses;
    private GameObject unseenDataCopy;
    private DecisionBoundary closestToDrag;

    // events the coordinate system fires
    public event Action dataPlottedEvent;
    // this event has as parameter the number of misclassified data points
    public event Action<int> classificationEvent;

    // Start is called before the first frame update
    protected void Start()
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
        numOfClasses = colorOfClass.Length;
    }


    // Update is called once per frame
    private void Update()
    {
        if (draggingBoundary)
        {
            if (numOfClasses <= 2)
            {
                decisionBoundaries[0].SetSecondAnchor(ScreenToSystemPoint(Input.mousePosition));
            }
            else
            {
                closestToDrag.SetSecondAnchor(ScreenToSystemPoint(Input.mousePosition));
            }
        } 

        foreach (PlottableData plottableData in data)
        {
            plottableData.transform.localPosition = SystemToLocalPoint(ArrayAsVector3(plottableData.GetFeatureValues()));
            if (plottableData.GetFeatureValues().Length < 0)
            {
                Vector3 localPosition = plottableData.transform.localPosition;
                localPosition.z = -1;
                plottableData.transform.localPosition = localPosition;
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
        Vector3 endPos = DataToWorldPoint(data);
        Vector3 endScale = Vector3.one * 0.05f;
        data.transform.localScale = endScale;
        data.transform.localPosition = endPos;
        data.transform.parent = transform;
        if (data.featureValues.Length < 3)
        {
            data.SwapToPlotSprite(colorOfClass[data.GetDataClass()]);
        } else if (data.featureValues.Length == 3)
        {
            data.SwapToPlotObject(colorOfClass[data.GetDataClass()]);
        } else
        {
            Debug.LogError("Only up to 3-dimensional data is supported");
        }
        if (dataPlottedEvent != null)
        {
            dataPlottedEvent();
        }
    }

    private IEnumerator DisplayDataWithAnimation(PlottableData data)
    {
        this.data.Add(data);
        Vector3 startPos = data.transform.position;
        Vector3 endPos = DataToWorldPoint(data);
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
        // if dataDisplayDuration was 0
        data.transform.localScale = endScale;
        data.transform.localPosition = endPos;
        data.transform.parent = transform;
        data.SwapToPlotSprite(colorOfClass[data.GetDataClass()]);
        if (dataPlottedEvent != null)
        {
            dataPlottedEvent();
        }
    }

    public int DisplayAndClassifyData(PlottableData dataToDisplay)
    {
        if (unseenDataCopy == null)
        {
            unseenDataCopy = Instantiate(dataToDisplay.gameObject);
            // I don't know why with a smaller local scale, this point is still bigger than those
            // that get dropped onto the coord system
            unseenDataCopy.transform.SetParent(transform);
            Destroy(unseenDataCopy.GetComponent<Rigidbody2D>());
            Destroy(unseenDataCopy.GetComponent<Collider2D>());

        }
        unseenDataCopy.transform.localScale = Vector3.one * 0.02f;
        unseenDataCopy.transform.position = DataToWorldPoint(dataToDisplay);
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
            return -1;
        }
        float discriminant = decisionBoundaryCoeffs[0] * featureValues[0] + decisionBoundaryCoeffs[1] * featureValues[1] + decisionBoundaryCoeffs[2];
        int classifiedAs = discriminant <= 0 ? 0 : 1;
        unseenDataCopy.GetComponent<PlottableData>().SwapToPlotSprite(colorOfClass[classifiedAs]);
        unseenDataCopy.transform.localScale = Vector3.one * 0.02f;
        return classifiedAs;
    }

    private Vector3 DataToWorldPoint(PlottableData data)
    {
        Vector3 localPoint = SystemToLocalPoint(ArrayAsVector3(data.featureValues));
        if (data.featureValues.Length < 3)
        {
            localPoint.z = -1;
        }
        
        return transform.TransformPoint(localPoint);
    }

    protected virtual void OnMouseDown()
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

    protected virtual void OnMouseUp()
    {
        draggingBoundary = false;
        draggingBoundary = false;
    }

    public virtual Vector3 LocalToSystemPoint(Vector3 localPos)
    {
        Vector3 scaledPos = localPos / (1 - 2 * offset);
        Vector3 shiftedPos = scaledPos + 0.5f * Vector3.one;
        return shiftedPos;
    }

    public virtual Vector3 SystemToLocalPoint(Vector3 systemPos)
    {
        Vector3 shiftedPos = systemPos - 0.5f * Vector3.one;
        Vector3 scaledPos = shiftedPos * (1 - 2 * offset);
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
        if (data == null || data.Count == 0)
        {
            return;
        }

        foreach (PlottableData d in data)
        {
            // remark: the implemented decision boundaries between two classes are a simplification for visualization purposes.
            // In practice they would emerge from the difference of two two class boundaries separating one class from all other
            // classes. In that case, classifying using those boundaries also works differently than how this simplification is
            // implemented.
            float[] featureValues = d.GetFeatureValues();
            if (featureValues.Length != 2)
            {
                Debug.LogError("Dimension other than 2 not supported.");
                return;
            }
            Dictionary<int, int> classes = new Dictionary<int, int>();
            for (int i = 0; i < colorOfClass.Length; i ++)
            {
                classes[i] = 0;
            }
            foreach (DecisionBoundary boundary in decisionBoundaries)
            {   
                float[] coeffs = boundary.GetCoefficients();
                if (coeffs.Length != 3)
                {
                    Debug.LogError("Dimension other than 2 not supported.");
                    return;
                }
                float discriminant = coeffs[0] * featureValues[0] + coeffs[1] * featureValues[1] + coeffs[2];
                classes[discriminant < 0 ? boundary.GetSeparatingClasses()[0] : boundary.GetSeparatingClasses()[1]]++;
            }
            int majorityClass = -1;
            int majorityCount = 0;
            foreach (int dataClass in classes.Keys)
            {
                if (classes[dataClass] > majorityCount)
                {
                    majorityCount = classes[dataClass];
                    majorityClass = dataClass;
                }
            }
            if (majorityClass == -1 || majorityCount == 0)
            {
                HighlightData(d);
                Debug.LogError("Failed to classify");
                continue;
            }
            
            if (majorityClass != d.GetDataClass())
            {
                HighlightData(d);
            }
        }
        if (classificationEvent != null)
        {
            print("classification event fired with " + highlights.Count);
            classificationEvent(highlights.Count);
        } else
        {
            print("classification event is null");
        }
    }



    private void HighlightData(PlottableData data)
    {
        GameObject newHighlight = Instantiate(highlightPrefab, transform);
        highlights.Add(newHighlight);
        newHighlight.transform.position = DataToWorldPoint(data);
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
            float distance;
            // check for "wrong" side of the line
            if (Vector2.Dot(boundary.GetDirectionVector(), systemPos - boundary.GetFirstAnchor()) < 0)
            {
                distance = Vector2.Distance(boundary.GetFirstAnchor() + boundary.GetDirectionVector(), systemPos);
            } else {

                float[] coefficients = boundary.GetCoefficients();
                if (coefficients.Length != 3)
                {
                    Debug.LogError("Dimension other than 2 not supported.");
                    return;
                }
                // the coefficients should already be normalized but doesn't hurt to make sure
                float normalizationFactor = new Vector2(coefficients[0], coefficients[1]).magnitude;
                Vector3 coefficientVector = ArrayAsVector3(coefficients);
                distance = Mathf.Abs((Vector3.Dot(coefficientVector, systemPos) + coefficients[coefficients.Length - 1]) / normalizationFactor);
            }
            if (distance < closestDistance)
            {
                closest = boundary;
                closestDistance = distance;
            }
        }
        closestToDrag = closest;
    }

    protected Vector3 ArrayAsVector3(float[] array)
    {
        if (array.Length > 3)
        {
            Debug.LogError("Array is too large, cannot fit " + array.Length + " entries into a Vector3");
        }
        Vector3 result = Vector3.zero;
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = array[i];
        }
        return result;
    }

    protected Vector2 ArrayAsVector2(float[] array)
    {
        if (array.Length > 2)
        {
            Debug.LogError("Array is too large, cannot fit " + array.Length + " entries into a Vector2");
        }
        Vector2 result = Vector2.zero;
        for (int i = 0; i < array.Length; i++)
        {
            result[i] = array[i];
        }
        return result;
    }
}
