using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;                 // Référence au Rigidbody2D
    private Animator anim;                  // Référence à l'Animator
    private GroundedTest groundedTester;    //Référence au script GroundedTest

    public float speed;                 // Vitesse de déplacement   
    public float jumpHeight = 6f;       // Hauteur désirée pour le saut
    public float timeToJumpApex = 0.6f; // Temps pour atteindre le point le plus haut du saut

    private float gravity;          // Gravité calculée
    private float jumpVelocity;     // Vélocité de saut calculée

    private float Move;             // Stocke la valeur de l'axe horizontal
    private bool isFacingRight;     // Stocke la direction du joueur
    private bool jump = false;      // Stocke l'état du saut

    //Stocke les limites du monde pour éviter que le joueur ne sorte de la zone de jeu
    private float WorldminBound;    
    private float WorldmaxBound;

    void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Calcul de la gravité et de la vélocité de saut basées sur la hauteur de saut et le temps pour atteindre l'apex
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        groundedTester = GetComponentInChildren<GroundedTest>(); // Récupère le script GroundedTest attaché à l'enfant du joueur

        WorldminBound = DataToStore.instance.LevelCompoCol2D.bounds.min.x;
        WorldmaxBound = DataToStore.instance.LevelCompoCol2D.bounds.max.x;
    }

    void Update()
    {
        Move = Input.GetAxis("Horizontal");

        DataToStore.instance.UpdatePlayerMovement(Move);
        DataToStore.instance.UpdateGroundCheckData(groundedTester.isGrounded);

        if (groundedTester.isGrounded) {        //Si notre personnage est au sol alors

            if (Input.GetKey(KeyCode.Space)) {  //Si notre joueur a la touche espace enfoncée
                jump = true;                    // Saut
                DataToStore.instance.jumpingData();
            }
        }

        anim.SetBool("isRunning", Mathf.Abs(Move) > 0);         // Animation de course
        anim.SetBool("isJumping", !groundedTester.isGrounded);  // Animation de saut

        if ((!isFacingRight && Move > 0) || (isFacingRight && Move < 0))
        {
            Flip(); // Retourne le joueur si nécessaire
        }
    }

    void FixedUpdate()
    {
        Vector2 ProcessedVelocity = new Vector2(Move * speed, rb.velocity.y);
        predictPlayerPosition(rb.position, ProcessedVelocity);
        
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

    //Fonction pour retourner le sprite du joueur
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    //Fonction qui empeche le joueur de sortir de la zone de jeu
    private void predictPlayerPosition(Vector2 position, Vector2 ProcessedVelocity)
    {
        if (position.x + ProcessedVelocity.x * Time.deltaTime < WorldminBound + 1
            || position.x + ProcessedVelocity.x * Time.deltaTime > WorldmaxBound - 1)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Move * speed, rb.velocity.y);
        }
    }

    public void StopPlayer()
    {
        rb.velocity = new Vector2(0, 0); // On arrête le joueur
        anim.SetBool("isRunning", false); // On arrête l'animation de course
    }
}
