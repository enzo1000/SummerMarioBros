using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataToStore : MonoBehaviour
{
    public static DataToStore instance; //Singleton pattern

    public int coinsCount;      //Compteur de pièces
    public Text coinsCountText; // Texte affichant le nombre de pièces

    public IDictionary<string, float> levelInfo;
    public IDictionary<string, float> playerTimeInfo; // Stocke diverse informations importantes concernant le joueur
    public bool levelFinished = false; // Stocke l'état du niveau
    public string levelName = "Level01"; // Nom du niveau

    //Singleton pattern
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of DataToStore found!");
            return;
        }
        instance = this;

        playerTimeInfo = new Dictionary<string, float>();
        levelInfo = new Dictionary<string, float>();

        StartCoroutine(StartTimer(levelName));
        EnemyList();
    }

    private void EnemyList()
    {
        int numberOfEnemies = 0;
        GameObject GroupOfEnemy = GameObject.FindGameObjectWithTag("Enemy");
        foreach (Transform enemy in GroupOfEnemy.GetComponentInChildren<Transform>())
        {
            numberOfEnemies++;
            //Debug.Log(numberOfEnemies);
        }
        levelInfo.Add(levelName + "Enemies", numberOfEnemies);
    }

    private void tileMapInfo()
    {
        //To be completed
    }

    public void AddCoins(int count)
    {
        coinsCount += count;
        coinsCountText.text = coinsCount.ToString();
    }

    private IEnumerator StartTimer(string levelName)
    {
        string timerName = levelName + "Timer";
        playerTimeInfo.Add(timerName, 0.0f);
        while (!levelFinished)
        {
            yield return new WaitForSeconds(0.25f);
            playerTimeInfo[timerName] += 0.25f;
            //Debug.Log(playerTimeInfo[timerName]);
        }
        //Fin du niveau
        // ....
    }
}