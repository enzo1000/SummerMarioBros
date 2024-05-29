using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.transform.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(0.0f);

            //S'occupe de faire respawn le joueur
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject lastPositionRegistered = GameObject.FindGameObjectWithTag("LastPositionRegistered");
            player.transform.position = lastPositionRegistered.transform.position;
        }
    }
}
