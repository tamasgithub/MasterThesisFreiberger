using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FMInput : MonoBehaviour
{
    public Transform[] functionMachines;
    private Transform inputBucket;
    public string inputType;
    private Type type;
    private object value;
    private bool isPickupable = true;
    private Camera cam;
    private Rigidbody2D rb;
    private bool dragging = false;
    private bool inputAccepted;

    // Start is called before the first frame update
    void Start()
    {
        inputBucket = GameObject.Find("InputBucket").transform;
        type = Type.GetType(inputType);
        if (type == null )
        {
            Debug.LogError("Input type " + inputType + "could not be parsed. Use the fully qualified name e.g. \"System.Int32\""!);
        }
        string stringValue = GetComponentInChildren<TextMesh>().text;
        value = Convert.ChangeType(stringValue, type);
        if (value.GetType() != type)
        {
            Debug.LogError("Converting the value " + stringValue + " to the specified type " + type.FullName + " failed. Make sure the" +
                " value can be converted to the type using \"Convert.ChangeType(value, type);\"");
        }

        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        isPickupable = true;
        if (IsInsideInputBucket())
        {
            isPickupable = true;
        }

        if (transform.position.y < -20)
        {
            transform.position = inputBucket.position;
            isPickupable = true;
            GetComponent<CircleCollider2D>().enabled = true;
        }


        if (dragging) {
            Vector3 newPos = cam.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = -1;
            transform.position = newPos;
        } else if (!isPickupable)
        {
            //i.e. is falling somewhere outside of the bucket

        }
    }

    public Type GetInputType()
    {
        return type;
    }

    public object GetValue()
    {
        return value;
    }

    public void SetValue(string stringValue)
    {
        value = Convert.ChangeType(stringValue, type);
        GetComponentInChildren<TextMesh>().text = stringValue;
    }

    private void OnMouseUp()
    {
        print("mouse up");
        if (dragging)
        {
            GetComponent<CircleCollider2D>().enabled = true;
            EnsureNotInCollision();
            rb.velocity = Vector3.zero;
        }
        dragging = false;
    }

    private void OnMouseDown()
    {
        print("mouse down");
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = 0;
        if (isPickupable)
        {            
            dragging = true;
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    private bool IsInsideInputBucket()
    {
        return transform.position.x < inputBucket.GetChild(1).position.x
            && transform.position.x > inputBucket.GetChild(2).position.x
            && transform.position.y > inputBucket.GetChild(0).position.y;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        transform.Translate(Vector3.forward * 3);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (inputAccepted)
        {
            Destroy(gameObject);
        } 
    }

    public void AcceptInput()
    {
        inputAccepted = true;
        isPickupable = false;
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        FMInputHole inputHole = collision.transform.GetComponent<FMInputHole>();
        if (inputHole == null)
        {
            return;
        }
        if (inputHole.GetInputType() == type)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            
        }
    }*/

    public void SetInForegroundAfter(float seconds)
    {
        StartCoroutine(SetInForeground(seconds));
    }

    IEnumerator SetInForeground(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        transform.Translate(Vector3.back * 5);
    }

    private void EnsureNotInCollision()
    {
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        // >1 because the own collider is trivially found
        while (Physics2D.OverlapCircleAll(transform.position, transform.lossyScale.x / 2f).Length > 1)
        {
            transform.Translate(Vector3.up);
        }
    }

}
