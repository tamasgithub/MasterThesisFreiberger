using UnityEngine;

public class NN_UI_Control : MonoBehaviour
{
    private NN network;
    public bool nextToLastLayer;
    public int underLayerWithIndex;
    public float offset;
    // add means the counterpart is remove, not add means remove and the counterpart is add
    public bool add;
    private NN_UI_Control counterpart;
    public int maxLayerCount;
    public int minLayerCount;
    public int maxLayerSize;
    public int minLayerSize;

    // Start is called before the first frame update
    void Start()
    {
        counterpart = transform.parent.GetChild(1 - transform.GetSiblingIndex()).GetComponent<NN_UI_Control>();
        network = FindObjectOfType<NN>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldPos = Vector3.one * -10;
        if (nextToLastLayer)
        {
            // like this instead of setActive(---condition---) bc GO can't reactivate itself, Update isn't called once deactivated
            // -> counterpart activates counterpart, never both inactive (except when min == max, then no need for any buttons)
            if (network.GetLayerCount() == maxLayerCount && add || network.GetLayerCount() == minLayerCount && !add) {
                gameObject.SetActive(false);
            }
            worldPos = network.transform.position + Vector3.right * network.GetLayerCount() * network.GetLayerDistance()
                + Vector3.up * offset;
        } else if (underLayerWithIndex < network.GetLayerCount())
        {
            int layerSize = network.GetLayerSize(underLayerWithIndex);
            if (layerSize == maxLayerSize && add || layerSize == minLayerSize && !add)
            {   
                gameObject.SetActive(false);
            }
            worldPos = network.transform.position + Vector3.right * underLayerWithIndex * network.GetLayerDistance() 
                + Vector3.down * network.GetNodeDistance() * (layerSize + 1) / 2f + Vector3.right * offset;
        }
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }

    public void OnClick()
    {
        if (nextToLastLayer)
        {
            if (add)
            {
                network.IncreaseLayerCount();
                if (network.GetLayerCount() > minLayerCount)
                {
                    counterpart.gameObject.SetActive(true);
                }
            } else {
                network.DecreaseLayerCount();
                if (network.GetLayerCount() < maxLayerCount)
                {
                    counterpart.gameObject.SetActive(true);
                }
            }
        } else
        {
            if (add)
            {
                network.IncreaseLayerSize(underLayerWithIndex);
                if (network.GetLayerSize(underLayerWithIndex) > minLayerSize)
                {
                    counterpart.gameObject.SetActive(true);
                }
            } else
            {
                network.DecreaseLayerSize(underLayerWithIndex);
                if (network.GetLayerSize(underLayerWithIndex) < maxLayerSize)
                {
                    counterpart.gameObject.SetActive(true);
                }
            }
        }
    }
}
