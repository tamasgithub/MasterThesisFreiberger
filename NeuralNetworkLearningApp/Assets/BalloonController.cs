using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    public Vector3 stop0;
    public Vector3 stop1;
    public float speed = 1;

    private float progress = 0;
    private int direction = 1;

    // coefficients of the height function, see CalculateHeight(...)
    private float a;
    private float b;
    private float c;
    // for mapping of progress to x of the the height function
    private float distance;
    private float l;

    // Start is called before the first frame update
    void Start()
    {
        stop0.y = Terrain.activeTerrain.SampleHeight(stop0);
        stop1.y = Terrain.activeTerrain.SampleHeight(stop1);
        distance = Vector2.Distance(new Vector2(stop0.x, stop0.z), new Vector2(stop1.x, stop1.z));
        float relHeightDiff = Mathf.Abs(stop0.y - stop1.y) / distance;
        // heightDiff = 0 => r = l
        float smoothing = 2;
        float r = Mathf.Exp(-relHeightDiff/smoothing) / 2f;
        l = r - 1;
        float m = (l + r) / 2f;
        // coefficients of the function h(x) = ax^exponent + c, where
        // h(l) = stop0.y, h(r) = stop1.y, exponent even, under assumption stop0.y <= stop1.y
        a = relHeightDiff / (m*m- r*r);
        b = (relHeightDiff - a * (r * r - l * l)) / (r - l);
        c = -(a * l * l + b * l);
        print("l: " + l);
        print("r: " + r);
        print("a: " + a);
        print("b: " + b);
        print("c: " + c);
        print(CalculateHeight(0));
        print(CalculateHeight(0.5f));
        print(CalculateHeight(1));
    }

    // Update is called once per frame
    void Update()
    {
        progress += direction * speed * Time.deltaTime;
        if (progress < 0 || progress > 1) {
            direction *= -1;
            return;
        }
        float x = Mathf.Lerp(stop0.x, stop1.x, progress);
        float z = Mathf.Lerp(stop0.z, stop1.z, progress);
        float y = CalculateHeight(progress);
        transform.position = new Vector3(x, y, z);
    }

    private float CalculateHeight(float progress)
    {
        // the path through the air has the form f(progress) = a*(progress)^10+c
        // with f(-2/3) = stop0.y and f(1/3) = stop1.y, a < 0
        if (stop0.y > stop1.y)
        {
            progress = 1 - progress;
        }
        float x = progress + l;
        return distance * (a * x * x + b * x + c);


    }
}
