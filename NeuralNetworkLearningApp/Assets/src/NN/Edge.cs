using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public bool isManuallyConnecting;
    private Node leftNode;
    private Node rightNode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isManuallyConnecting)
        {
            Vector2 rightAnchor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UpdatePosition(leftNode.transform.position, rightAnchor);
            return;
        }
        
        if (leftNode == null || rightNode == null) {
            print("Self destruction");
            Destroy(gameObject);
        } else
        {
            UpdatePosition(leftNode.transform.position, rightNode.transform.position);
        }
    }

    private void UpdatePosition(Vector2 leftAnchor, Vector2 rightAnchor)
    {
        if (rightAnchor.x < leftAnchor.x)
        {
            Vector2 temp = leftAnchor;
            leftAnchor = rightAnchor;
            rightAnchor = temp;
        }

        transform.position = new Vector2((leftAnchor.x + rightAnchor.x) / 2, (leftAnchor.y + rightAnchor.y) / 2);
        transform.localScale = new Vector3(Vector2.Distance(leftAnchor, rightAnchor), transform.localScale.y, transform.localScale.z);
        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.right, rightAnchor - leftAnchor);

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
        // TODO: maybe sort inside the hierarchy
        leftNode.SetOutgoingEdge(this, rightNode.GetNodeIndex());
        rightNode.SetIncomingEdge(this, leftNode.GetNodeIndex());
    }
}
