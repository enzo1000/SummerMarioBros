using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float timeOffset;

    private Vector3 velocity;
    private float minBound;
    private float maxBound;

    private void Start()
    {
        //Make the camera unable to follow the player out of the border of the world
        minBound = DataToStore.instance.LevelCompoCol2D.bounds.min.x;
        maxBound = DataToStore.instance.LevelCompoCol2D.bounds.max.x;
    }

    void Update()
    {
        //Enregistrer la position de départ de la caméra
        Vector3 posOffset = new Vector3(0, transform.position.y, transform.position.z);

        //Enregistrer la position de la caméra
        Vector3 cameraPositionX = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //Enregistrer la position du joueur et ajouter l'offset de la caméra sur les axes Y et Z
        Vector3 playerPosition = new Vector3(player.position.x, 0, 0) + posOffset;

<<<<<<< HEAD
        //min && max cases
=======

        //Faite en sorte que la caméra ne puisse pas suivre le joueur en dehors de la bordure du monde
        float minBound = DataToStore.instance.LevelCompoCol2D.bounds.min.x;
        float maxBound = DataToStore.instance.LevelCompoCol2D.bounds.max.x;

        //cas min && max
>>>>>>> aed09f11cc138e760ec2a20f3573d8ab019e99ae
        if (playerPosition.x > minBound + (gameObject.GetComponent<Camera>().orthographicSize * 2 - 1)
            && playerPosition.x < maxBound - (gameObject.GetComponent<Camera>().orthographicSize * 2 - 1)) 
        {
            // Bouger la caméra de manière fluide de sa position actuelle à la position du joueur (Uniquement sur l'axe X)
            transform.position = Vector3.SmoothDamp(cameraPositionX, playerPosition, ref velocity, timeOffset);
        }
    }
}
