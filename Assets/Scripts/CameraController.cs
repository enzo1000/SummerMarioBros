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
        //transform.position = new Vector3(player.position.x, 2, -10);
        //Faite en sorte que la caméra ne puisse pas suivre le joueur en dehors de la bordure du monde
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

        //cas min && max
        if (playerPosition.x > minBound + (gameObject.GetComponent<Camera>().orthographicSize * 2 - 1)
            && playerPosition.x < maxBound - (gameObject.GetComponent<Camera>().orthographicSize * 2 - 1)) 
        {
            // Bouger la caméra de manière fluide de sa position actuelle à la position du joueur (Uniquement sur l'axe X)
            transform.position = Vector3.SmoothDamp(cameraPositionX, playerPosition, ref velocity, timeOffset);
        }
        else
        {
            transform.position = new Vector3(7, 2, -10);
        }
    }
}
