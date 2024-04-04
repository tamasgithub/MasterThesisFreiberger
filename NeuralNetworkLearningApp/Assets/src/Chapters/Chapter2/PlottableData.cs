using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlottableData : MonoBehaviour
{
    private Transform inputBucket;
    public bool draggable = false;
    private Camera cam;
    private Rigidbody2D rb;
    private bool dragging = false;

    public Sprite spriteInPlot;
    public int dataClass = 0;
    public Gradient gradient;
    [Range(0.0f, 1.0f)]
    public float[] featureValues;
    // -1: no correlation, positive values: correlating to the feature with this index
    public int colorCorrelatingToFeature = 0;
    // -1: no correlation, positive values: correlating to the feature with this index
    public int sizeCorrelatingToFeature = 1;
    public float minSize;
    public float maxSize;
    new private SpriteRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        if (colorCorrelatingToFeature >= 0 && colorCorrelatingToFeature < featureValues.Length)
        {
            renderer.color = gradient.Evaluate(featureValues[colorCorrelatingToFeature]);
        }
        
        if (sizeCorrelatingToFeature >= 0 && sizeCorrelatingToFeature < featureValues.Length)
        {
            transform.localScale = Vector3.one * (minSize + featureValues[sizeCorrelatingToFeature] * (maxSize - minSize));
        }
        if (GameObject.Find("InputBucket") != null)
        {
            inputBucket = GameObject.Find("InputBucket").transform;
        }
        
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            Vector3 newPos = cam.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = -1;
            transform.position = newPos;
        }

        if (transform.position.y < -20)
        {
            if (inputBucket != null)
            {
                transform.position = inputBucket.position;
            }
            GetComponent<Collider2D>().enabled = true;
        }
    }

    public int GetDataClass()
    {
        return dataClass;
    }

    public float[] GetFeatureValues()
    {
        return featureValues;
    }

    public void SetFeatureValue(int featureIndex, float featureValue)
    {
        featureValues[featureIndex] = featureValue;
        if (colorCorrelatingToFeature >= 0 && colorCorrelatingToFeature < featureValues.Length)
        {
            renderer.color = gradient.Evaluate(featureValues[colorCorrelatingToFeature]);
        }

        if (sizeCorrelatingToFeature >= 0 && sizeCorrelatingToFeature < featureValues.Length)
        {
            transform.localScale = Vector3.one * (minSize + featureValues[sizeCorrelatingToFeature] * (maxSize - minSize));
        }
    }

    public void SetDraggable(bool draggable)
    {
        this.draggable = draggable;
    }

    private void OnMouseUp()
    {
        if (dragging)
        {
            GetComponent<Collider2D>().enabled = true;
            EnsureNotInCollision();
            rb.velocity = Vector3.zero;
        }
        dragging = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        draggable = false;
        Destroy(rb);
    }

    private void OnMouseDown()
    {
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = 0;
        if (draggable)
        {
            dragging = true;
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private bool IsInsideInputBucket()
    {
        if (inputBucket == null)
        {
            return false;
        }
        return transform.position.x < inputBucket.GetChild(1).position.x
            && transform.position.x > inputBucket.GetChild(2).position.x
            && transform.position.y > inputBucket.GetChild(0).position.y;
    }

    private void EnsureNotInCollision()
    {   
        // >1 because the own collider is trivially found
        while (Physics2D.OverlapCircleAll(transform.position, transform.lossyScale.x / 2f, 
            LayerMask.NameToLayer("Avoid Collision")).Length > 1)
        {
            transform.Translate(Vector3.up);
        }
    }

    internal void SwapToPlotSprite(Color colorOfClass)
    {
        renderer.sprite = spriteInPlot;
        renderer.color = colorOfClass;
    }

    public void SwapToPlotObject(Color colorOfClass)
    {
        // disable the sprite renderer and enable the child 3D object that is displayed in the 3D coordinate system
        if (transform.childCount == 0)
        {
            //fallback
            SwapToPlotSprite(colorOfClass);
            return;
        }
        renderer.enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetComponent<Renderer>().material.color = colorOfClass;
    }
}

