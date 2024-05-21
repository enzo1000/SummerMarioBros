using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataToStore : MonoBehaviour
{
    public static DataToStore instance; //Singleton pattern

    public int coinsCount;      //Compteur de pièces
    public Text coinsCountText; // Texte affichant le nombre de pièces

    public IDictionary<string, float> levelInfo;        // Stocke des informations sur le niveau
    public IDictionary<string, float> playerTimeInfo;   // Stocke diverse informations importantes concernant le joueur
    public IDictionary<string, string> causeOfDeath;     // Stocke des informations sur la cause de la mort du joueur

    public bool levelFinished = false;                  // Stocke l'état du niveau
    public string levelName = "Level01";                // Nom du niveau
    public CompositeCollider2D LevelCompoCol2D;

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
        causeOfDeath = new Dictionary<string, string>();

        StartCoroutine(StartTimer(levelName));
        EnemyList();

        //Recupere le CompositeCollider2D du sol du niveau
        LevelCompoCol2D = GameObject.FindGameObjectWithTag("Ground").GetComponent<CompositeCollider2D>();
        //Debug.Log(LevelCompoCol2D.bounds.min.x + " | " + LevelCompoCol2D.bounds.max.x);
    }

    private void EnemyList()
    {
        int numberOfEnemies = 0;
        GameObject GroupOfEnemy = GameObject.FindGameObjectWithTag("Enemy");
        foreach (Transform enemy in GroupOfEnemy.GetComponentInChildren<Transform>())
        {
            numberOfEnemies++;
        }
        levelInfo.Add(levelName + "Enemies", numberOfEnemies);
    }

    private void CoinsList()
    {
        int numberOfCoins = 0;
        GameObject GroupOfCoins = GameObject.FindGameObjectWithTag("Coins");
        foreach (Transform coin in GroupOfCoins.GetComponentInChildren<Transform>())
        {
            numberOfCoins++;
        }
        levelInfo.Add(levelName + "Coins", numberOfCoins);
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