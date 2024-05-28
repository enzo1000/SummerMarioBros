using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private bool isInvincible = false;
    private GameObject playerSpawn;
    private GameObject lifePoints;
    private Animator anim; // Référence à l'Animator

    public static PlayerHealth instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerHealth found!");
            return;
        }
        instance = this;
    }

    void Start()
    {
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        anim = GetComponent<Animator>();
    }

    private void CanvasHealthModification(float source)
    {
        lifePoints = GameObject.FindGameObjectWithTag("LifePoints");
        int currentHealth = 0;
        List<Transform> transformsTab = new List<Transform>();

        //On compte le nombre de pdv actifs
        foreach (Transform lifePoint in lifePoints.GetComponentInChildren<Transform>())
        {
            if (lifePoint.gameObject.activeSelf)
            {
                currentHealth++;
                transformsTab.Add(lifePoint);
            }
        }

        //On désactive le dernier pdv
        transformsTab[currentHealth - 1].gameObject.SetActive(false);

        //Si c'était son dernier point de vie, Game Over
        if (currentHealth == 1)
        {
            GameOver(source);
        }
    }

    //Fonction pour récupérer tous les points de vie sur le canvas
    private void RecoverFullHealth()
    {
        foreach (Transform lifePoint in lifePoints.GetComponentInChildren<Transform>())
        {
            lifePoint.gameObject.SetActive(true);
        }
    }

    //Fonction pour gérer le Game Over : 
    // - On stocke la cause de la mort
    // - On désactive les inputs
    // - On lance l'animation de mort + affichage du menu de Game Over
    public void GameOver(float source)
    {
        DataToStore.instance.causeOfDeath["CauseOfDeath"] = source;
        DataToStore.instance.causeOfDeath["XDeath"] = gameObject.transform.position.x;
        DataToStore.instance.causeOfDeath["YDeath"] = gameObject.transform.position.y;

        gameObject.GetComponent<PlayerMovement>().enabled = false;   //bloque les inputs
        anim.SetTrigger("isDead");
        gameObject.GetComponent<Rigidbody2D>().simulated = false;   //Empeche les intéractions physiques avec les autre éléments

        GameOverMenu.instance.OnPlayerDeath();
    }

    //Fonction pour gérer le respawn du joueur appelé dans GameOverMenu.cs
    public void Respawn()
    {
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        anim.SetTrigger("Respawn");
        gameObject.GetComponent<Rigidbody2D>().simulated = true;
        RecoverFullHealth();
    }

    public void TakeDamage(float source)
    {
        //Checking isInvincible to avoid multiple hits
        if (!isInvincible) CanvasHealthModification(source);

        //0 = "Hole", 1 = "Gumba"
        if (source == 0.0f)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = playerSpawn.transform.position;
        }

        isInvincible = true;
        StartCoroutine(InvincibilityTime());
        StartCoroutine(InvincibilityFlash());
    }

    public IEnumerator InvincibilityFlash()
    {
        while(isInvincible)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(0.25f);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.25f);
        }
    }

    public IEnumerator InvincibilityTime()
    {
        yield return new WaitForSeconds(3f);
        isInvincible = false;
    }
}
