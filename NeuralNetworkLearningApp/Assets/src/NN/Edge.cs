using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Edge : MonoBehaviour
{
    public bool isManuallyConnecting;
    public GameObject labelPrefab;
    public Gradient gradient;
    public float weight;

    private Node leftNode;
    private Node rightNode;
    private GameObject hoverLabel;
    private bool hoveringEnabled;
    public GameObject editorPrefab;
    private GameObject editor;
    private bool editingEnabled;
    new private SpriteRenderer renderer;
    private void Awake()
    {
        renderer = transform.GetComponent<SpriteRenderer>();
    }

    public void RandomizeWeight()
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
                renderer.color = gradient.Evaluate((weight + 1) / 2f);
            }
            
            if (Input.GetMouseButtonDown(1) && editingEnabled)
            {
                print("editing edge");
                // right click
                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    print("ray hit");
                    if (hoverLabel != null)
                    {
                        Destroy(hoverLabel);
                    }
                    InputField input;
                    if (editor != null)
                    {
                        input = editor.GetComponentInChildren<InputField>();
                        input.Select();
                        return;
                    }
                    Vector3 position = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 100;
                    editor = Instantiate(editorPrefab, position, Quaternion.identity, GameObject.Find("Canvas").transform);
                    editor.GetComponent<NNEditor>().SetEditedEdge(this);
                    Text label = editor.transform.GetChild(0).GetComponent<Text>();
                    label.text = "Weight:";
                    input = editor.GetComponentInChildren<InputField>();
                    input.Select();
                    input.SetTextWithoutNotify(weight.ToString("0.00"));
                }
            }
            if (hoverLabel != null)
            {
                // TODO: this is duplicate logic with the nodes' display of their bias
                hoverLabel.transform.position = Input.mousePosition + Vector3.up * 80;
            }
        }
    }



        public float GetWeight()
    {
        return weight;
    }

    public void SetWeight(float weight)
    {
        this.weight = weight;
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
        GameObject.Find("AchievementManager").GetComponent<AchievementManager>().IncreaseRequirement(AchievementReqType.MANUAL_CONNECTIONS, 1);
    }

    public void SetColorEdges(bool colorEdges)
    {
        renderer.color = colorEdges ? gradient.Evaluate(weight) : Color.black;
    }

    public void SetHoveringEnabled(bool hoveringEnabled)
    {
        this.hoveringEnabled = hoveringEnabled;
        if(!hoveringEnabled && hoverLabel != null)
        {
            Destroy(hoverLabel);
        }
    }

    public void SetEditingEnabled(bool editingEnabled)
    {
        this.editingEnabled = editingEnabled;
        if (!editingEnabled && editor != null)
        {
            Destroy(editor);
        }
    }

    private void UpdatePosition(Vector2 leftAnchor, Vector2 rightAnchor)
    {
        // set z to 1 so that nodes are in foreground and take priority for clicks
        transform.position = new Vector3((leftAnchor.x + rightAnchor.x) / 2, (leftAnchor.y + rightAnchor.y) / 2, 1);
        transform.localScale = new Vector3(Vector2.Distance(leftAnchor, rightAnchor), transform.localScale.y, transform.localScale.z);

        float angle = Vector2.SignedAngle(Vector2.right, rightAnchor - leftAnchor);
        transform.eulerAngles = Vector3.forward * angle;        
    }

    private void OnMouseEnter()
    {
        if (hoveringEnabled)
        {
            hoverLabel = Instantiate(labelPrefab, GameObject.Find("Canvas").transform);
            hoverLabel.GetComponent<RectTransform>().position = Input.mousePosition;
            hoverLabel.GetComponentInChildren<Text>().text = "Weight:\n" + weight.ToString("0.00");
            rightNode.EdgeHovered();
        }   
    }

    private void OnMouseExit()
    {
        if (hoverLabel != null)
        {
            Destroy(hoverLabel);
        }
        if (editor != null)
        {
            Destroy(editor);
        }
    }

    private void OnDestroy()
    {
        if (hoverLabel != null)
        {
            Destroy(hoverLabel);
        }
        if (editor != null)
        {
            Destroy(editor);
        }
    }
}
