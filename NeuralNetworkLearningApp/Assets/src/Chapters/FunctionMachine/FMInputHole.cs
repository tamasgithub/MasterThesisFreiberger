using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class FMInputHole : MonoBehaviour
{
    public string inputType;
    private Type type;
    private FMInput inputInHole = null;
    private FunctionMachine functionMachine;

    private void Awake()
    {
        type = Type.GetType(inputType);
        if (type == null)
        {
            Debug.LogError("Input type " + inputType + " could not be parsed. Use the fully qualified name e.g. \"System.Int32\""!);
        }
        functionMachine = transform.parent.GetComponentInChildren<FunctionMachine>();
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
        /*GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponentInChildren<PolygonCollider2D>().isTrigger = true;*/
        inputInHole.GetComponent<Collider2D>().isTrigger = true;
        return inputInHole.GetValue();
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        FMInput collidingInput = collider.transform.GetComponent<FMInput>();
        if (!functionMachine.IsProcessingInputs())
        {
            if (collidingInput.GetInputType() == type) {
                inputInHole = collidingInput;
                collidingInput.AcceptInput();
            }
        } else
        {
            collider.rigidbody.AddForce(Vector2.up*2, ForceMode2D.Impulse);
        }
    }



}
