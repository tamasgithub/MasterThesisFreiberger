using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class DropdownHandler : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Dropdown>().value = (int)GameObject.Find("Translator").GetComponent<I18N>().gameLang;
    }

    public void SetLanguage(int languageId)
    {
        GameObject.Find("Translator").GetComponent<I18N>().setLanguage((LanguageCode)languageId);
    }
}

