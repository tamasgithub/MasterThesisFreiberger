using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class FMInputHole : MonoBehaviour
{
    public string inputType;
    private Type type;
    private FMInput inputInHole = null;

    private void Awake()
    {
        type = Type.GetType(inputType);
        if (type == null)
        {
            Debug.LogError("Input type " + inputType + " could not be parsed. Use the fully qualified name e.g. \"System.Int32\""!);
        }
    }

    public Type GetInputType() { 
        return type;
    }

    public FMInput GetInputInHole()
    {
        return inputInHole;
    }

    public void SetInputInHole(FMInput input)
    {
        inputInHole = input;
    }

    public object LetInputIn()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponentInChildren<PolygonCollider2D>().isTrigger = true;
        return inputInHole.GetValue();
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        FMInput collidingInput = collider.transform.GetComponent<FMInput>();
        if (!GetComponent<BoxCollider2D>().isTrigger)
        {
            inputInHole = collidingInput.GetInputType() == type ? collidingInput : null;
        }
        
        print("collision: inputInHole = " + inputInHole);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // inputs of wrong type and in case of multiple inüuts in one hole,
        // inputs are thrown out of the hole in an upwards direction
        // these inputs should not be disabled
        if (collision.GetComponent<Rigidbody2D>().velocity.y > 0) {
            return;
        }
        collision.gameObject.SetActive(false);
        GetComponent<BoxCollider2D>().isTrigger = false;
        GetComponentInChildren<PolygonCollider2D>().isTrigger = false;
    }


}
