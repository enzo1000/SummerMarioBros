using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private bool isInvincible = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void TakeDamage()
    {
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
