using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class FMInputHole : MonoBehaviour
{
    public string inputType;
    public event Action inputRejectedEvent;

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
        inputInHole.AcceptInput();
        return inputInHole.GetValue();
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        FMInput collidingInput = collider.transform.GetComponent<FMInput>();
        if (!functionMachine.IsProcessingInputs())
        {
            if (collidingInput.GetInputType() == type) {
                inputInHole = collidingInput;
                collider.rigidbody.velocity = Vector2.zero;
                if (type == typeof(string))
                {
                    float zRotation = collider.transform.rotation.eulerAngles.z;
                    collider.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 
                        Mathf.Sign(180 - zRotation) *  90));
                    collider.rigidbody.angularVelocity = 0;
                }
            } else
            {
                collider.rigidbody.AddForce(Vector3.up * 6
                + (UnityEngine.Random.value > 0.5f ? Vector3.right : Vector3.right) * 0.3f, ForceMode2D.Impulse);
                collider.transform.GetComponent<CircleCollider2D>().enabled = false;
                collidingInput.SetInForegroundAfter(1);

                if (inputRejectedEvent != null)
                {
                    print("fire input rejected event");
                    inputRejectedEvent();
                } else
                {
                    print("event has no subscribers");
                }
                    
            }
        } else
        {
            collider.rigidbody.AddForce(Vector2.up*2, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        SetInputInHole(null);
    }

}
