using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusClass : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            DataToStore.instance.AddCoins(1);
            CurrentSceneManager.instance.coinsPickedUpCount++;
            Destroy(gameObject);
        }
    }
}
