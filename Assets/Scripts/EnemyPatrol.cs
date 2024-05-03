using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Ce système de patrouille est basé sur la méthode des waypoints
// ce système consitet à définir des points de passage pour l'ennemi
// l'ennemi se déplace de point en point (aller-retour)
public class EnemyPatrol : MonoBehaviour
{
    public float speed;
    public Transform[] waypoints;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    //Variable pour stocker le point de passage vers lequel l'ennemie ce dirige
    private Transform target;
    //Variable pour stocker l'index du point de passage vers lequel l'ennemie ce dirige (Le même que celui de la variable target)
    private int destPoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        target = waypoints[0];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = target.position - transform.position;
        //On normalise le vecteur (sa longueur devient 1)
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        //Si l'ennemie est proche du point de passage (le superpos)
        if (Vector3.Distance(transform.position, target.position) < 0.3f)
        {
            //On parcours notre liste de waypoints modulo la taille de la liste pour éviter les débordements
            destPoint = (destPoint + 1) % waypoints.Length;
            target = waypoints[destPoint];
        }

        Flip(dir.x);

    }

    void Flip(float speed)
    {
        if (speed < 0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (speed > -0.1f)
        {
            spriteRenderer.flipX = false;
        }
    }
}
