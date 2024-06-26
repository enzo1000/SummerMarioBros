using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private bool isInvincible = false;
    private GameObject lifePoints;
    private Animator anim; // R�f�rence � l'Animator

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

        //On d�sactive le dernier pdv
        transformsTab[currentHealth - 1].gameObject.SetActive(false);

        DataToStore.instance.SetCauseOfDeath(currentHealth - 1, source, gameObject.transform.position.x, gameObject.transform.position.y);

        //Si c'�tait son dernier point de vie, Game Over
        if (currentHealth == 1)
        {
            GameOver();
        }
    }

    //Fonction pour r�cup�rer tous les points de vie sur le canvas
    private void RecoverFullHealth()
    {
        foreach (Transform lifePoint in lifePoints.GetComponentInChildren<Transform>())
        {
            lifePoint.gameObject.SetActive(true);
        }
    }

    //Fonction pour g�rer le Game Over : 
    // - On stocke la cause de la mort
    // - On d�sactive les inputs
    // - On lance l'animation de mort + affichage du menu de Game Over
    public void GameOver()
    {
        DataToStore.instance.ProcessEndOfLevelData();   //On calcul nos taux de bonus / kill
        EndOfLevel.instance.FindDicoDatas();            //On envoie nos donn�es � EndOfLevel.cs
        DataToStore.instance.ResetData(SceneManager.GetActiveScene().name);    //On reset nos dictionnaires pour recommencer

        gameObject.GetComponent<PlayerMovement>().enabled = false;   //bloque les inputs
        anim.SetTrigger("isDead");
        gameObject.GetComponent<Rigidbody2D>().simulated = false;   //Empeche les int�ractions physiques avec les autre �l�ments

        GameOverMenu.instance.OnPlayerDeath();
    }

    //Fonction pour g�rer le respawn du joueur appel� dans GameOverMenu.cs
    public void Respawn()
    {
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        anim.SetTrigger("Respawn");
        gameObject.GetComponent<Rigidbody2D>().simulated = true;
        RecoverFullHealth();
    }

    public void TakeDamage(float source)
    {
        //Pour le source : 0 = "Hole", 1 = "Gumba"
        // Check si le joueur est invincible pour �viter les multiples hits
        if (!isInvincible) CanvasHealthModification(source);

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
