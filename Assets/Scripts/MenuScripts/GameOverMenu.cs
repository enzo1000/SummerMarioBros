using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public static GameOverMenu instance;
    public GameObject gameOverUI;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GameOverMenu found!");
            return;
        }
        instance = this;
    }

    public void OnPlayerDeath()
    {
        if (CurrentSceneManager.instance.isPlayerPresentByDefault)
        {
            DontDestroyOnLoadScene.instance.RemoveFromDDOL();
        }

        gameOverUI.SetActive(true);
    }

    public void RetryButton()
    {
        DataToStore.instance.RemoveCoins(CurrentSceneManager.instance.coinsPickedUpCount);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerHealth.instance.Respawn();
        gameOverUI.SetActive(false);
    }

    public void MainMenuButton()
    {
        DontDestroyOnLoadScene.instance.RemoveFromDDOL();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
