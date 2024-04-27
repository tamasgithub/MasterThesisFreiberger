using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text label;
    public string description;

    public void Resume()
    {
        GameObject.Find("Player").GetComponent<PlayerControl>().ToggleMenu();
    }
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitToDesktop()
    {
            Application.Quit();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        label.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        label.text = description;
    }
}
