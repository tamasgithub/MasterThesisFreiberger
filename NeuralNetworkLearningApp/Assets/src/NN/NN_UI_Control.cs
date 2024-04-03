using UnityEngine;

public class NN_UI_Control : MonoBehaviour
{
    public NN network;
    public bool nextToLastLayer;
    public int underLayerWithIndex;
    public float offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPos = Vector3.one * -10;
        if (nextToLastLayer)
        {
            worldPos = network.transform.position + Vector3.right * network.GetLayerCount() * network.GetLayerDistance()
                + Vector3.up * offset;
        } else if (underLayerWithIndex < network.GetLayerCount())
        {
            int layerSize = network.GetLayerSize(underLayerWithIndex);
            worldPos = network.transform.position + Vector3.right * underLayerWithIndex * network.GetLayerDistance() 
            + Vector3.down * network.GetNodeDistance() * (layerSize + 1) / 2f + Vector3.right * offset;
        }
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }
}
