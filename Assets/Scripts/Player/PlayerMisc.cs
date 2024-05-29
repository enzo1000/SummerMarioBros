using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//Classe qui peut aussi etre deplace dans le GameManager dependant du pdv
public class PlayerMisc : MonoBehaviour
{
    private GameObject playerSpawn;
    private GameObject LastPositionRegistered;         // Référence au point de spawn du joueur
    private GameObject player;                         // Référence au joueur
    private GroundedTest groundTest;

    //Appelé en premier dans l'éxécution du prgramme
    private void Awake()
    {
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        SceneManager.sceneLoaded += OnSceneLoad;
        LastPositionRegistered = GameObject.FindGameObjectWithTag("LastPositionRegistered");

        groundTest = GetComponentInChildren<GroundedTest>();
    }
    
    //Appelé lorsqu'une scène est chargée
    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
            LastPositionRegistered = GameObject.FindGameObjectWithTag("LastPositionRegistered");
            GameObject.FindGameObjectWithTag("Player").transform.position = playerSpawn.transform.position;
        }
    }

    void UpdatePlayerSpawn()
    {
        //LastPositionRegistered.transform.position = new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), 0);
        LastPositionRegistered.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
    }

    //Appelé à chaque frame
    private void Update()
    {
        if (groundTest.isGrounded)
        {
            UpdatePlayerSpawn();
        }
    }
}
