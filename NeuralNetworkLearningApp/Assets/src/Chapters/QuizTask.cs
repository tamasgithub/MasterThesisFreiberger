using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizTask : Task
{
    public GameObject answerPrefab;
    public string[] answers;
    public List<int> correctAnswers;
    //not yet implemented
    bool multiselect = true;

    // Start is called before the first frame update
    public override void StartTask()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            GameObject newAnswer = Instantiate(answerPrefab, Vector3.zero, Quaternion.identity, transform);
            newAnswer.transform.SetSiblingIndex(transform.childCount - 2);
            newAnswer.GetComponentInChildren<Text>().text = answers[i];
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Submit()
    {
        bool correct = true;
        foreach (Toggle toggle in GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn == correctAnswers.Contains(toggle.transform.parent.GetSiblingIndex() - 1))
            {
                if (toggle.isOn)
                {
                    toggle.transform.GetComponentInChildren<Text>().color = Color.green;
                } else
                {
                    toggle.transform.GetComponentInChildren<Text>().color = Color.white;
                }
            } else
            {
                correct = false;
                toggle.transform.GetComponentInChildren<Text>().color = Color.red;
            }
        }
        if (correct)
        {
            transform.GetChild(transform.childCount - 1).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(transform.childCount - 1).GetChild(1).gameObject.SetActive(true);
        }
    }

    public void Next()
    {
        gameObject.SetActive(false);
        TaskFinished();
    }
}
