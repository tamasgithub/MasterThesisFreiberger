using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateSystem : MonoBehaviour
{
    public List<GameObject> datapointPrefabs;
    private int[] offsetPixels = new int[] { 25, 25 };
    private int pixelsPerUnit = 50;

    // Start is called before the first frame update
    void Start()
    {
        // putting the datapoints manually into the scene for now
        /*DisplayDataPoints(0, new float[,] {
            { 1f, 2f },
            { 3f, 2.5f }

        });*/
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayDataPoints(int dataClass, float[,] coordinates)
    {
        if (dataClass >= datapointPrefabs.Count)
        {
            return;
        }

        for (int row = 0; row < coordinates.GetLength(0); row++)
        {
            GameObject newDataPoint = Instantiate(datapointPrefabs[dataClass], transform);
            newDataPoint.GetComponent<RectTransform>().anchoredPosition = 
                new Vector2(coordinates[row, 0] * pixelsPerUnit + offsetPixels[0], coordinates[row, 1] * pixelsPerUnit + offsetPixels[1]);
        }
    }
}
