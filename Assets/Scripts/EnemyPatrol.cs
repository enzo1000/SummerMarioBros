using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Ce système de patrouille est basé sur la méthode des waypoints
// ce système consitet à définir des points de passage pour l'ennemi
// l'ennemi se déplace de point en point (aller-retour)
public class EnemyPatrol : MonoBehaviour
{
    private Animator anim; // Référence à l'Animator
    public float speed;
    public Transform[] waypoints;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    //Variable pour stocker le point de passage vers lequel l'ennemie ce dirige
    private Transform target;
    //Variable pour stocker l'index du point de passage vers lequel l'ennemie ce dirige (Le même que celui de la variable target)
    private int destPoint = 0;
    //Variable pour stocker le weakSpot de l'ennemie et le réutiliser pour vérifier si le joueur est en collision avec
    private BoxCollider2D weakPoint;

    // Start is called before the first frame update
    void Start()
    {
        //On récupère l'Animator de l'ennemi
        anim = GetComponent<Animator>();

        //On initialise le point de passage de l'ennemie à son premier point de passage
        target = waypoints[0];

        player = GameObject.FindGameObjectWithTag("Player");

        //On ajoute un weakSpot à l'ennemie pour pouvoir le tuer
        CreateBoxCollider2D(new Vector2(0f, 0.1f), new Vector2(0.1f, 0.02f));
        
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

        if (player == null)
        {
            return;
        }

        if(weakPoint.IsTouching(player.GetComponent<CircleCollider2D>()))
        {
            //Permet de normaliser la vélocité du joueur pour éviter d'avoir une inconsistance sur la hauteur de saut
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, 0f);
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 500.0f));
            //On joue l'animation de mort?
            
            //On détruit l'ennemi apres l'animation de mort
            Destroy(transform.parent.gameObject);
        }

        Flip(dir.x);
    }

    void CreateBoxCollider2D(Vector2 offset, Vector2 size)
    {
        BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D.offset = offset;
        boxCollider2D.size = size;
        weakPoint = boxCollider2D;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.transform.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(1.0f);
        }
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
