using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    int size = 0;
    float nodeDistance = 1.0f;
    bool fullyConnected = false;
    public GameObject node;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: fancy c# property instead
    public int GetSize()
    {
        return size;
    }
    public void SetSize(int i)
    {
        if (i < size)
        {
            for (int j = size - 1; j >= i; j--)
            {
                // destroy last child of layer i
                Destroy(transform.GetChild(j).gameObject);
                size--;
            }
        }
        else if (i > size)
        {
            for (int j = size; j < i; j++)
            {
                GameObject newNode = Instantiate(node, transform);
                newNode.name = "node" + j;
                size++;
                if (fullyConnected)
                {
                    //TODO: newNode.GetComponent<Node>().fullyConnect();
                }
            }
        }

        for (int j = 0; j < size; j++)
        {
            PositionNode(j);
        }
    }

    private void PositionNode(int i)
    {
        transform.Find("node" + i).localPosition = Vector2.down * nodeDistance * (i - (size - 1) / 2.0f);
    }

    public void SetNodeDistance(float nodeDistance)
    {
        if (this.nodeDistance == nodeDistance)
        {
            return;
        }
        this.nodeDistance = nodeDistance;
        for (int i = 0; i < size; i++)
        {
            PositionNode(i);
        }
    }
}
