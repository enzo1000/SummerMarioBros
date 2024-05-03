using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float timeOffset;

    private Vector3 velocity;

    void Update()
    {
        //Store the start position of the camera
        Vector3 posOffset = new Vector3(0, transform.position.y, transform.position.z);

        //Store the position of the camera
        Vector3 cameraPositionX = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //Store the X position of the player and also add the camera offset on Y and Z axis
        Vector3 playerPositionX = new Vector3(player.position.x, 0, 0) + posOffset;

        //Smoothly move the camera from its current position to the player's position (Only on X axis)
        transform.position = Vector3.SmoothDamp(cameraPositionX, playerPositionX, ref velocity, timeOffset);
    }
}
