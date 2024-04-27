using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

// this is not a typo for "Handler", this class describes the behavoiur of the handle that turns an FM's gears
public class FMHandle : MonoBehaviour
{
    private bool rotating;
    private FunctionMachine machine;
    private Transform gear;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        gear = transform.parent.parent;
        machine = gear.parent.GetComponent<FunctionMachine>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Collider2D>().enabled = !machine.AreGearsTurning();
     
        if (rotating)
        {
            Vector2 mouseWorldPos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
            //print(worldPos);
            float angleToRotate = Vector2.SignedAngle(mouseWorldPos - (Vector2)gear.position,
                (Vector2)transform.position - (Vector2)gear.position);
            if (angleToRotate > 0 && angleToRotate < 30) {
                machine.RotateGears(angleToRotate);
            }
            
        }
    }

    void OnMouseDown()
    {
        rotating = true;
    }

    void OnMouseUp()
    {
        rotating = false;
    }
}
