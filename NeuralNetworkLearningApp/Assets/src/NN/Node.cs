using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject edge;
    public bool manuallyConnectable = true;
    private Edge manuallyConnectingEdge;
    private List<Edge> incomingEdges = new List<Edge>();
    private List<Edge> outgoingEdges = new List<Edge>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (manuallyConnectable)
        {
            ManuallyConnect();
        }
    }

    private void ManuallyConnect()
    {
        manuallyConnectingEdge = Instantiate(edge, transform).GetComponent<Edge>();
        manuallyConnectingEdge.isManuallyConnecting = true;
        manuallyConnectingEdge.SetFirstNode(this);
    }

    private void OnMouseUp()
    {
        print("Mouse up");
        //RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, -Vector2.up);
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
        if(hit.collider != null)
        {
            manuallyConnectingEdge.SetSecondNode(hit.collider.transform.GetComponent<Node>());
        }
        manuallyConnectingEdge.isManuallyConnecting = false;
        manuallyConnectingEdge = null;
    }
}
