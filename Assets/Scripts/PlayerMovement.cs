using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb; // Référence au Rigidbody2D
    private Animator anim; // Référence à l'Animator
    private GameObject playerSpawn; // Référence au point de spawn du joueur
    private GameObject pauseMenu;   // Référence au menu pause

    public float speed; // Vitesse de déplacement   
    public float jumpHeight = 6f; // Hauteur désirée pour le saut
    public float timeToJumpApex = 0.6f; // Temps pour atteindre le point le plus haut du saut

    private float gravity; // Gravité calculée
    private float jumpVelocity; // Vélocité de saut calculée

    private float Move; // Stocke la valeur de l'axe horizontal
    private bool isFacingRight; // Stocke la direction du joueur
    private GroundedTest groundedTester; // Référence au script GroundedTest
    private bool jump = false; // Stocke l'état du saut
    private bool isMovable = true; // Stocke l'état du mouvement

    void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn"); // Récupère le GameObject avec le tag "PlayerSpawn"
        //On passe par un parent actif pour atteindre un enfant inactif (FindGameObjectWithTag ne peut pas le faire)
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").transform.GetChild(0).gameObject; // Récupère le GameObject avec le tag "PauseMenu"

        if (pauseMenu == null)
        {
            Debug.LogError("PauseMenu not found");
        }

        // Calcul de la gravité et de la vélocité de saut basées sur la hauteur de saut et le temps pour atteindre l'apex
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        groundedTester = GetComponentInChildren<GroundedTest>(); // Récupère le script GroundedTest attaché à l'enfant du joueur
    }

    void Update()
    {
        //Ouverture du menu pause
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowPauseMenu();
        }

        if (!isMovable) return; // Si le joueur ne peut pas bouger, on sort de la fonction

        Move = Input.GetAxis("Horizontal");
        if (groundedTester.isGrounded) {  //Si notre personnage est au sol alors
            UpdatePlayerSpawn();          // Met à jour le point de spawn du joueur

            if (Input.GetKey(KeyCode.Space)) {  //Si notre joueur a la touche espace enfoncée
                jump = true;                    // Saut
            }
        }

        anim.SetBool("isRunning", Mathf.Abs(Move) > 0); // Animation de course
        anim.SetBool("isJumping", !groundedTester.isGrounded); // Animation de saut

        if ((!isFacingRight && Move > 0) || (isFacingRight && Move < 0))
        {
            Flip(); // Retourne le joueur si nécessaire
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(Move * speed, rb.velocity.y); // Déplacement horizontal

        if (jump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity); // Saut
            jump = false;
        }

        // Applique la gravité calculée lorsque le joueur n'est pas au sol
        if (!groundedTester.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + gravity * Time.fixedDeltaTime); // Applique la gravité
        }
    }

    public void ShowPauseMenu()
    {
        //A minima, désactiver les mouvements du joueur
        isMovable = !isMovable;
        //Afficher le menu pause
        if (!isMovable)
        {
            pauseMenu.gameObject.SetActive(true);
            Move = 0;
            rb.velocity = new Vector2(0, 0);
            rb.simulated = false;
        }
        else
        {
            pauseMenu.gameObject.SetActive(false);
            rb.velocity = new Vector2(0, 0);
            rb.simulated = true;
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void UpdatePlayerSpawn()
    {
        playerSpawn.transform.position = gameObject.transform.position;
    }
}
