using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length, startPosX;
    private SpriteRenderer[] spriteRenderers;
    public float parallaxEffectMultiplier; // Parallax effet

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length > 0)
        {
            startPosX = transform.position.x;
            length = spriteRenderers[0].bounds.size.x; //Calcule la longueur d'un sprite du background
        }
        else
        {
            Debug.LogError("No SpriteRenderers found in children of " + gameObject.name);
        }
    }

    void Update()
    {
        // on veut créer un effet de parallaxe
        // Quand un des enfants du background sort de l'écran, on veut le déplacer de l'autre côté de l'écran

        // On verifie qu'on a bien un background
        if (spriteRenderers.Length > 0)
        {
            // on vérifie si un des enfants du background est hors de l'écran
            //on calcule la distance entre la camera et le background
            float temp = (cam.transform.position.x * (1 - parallaxEffectMultiplier));
            float dist = (cam.transform.position.x * parallaxEffectMultiplier);

            //on déplace le background de l'autre côté de l'écran
            transform.position = new Vector3(startPosX + dist, transform.position.y, transform.position.z);

            // si le background est hors de l'écran, on le déplace de l'autre côté de l'écran
            if (temp > startPosX + length)
            {
                startPosX += length;
            }
            else if (temp < startPosX - length)
            {
                startPosX -= length;
            }
        }


    }
}
