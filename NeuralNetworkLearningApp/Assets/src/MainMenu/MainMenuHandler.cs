using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject achievementsPanel;
    public GameObject settingsPanel;

    public void OpenChapters()
    {
        SceneManager.LoadSceneAsync("ChaptersOverview");
    }

    public void CloseChapters()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void OpenAchievements()
    {
        mainMenuPanel.SetActive(false);
        achievementsPanel.SetActive(true);
    }

    public void CloseAchievements()
    {
        mainMenuPanel.SetActive(true);
        achievementsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
