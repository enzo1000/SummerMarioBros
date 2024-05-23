using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataToStore : MonoBehaviour
{
    public static DataToStore instance; //Singleton pattern

    public float playerMovement;            // Stocke la valeur de l'axe horizontal du joueur
    public bool firstMovementCheck = false; // Stocke l'état du premier mouvement du joueur

    public int coinsCount;      //Compteur de pièces
    public Text coinsCountText; // Texte affichant le nombre de pièces

    public IDictionary<string, float> levelInfo;        // Stocke des informations sur le niveau
    public IDictionary<string, float> playerTimeInfo;   // Stocke diverse informations importantes concernant le joueur
    public IDictionary<string, string> causeOfDeath;    // Stocke des informations sur la cause de la mort du joueur

    public bool isGrounded = true;                      // Stocke l'état du joueur par rapport au sol
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

        InitDataToStoreField();
        StartCoroutine(StartTimer(levelName));
        EnemyList();
        CoinsList();

        //Recupere le CompositeCollider2D du sol du niveau
        LevelCompoCol2D = GameObject.FindGameObjectWithTag("Ground").GetComponent<CompositeCollider2D>();
        //Debug.Log(LevelCompoCol2D.bounds.min.x + " | " + LevelCompoCol2D.bounds.max.x);
    }

    //Initialise les champs importants pour le stockage des données du joueur)
    private void InitDataToStoreField()
    {
        //Temps auquel le joueur fait son premier deplacement
        playerTimeInfo.Add(levelName + "FirstDeplacementTimer", 0.0f);

        //Temps auquel le joueur s'arrete de bouger (utilise pour calculer la plus grande pause)
        playerTimeInfo.Add(levelName + "StartPause", 0.0f);
        //Temps de la plus grande pause sans se deplacer du joueur
        playerTimeInfo.Add(levelName + "MaxPause", 0.0f);

        //Temps que le joueur a passe sans bouger
        playerTimeInfo.Add(levelName + "PauseTime", 0.0f);
        //Temps que le joueur a passe a aller a gauche
        playerTimeInfo.Add(levelName + "LeftDeplacementTimer", 0.0f);
        //Temps que le joueur a passe a aller a droite
        playerTimeInfo.Add(levelName + "RightDeplacementTimer", 0.0f);

        //Nombre de saut que le joueur a réalisés
        playerTimeInfo.Add(levelName + "JumpCount", 0.0f);

        //Si le joueur saute, alors on stock le temps de départ (où le saut commence) pour pouvoir le soustraire quand il atterira
        playerTimeInfo.Add(levelName + "JumpAirTimeStart", 0.0f);
        //Idem mais mais pour le temps passe en l'air
        playerTimeInfo.Add(levelName + "AirTimeStart", 0.0f);

        //Temps passe en l'air
        playerTimeInfo.Add(levelName + "AirTime", 0.0f);
        //Temps passe en l'air apres un saut
        playerTimeInfo.Add(levelName + "JumpAirTime", 0.0f);
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

    public void AddCoins(int count)
    {
        coinsCount += count;
        coinsCountText.text = coinsCount.ToString();
    }

    //Une fonction set afin de formaliser la modification de la valeur de playerMovement dans DataToStore
    public void UpdatePlayerMovement(float Movement)
    {
        playerMovement = Movement;
    }

    //Une autre fonction de set pour renseigner les informations de saut
    public void jumpingData()
    {
        playerTimeInfo[levelName + "JumpAirTimeStart"] = playerTimeInfo[levelName + "Timer"];
    }

    public void UpdateGroundCheckData(bool playerIsGrounded)
    {
        if (playerIsGrounded == isGrounded) return; //verification des etats de transition de isGrounded pour eviter les problemes de calcul
        //(Le joueur met en moyenne 4 frames a quitter le sol, donc on ne veut pas enregistrer le saut 4 fois en 1 frame)
        isGrounded = playerIsGrounded;
        isGroundedCheckTimer();
    }

    private IEnumerator StartTimer(string levelName)
    {
        string timerName = levelName + "Timer";
        float clockFreq = 0.25f;
        playerTimeInfo.Add(timerName, 0.0f);
        while (!levelFinished)
        {
            yield return new WaitForSeconds(clockFreq);
            playerTimeInfo[timerName] += clockFreq;

            DirectionCheckTimer(clockFreq);
        }
        //Fin du niveau
        // ...
    }

    private void isGroundedCheckTimer()
    {
        //Le joueur est en l'air
        if (!isGrounded)
        {
            //Le joueur n'a pas saute et qu'il vient de quitter le sol alors :
            if (playerTimeInfo[levelName + "JumpAirTimeStart"] == 0 && playerTimeInfo[levelName + "AirTimeStart"] == 0)
            {
                playerTimeInfo[levelName + "AirTimeStart"] = playerTimeInfo[levelName + "Timer"];
            }
        }
        //Quand le joueur re touche le sol
        else
        {
            if (playerTimeInfo[levelName + "JumpAirTimeStart"] != 0.0f)
            {
                playerTimeInfo[levelName + "JumpAirTime"] += playerTimeInfo[levelName + "Timer"] - playerTimeInfo[levelName + "JumpAirTimeStart"];
                playerTimeInfo[levelName + "JumpAirTimeStart"] = 0.0f;
                playerTimeInfo[levelName + "JumpCount"]++;
            }
            else if (playerTimeInfo[levelName + "AirTimeStart"] != 0.0f)
            {
                playerTimeInfo[levelName + "AirTime"] += playerTimeInfo[levelName + "Timer"] - playerTimeInfo[levelName + "AirTimeStart"];
                playerTimeInfo[levelName + "AirTimeStart"] = 0.0f;
            }
        }
    }

    //Fonction un peu foure tout pour les timers relié à la direction du joueur / mouvements du joueur
    private void DirectionCheckTimer(float clockFreq)
    {
        if (playerMovement != 0)
        {
            //Temps passe sans bouger avant le premier mouvement
            if (firstMovementCheck == false)
            {
                firstMovementCheck = true;
                playerTimeInfo[levelName + "FirstDeplacementTimer"] = playerTimeInfo[levelName + "Timer"];
            }

            //Si le joueur va à droite / gauche
            if (playerMovement < 0)
            {
                playerTimeInfo[levelName + "LeftDeplacementTimer"] += clockFreq;
            } else
            {
                playerTimeInfo[levelName + "RightDeplacementTimer"] += clockFreq;
            }

            //Si le joueur bouge et que le timer de pause est différent de 0, on stocke le temps que le joueur a passé sans bouger
            //précision, on pourrait certainement ne pas avoir à réaliser ce if mais pour des raisons de clarté, on préfèrera le laisser
            if (playerTimeInfo[levelName + "StartPause"] != 0)
            {
                float timePaused = playerTimeInfo[levelName + "Timer"] - playerTimeInfo[levelName + "StartPause"];
                if (timePaused > playerTimeInfo[levelName + "MaxPause"])
                {
                    playerTimeInfo[levelName + "MaxPause"] = timePaused;
                }
                playerTimeInfo[levelName + "StartPause"] = 0.0f;
            }
        }
        else
        {
            //Si le joueur s'arrête de bouger, on stocke le temps où le joueur commence à arreter de bouger
            if (playerTimeInfo[levelName + "StartPause"] == 0.0f)
            {
                playerTimeInfo[levelName + "StartPause"] = playerTimeInfo[levelName + "Timer"];
            }
            //On incremente aussi le temps que le joueur passe sans bouger
            playerTimeInfo[levelName + "PauseTime"] += clockFreq;
        }
    }
}