using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string levelToLoad;

    [SerializeField]
    private GameObject settingWindow;

    public void StartGame()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void OpenSettings()
    {
        settingWindow.SetActive(true);
    }

    public void CloseSettings()
    {
        settingWindow.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
