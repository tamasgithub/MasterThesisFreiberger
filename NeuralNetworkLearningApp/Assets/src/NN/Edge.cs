using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Edge : MonoBehaviour
{
    public bool isManuallyConnecting;
    public GameObject labelPrefab;
    public Gradient gradient;
    public float weight;

    private Node leftNode;
    private Node rightNode;
    private GameObject tempLabel;
    private bool hoveringEnabled;
    new private SpriteRenderer renderer;
    private void Awake()
    {
        renderer = transform.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        weight = UnityEngine.Random.value * 2 - 1;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isManuallyConnecting)
        {
            Vector2 secondAnchor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (secondAnchor.x < leftNode.transform.position.x)
            {
                UpdatePosition(secondAnchor, leftNode.transform.position);
            }
            else
            {
                UpdatePosition(leftNode.transform.position, secondAnchor);
            }
            return;
        }

        if (leftNode == null || rightNode == null)
        {
            Destroy(gameObject);
        }
        else
        {
            UpdatePosition(leftNode.transform.position, rightNode.transform.position);
            if (transform.parent.parent.parent.GetComponent<NN>().colorEdges)
            {
                renderer.color = gradient.Evaluate(weight);
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    leftNode.RemoveOutgoingEdge(rightNode.GetNodeIndex());
                    rightNode.RemoveIncomingEdge(leftNode.GetNodeIndex());
                    Destroy(gameObject);
                }
            }
            if (tempLabel != null)
            {
                // TODO: this is duplicate logic with the nodes' display of their bias
                tempLabel.transform.position = Input.mousePosition + Vector3.up * 30;
            }
        }
    }

    public float GetWeight()
    {
        return weight;
    }

    public float GetLeftNodeValue()
    {
        return leftNode.GetValue();
    }

    public void SetFirstNode(Node node)
    {
        leftNode = node;
    }

    public void SetSecondNode(Node node)
    {
        // only allow edges between neighboring layers
        if (Mathf.Abs(leftNode.GetLayerIndex() - node.GetLayerIndex()) != 1)
        {
            return;
        }
        if (leftNode.GetLayerIndex() < node.GetLayerIndex())
        {
            rightNode = node;
        } else
        {
            Node temp = leftNode;
            leftNode = node;
            rightNode = temp;
        }
        // duplicate edge (expression should yield true && true or false && false if everything works correctly
        if (leftNode.HasOutgoingEdge(rightNode.GetNodeIndex()) && rightNode.HasIncomingEdge(leftNode.GetNodeIndex()))
        {
            Destroy(gameObject);
            return;
        }
        transform.parent = rightNode.transform;
        GetComponent<BoxCollider2D>().enabled = true;
        // TODO: maybe sort inside the hierarchy
        leftNode.SetOutgoingEdge(this, rightNode.GetNodeIndex());
        rightNode.SetIncomingEdge(this, leftNode.GetNodeIndex());
    }

    public void SetColorEdges(bool colorEdges)
    {
        renderer.color = colorEdges ? gradient.Evaluate(weight) : Color.black;
    }

    public void SetHoveringEnabled(bool hoveringEnabled)
    {
        this.hoveringEnabled = hoveringEnabled;
        if(!hoveringEnabled)
        {
            Destroy(tempLabel);
        }
    }

    private void UpdatePosition(Vector2 leftAnchor, Vector2 rightAnchor)
    {
        // set z to 1 so that nodes are in foreground and take priority for clicks
        transform.position = new Vector3((leftAnchor.x + rightAnchor.x) / 2, (leftAnchor.y + rightAnchor.y) / 2, 1);
        transform.localScale = new Vector3(Vector2.Distance(leftAnchor, rightAnchor), transform.localScale.y, transform.localScale.z);
        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.right, rightAnchor - leftAnchor);
    }

    private void OnMouseEnter()
    {
        if (hoveringEnabled)
        {
            tempLabel = Instantiate(labelPrefab, GameObject.Find("Canvas").transform);
            tempLabel.GetComponent<RectTransform>().position = Input.mousePosition;
            tempLabel.GetComponent<Text>().text = "Weight:\n" + weight.ToString("0.00");
            rightNode.EdgeHovered();
        }   
    }

    private void OnMouseExit()
    {
        Destroy(tempLabel);
    }

    private void OnDestroy()
    {
        if (tempLabel != null)
        {
            Destroy(tempLabel);
        }
    }
}
