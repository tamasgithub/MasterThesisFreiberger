using System;
using System.Collections;
using UnityEngine;

public class FunctionMachine : MonoBehaviour
{
    [SerializeField]
    private bool gearsTurning = true;
    [SerializeField]
    private Transform[] gears = new Transform[3];
    [SerializeField]
    private GameObject[] outputHoles;
    [SerializeField]
    private Function function;
    [SerializeField]
    private bool outputDecodable = false;
    [SerializeField]
    private bool outputVolatile = false;

    // events the FM fires
    public event Action hoverEvent;
    public event Action inputsAcceptedEvent;
    public event Action inputsProcessedEvent;
    public event Action failedToPrecessInputsEvent;

    private FMInputHole[] inputHoles;
    private DataPrefabHolder dataPrefabHolder;
    private float gearsRotatedThisFrame;
    private bool processingInputs = false;

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

        if (outputHoles.Length != FunctionDetails.GetFunctionOutputTypes(function).Length)
        {
            Debug.LogError("The number of output holes doesn't match the number" +
                " of outputs of the function");
        }
        dataPrefabHolder = FindObjectOfType<DataPrefabHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        gears[0].GetChild(0).gameObject.SetActive(!gearsTurning);
        if (gearsTurning)
        {
            RotateGears(Time.deltaTime * 200);
        }

        if (AllInputsFilled())
        {
            if (inputsAcceptedEvent != null)
            {
                inputsAcceptedEvent();
            }
            
            ProcessInputs();
        }
    }

    private void LateUpdate()
    {
        gearsRotatedThisFrame = 0;
    }

    public bool AreGearsTurning()
    {
        return gearsTurning;
    }

    public void RotateGears(float angle)
    {

        for (int i = 0; i < gears.Length; i++)
        {
            gears[i].Rotate(new Vector3(0, 0, angle * 2 * (i % 2 - 0.5f)));
        }
        gearsRotatedThisFrame = angle;
    }

    public bool IsProcessingInputs()
    {
        return processingInputs;
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
        processingInputs = true;
        object[] inputValues = new object[inputHoles.Length];
        for (int i = 0; i < inputHoles.Length; i++)
        {
            inputValues[i] = inputHoles[i].LetInputIn();
        }
        try
        {
            object[] outputValues = FunctionDetails.GetImplementation(function)(inputValues);
            StartCoroutine(CreateOutputs(outputValues));
        } catch
        {
            if (failedToPrecessInputsEvent != null)
            {
                failedToPrecessInputsEvent();
            }
        }
        
        
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
            Type outputType = FunctionDetails.GetFunctionOutputTypes(function)[i];
            if (outputType == typeof(string) && ((string)outputValues[i]).Length == 1)
            {
                outputType = typeof(char);
            }
            GameObject outputPrefab = dataPrefabHolder.GetDataPrefabForType(outputType);

            GameObject outputData = Instantiate(outputPrefab, outputHoles[i].transform.position, Quaternion.identity);
            outputData.transform.Translate(Vector3.forward * 2 + Vector3.right * UnityEngine.Random.Range(-.1f, .1f));
            outputData.GetComponentInChildren<TextMesh>().text = outputValues[i].ToString();
            if (outputType == typeof(int) && outputDecodable)
            {
                outputData.AddComponent<IntFMOutputDecoder>();
            }
            if (outputVolatile)
            {
                outputData.AddComponent<FMVolatileOutput>();
            }
        }
        if (inputsProcessedEvent != null)
        {
            inputsProcessedEvent();
        }
        processingInputs = false;
    }

    private void OnMouseEnter()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        if (hoverEvent != null)
        {
            hoverEvent();
        }
        
    }

    private void OnMouseExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
