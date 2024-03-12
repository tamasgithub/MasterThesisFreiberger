using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecodingTask : Task
{
    public int countOfOutputsToDecode = 1;
    private int outputsDecoded = 0;
    public override void StartTask()
    {
        foreach(IntFMOutputDecoder decoder in FindObjectsOfType(typeof(IntFMOutputDecoder))) {
            decoder.decodedEvent += OutputDecoded;
        }
    }

    public void OutputDecoded()
    {
        outputsDecoded++;
        if (outputsDecoded == countOfOutputsToDecode)
        {
            gameObject.SetActive(false);
            TaskFinished();
        }
    }
}
