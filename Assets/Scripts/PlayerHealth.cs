using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private bool isInvincible = false;
    private GameObject lifePoints;

    private void CanvasHealthModification()
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
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        //bloquer les inputs
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        //Jouer l'animation de mort (To be implemented)

        //empecher les intéractions physiques avec les autre éléments
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
    }

    public void TakeDamage()
    {
        //Checking isInvincible to avoid multiple hits
        if (!isInvincible) CanvasHealthModification();
  
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
