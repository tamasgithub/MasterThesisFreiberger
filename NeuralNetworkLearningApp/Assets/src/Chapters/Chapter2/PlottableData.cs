using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlottableData : MonoBehaviour
{
    private Transform inputBucket;
    private bool draggable = true;
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
    private SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        if (colorCorrelatingToFeature >= 0 && colorCorrelatingToFeature < featureValues.Length)
        {
            renderer.color = gradient.Evaluate(featureValues[colorCorrelatingToFeature]);
        }
        
        if (sizeCorrelatingToFeature >= 0 && sizeCorrelatingToFeature < featureValues.Length)
        {
            transform.localScale = Vector3.one * (minSize + featureValues[sizeCorrelatingToFeature] * (maxSize - minSize));
        }

        inputBucket = GameObject.Find("InputBucket").transform;
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

        draggable = true;
        if (IsInsideInputBucket())
        {
            draggable = true;
        }

        if (transform.position.y < -20)
        {
            transform.position = inputBucket.position;
            draggable = true;
            GetComponent<Collider2D>().enabled = true;
        }

        if (dragging)
        {
            Vector3 newPos = cam.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = -1;
            transform.position = newPos;
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
}

