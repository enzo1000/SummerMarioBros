using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusClass : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Par exemple, vérifiez le nom du collider
        if (collision.gameObject.name == "Jumphitbox")
        {
            DataToStore.instance.AddCoins(1);
            Destroy(gameObject);
        }
    }
}
