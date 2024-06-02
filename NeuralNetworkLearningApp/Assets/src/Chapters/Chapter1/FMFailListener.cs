using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FMFailListener : MonoBehaviour
{
    public FunctionMachine machine;
    public GameObject failOverlay;
    // Start is called before the first frame update
    void Start()
    {
        machine.failedToPrecessInputsEvent += HandleFail;
    }

    private void HandleFail()
    {

        failOverlay.SetActive(true);
    }

    public void OnRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}