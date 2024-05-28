using UnityEngine;
using UnityEngine.UIElements;

public class ParallaxEffect : MonoBehaviour
{
    private float length, startPosX;
    private SpriteRenderer[] spriteRenderers;
    public float parallaxEffectMultiplier; // Parallax effec

    private Camera cam;

    void Start()
    {
        //cam = Camera.main;
        cam = FindObjectOfType<Camera>();

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length > 0)
        {
            startPosX = transform.position.x;
            length = spriteRenderers[0].bounds.size.x; // Calculate the length of one sprite
        }
        else
        {
            Debug.LogError("No SpriteRenderers found in children of " + gameObject.name);
        }
    }

    void Update()
    {
        // we want to create a parallax effect 
        //when one of the backgroung child go out of the screen, we want to move it to the other side of the screen

        // we check if we have a background
        if (spriteRenderers.Length > 0)
        {
            // we check if one of the background child is out of the screen
            float temp = (cam.transform.position.x * (1 - parallaxEffectMultiplier));// we calculate the distance between the camera and the background
            float dist = (cam.transform.position.x * parallaxEffectMultiplier); // we calculate the distance between the camera and the background

            // we move the background to the other side of the screen
            transform.position = new Vector3(startPosX + dist, transform.position.y, transform.position.z);

            // if the background is out of the screen, we move it to the other side of the screen
            if (temp > startPosX + length)
            {
                startPosX += length;
            }
            else if (temp < startPosX - length)
            {
                startPosX -= length;
            }
        }





        //if (spriteRenderers.Length > 0)
        //{
        //    float temp = (cam.transform.position.x * (1 - parallaxEffectMultiplier));
        //    float dist = (cam.transform.position.x * parallaxEffectMultiplier);

        //    transform.position = new Vector3(startPosX + dist, transform.position.y, transform.position.z);

        //    if (temp > startPosX + length)
        //    {
        //        startPosX += length;
        //    }
        //    else if (temp < startPosX - length)
        //    {
        //        startPosX -= length;
        //    }
        //}
    }
}
