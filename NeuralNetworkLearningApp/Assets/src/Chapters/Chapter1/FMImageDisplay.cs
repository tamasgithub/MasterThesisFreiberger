using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FMImageDisplay : MonoBehaviour
{
    // used for both input and output holes
    public Sprite fmHole;
    public NN network;
    // Start is called before the first frame update
    void Start()
    {
        int layerCount = network.GetLayerCount();
        int inputs = network.GetLayerSize(0);
        int outputs = network.GetLayerSize(layerCount - 1);
        float nodeDist = network.GetNodeDistance();
        float layerDist = network.GetLayerDistance();
        

        for (int i = 0; i < inputs; i++)
        {
            GameObject hole = new GameObject("inputHole" + i);
            
            hole.AddComponent<SpriteRenderer>().sprite = fmHole;
            Vector3 position = network.transform.position;
            position.z = 5;
            position.y = (- (inputs - 1) / 2f + i) * nodeDist + transform.position.y;
            hole.transform.position = position;
            hole.transform.eulerAngles = new Vector3(0, 0, 90);
            hole.transform.localScale = new Vector3(0.2f, 0.25f, 1f);
            hole.transform.parent = transform;
        }

        for (int i = 0; i < outputs; i++)
        {
            GameObject hole = new GameObject("inputHole" + i);
            
            hole.AddComponent<SpriteRenderer>().sprite = fmHole;
            Vector3 position = network.transform.position;
            position.z = 5;
            position.y = (-(outputs - 1) / 2f + i) * nodeDist + transform.position.y;
            position.x = (layerCount - 1) / 2f * layerDist;
            hole.transform.position = position;
            hole.transform.eulerAngles = new Vector3(0, 0, -90);
            hole.transform.localScale = new Vector3(0.2f, 0.25f, 1f);
            hole.transform.parent = transform;
        }
    }



}
