using System;
using System.Collections;
using UnityEngine;

public class FunctionMachine : MonoBehaviour
{
    public bool gearsTurning = true;
    public Transform[] gears = new Transform[3];

    private FMInputHole[] inputHoles;
    public GameObject[] outputHoles;
    public GameObject[] outputDataPrefabs;
    public GameObject intDataPrefab;

    public Function function;
    private float gearsRotatedThisFrame;
    
    // Start is called before the first frame update
    void Start()
    {
        inputHoles = transform.parent.GetComponentsInChildren<FMInputHole>();
        // possibly should allow input holes to be in any order inside the parent somehow
        Type[] functionInputTypes = FunctionDetails.GetFunctionInputTypes(function);
        bool inputTypesCorrect = functionInputTypes.Length == inputHoles.Length;
        for (int i = 0; i < inputHoles.Length; i++)
        {
            inputTypesCorrect &= inputHoles[i].GetInputType() == functionInputTypes[i];

        }
        if (!inputTypesCorrect)
        {
            Debug.LogError("The input types of the input holes and the" +
                "input types of the performing function don't match!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (gearsTurning)
        {
            RotateGears(Time.deltaTime * 100);
        }

        if (AllInputsFilled())
        {
            ProcessInputs();
        }
    }

    private void LateUpdate()
    {
        gearsRotatedThisFrame = 0;
    }

    public void RotateGears(float angle)
    {

        for (int i = 0; i < gears.Length; i++)
        {
            gears[i].Rotate(new Vector3(0, 0, angle * 2 * (i % 2 - 0.5f)));
        }
        gearsRotatedThisFrame = angle;
        
        print("gears rot frame: " + gearsRotatedThisFrame);
    }


    private bool AllInputsFilled()
    {
        foreach (FMInputHole inputHole in inputHoles)
        {
            if (inputHole.GetInputInHole() == null)
            {
                return false;
            }
        }
        return true;
    }

    private void ProcessInputs()
    {
        object[] inputValues = new object[inputHoles.Length];
        for (int i = 0; i < inputHoles.Length; i++)
        {
            inputValues[i] = inputHoles[i].LetInputIn();
        }
        object[] outputValues = FunctionDetails.GetImplementation(function)(inputValues);
        StartCoroutine(CreateOutputs(outputValues));
        foreach (FMInputHole inputHole in inputHoles)
        {
            inputHole.SetInputInHole(null);
        }
    }

    IEnumerator CreateOutputs(object[] outputValues)
    {
        float gearsRotatedTotal = 0;
        while (gearsRotatedTotal < 360f)
        {
            gearsRotatedTotal += gearsRotatedThisFrame;
            yield return null;
        }
        for (int i = 0; i < outputValues.Length; i++)
        {
            /*Type outputType = FunctionDetails.GetFunctionOutputTypes(function)[i];
            GameObject outputPrefab = null;
            foreach(GameObject dataPrefab in outputDataPrefabs) {
                if (GetComponent<FMInput>().GetInputType() == outputType)
                {
                    outputPrefab = dataPrefab;
                    break;
                }
            }
            if (outputPrefab == null) {
                Debug.LogError("No output prefab for the output type " + outputType 
                    + " of the function " + function + " was provided!");
                return;
            }*/
            GameObject resultData = Instantiate(intDataPrefab, outputHoles[i].transform.position, Quaternion.identity);
            resultData.transform.Translate(Vector3.forward * 2);
            resultData.GetComponentInChildren<TextMesh>().text = outputValues[i].ToString();
            print("popped out an output");
        }
        
    }
}
