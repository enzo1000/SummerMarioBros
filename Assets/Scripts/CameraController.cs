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
        //Store the start position of the camera
        Vector3 posOffset = new Vector3(0, transform.position.y, transform.position.z);

        //Store the position of the camera
        Vector3 cameraPositionX = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //Store the X position of the player and also add the camera offset on Y and Z axis
        Vector3 playerPosition = new Vector3(player.position.x, 0, 0) + posOffset;

        //min && max cases
        if (playerPosition.x > minBound + (gameObject.GetComponent<Camera>().orthographicSize * 2 - 1)
            && playerPosition.x < maxBound - (gameObject.GetComponent<Camera>().orthographicSize * 2 - 1)) 
        {
            //Smoothly move the camera from its current position to the player position (Only on X axis)
            transform.position = Vector3.SmoothDamp(cameraPositionX, playerPosition, ref velocity, timeOffset);
        }
    }
}
