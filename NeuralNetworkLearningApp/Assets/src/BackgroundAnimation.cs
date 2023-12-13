using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimation : MonoBehaviour
{
    //TODO: set in settings
    Vector2 resolution = new Vector2(1920, 1080);
    //TODO: get from actual menu
    Vector2 menuSize = new Vector2(800, 600);

    int lastSecondDrawn = -1;
    private GameObject startingPointGO;
    private GameObject endingPointGO;
    private GameObject lineGO;

    public GameObject nodePrefab;
    public GameObject linePrefab;

    public Color[] lineColors = new Color[5];

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int sceneSeconds = (int)Time.timeSinceLevelLoad;
        /*print("next frame");
        print(sceneSeconds);
        print(lastSecondDrawn);*/
        if(sceneSeconds > lastSecondDrawn)
        {
            switch (sceneSeconds % 10)
            {
                case 0:
                    DrawStartingNode();
                    break;
                case 1: DrawEndingNode();
                    break;
                case 2: DrawConnectingLine();
                    break;
                case 4: StartColorChange();
                    break;
                case 9: RemoveObjects();
                    break;
            }
            lastSecondDrawn = sceneSeconds;
        }
        
    }

    private void RemoveObjects()
    {
        Destroy(startingPointGO);
        Destroy(endingPointGO);
        Destroy(lineGO);
    }

    private void StartColorChange()
    {
        Gradient gradient = new Gradient();
        Color col0 = lineGO.GetComponent<Image>().color;
        Color col1 = lineColors[Random.Range(0, lineColors.Length)];
        Color col2 = new Color();
        while ((col2 = lineColors[Random.Range(0, lineColors.Length)]) == col1) { };
        Color col3 = new Color();
        while ((col3 = lineColors[Random.Range(0, lineColors.Length)]) == col1 || col3 == col2) { };
        print(col1);
        print(col2);
        print(col3);

        gradient = new Gradient()
        {
            // The number of keys must be specified in this array initialiser
            colorKeys = new GradientColorKey[5] {
                // Add your colour and specify the stop point
                new GradientColorKey(col0, 0),
                new GradientColorKey(col1, 0.25f),
                new GradientColorKey(col2, 0.5f),
                new GradientColorKey(col3, 0.75f),
                new GradientColorKey(col0, 1),
            },
            // This sets the alpha to 1 at both ends of the gradient
            alphaKeys = new GradientAlphaKey[2] {
                new GradientAlphaKey(1, 0),
                new GradientAlphaKey(1, 1)
            }
        };
        StartCoroutine(ColorChange(gradient));
    }

    private IEnumerator ColorChange(Gradient gradient)
    {
        float durationSeconds = 4.5f;
        while (durationSeconds > 0)
        {
            durationSeconds -= Time.deltaTime;
            lineGO.GetComponent<Image>().color = gradient.Evaluate((3.0f - durationSeconds) / 3.0f);
            yield return null;
        }
        yield return null;
    }

    private void DrawConnectingLine()
    {
        lineGO = Instantiate(linePrefab, transform);
        lineGO.transform.SetSiblingIndex(0);
        Vector2 startingPoint = startingPointGO.GetComponent<RectTransform>().anchoredPosition;
        Vector2 endingPoint = endingPointGO.GetComponent<RectTransform>().anchoredPosition;

        float angle = Vector2.SignedAngle(Vector2.right, endingPoint - startingPoint);
        lineGO.transform.Rotate(new Vector3(0, 0, angle));
        StartCoroutine(AnimateLine(startingPoint, endingPoint));
        
    }

    private IEnumerator AnimateLine(Vector2 startingPoint, Vector2 endingPoint)
    {
        float durationSeconds = 3.0f;
        RectTransform lineTransform = lineGO.GetComponent<RectTransform>();
        lineTransform.anchoredPosition = (Vector3)startingPoint + Vector3.forward;
        lineTransform.localScale = new Vector3(0, 1, 1);
        float dist = (startingPoint - endingPoint).magnitude;
        float durationPassed = 0f;
        while (durationPassed < durationSeconds)
        {
            durationPassed += Time.deltaTime;
            lineTransform.anchoredPosition += (endingPoint - startingPoint) * (Time.deltaTime / durationSeconds) / 2;
            // initial length = 100, local scale == dist / 100 means full length
            lineTransform.localScale += new Vector3(dist / 100 * (Time.deltaTime / durationSeconds), 0, 0);
            
            yield return null;
        }
        yield return null;
    }

    private void DrawEndingNode()
    {
        // intialise far to the left
        Vector2 endingPoint = Vector2.zero;
        Vector2 startingPoint = startingPointGO.GetComponent<RectTransform>().anchoredPosition;
        // change to random point not too far left and far enough from the starting point
        while (endingPoint.x < startingPoint.x + 100
            || (startingPoint - endingPoint).magnitude < 200) {
            endingPoint = GetPointOutsideMenu();
        }

        endingPointGO = Instantiate(nodePrefab, transform);
        endingPointGO.GetComponent<RectTransform>().anchoredPosition = new Vector3(endingPoint.x, endingPoint.y, 0);
    }

    private void DrawStartingNode()
    {
        // intialise far to the right
        Vector2 startingPoint = resolution;
        // change to random point not too far right
        while ((startingPoint = GetPointOutsideMenu()).x > resolution.x * 2 / 3) { }

        startingPointGO = Instantiate(nodePrefab, transform);
        startingPointGO.GetComponent<RectTransform>().anchoredPosition = new Vector3(startingPoint.x, startingPoint.y, 0);

    }

    private Vector2 GetPointOutsideMenu()
    {
        Vector2 point = new Vector2(resolution.x / 2, resolution.y / 2);
        while (point.x > (resolution.x - menuSize.x) / 2 && point.x < (resolution.x + menuSize.x) / 2
            && point.y > (resolution.y - menuSize.y) / 2 && point.y < (resolution.y + menuSize.y) / 2) {
            point.x = Random.Range(resolution.x / 10, resolution.x * 9 / 10);
            point.y = Random.Range(resolution.y / 5, resolution.y * 4 / 5);
        }
        return point;
    }
}
