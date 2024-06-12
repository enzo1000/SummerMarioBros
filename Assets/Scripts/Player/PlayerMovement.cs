using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;                 // R�f�rence au Rigidbody2D
    private Animator anim;                  // R�f�rence � l'Animator
    private GroundedTest groundedTester;    //R�f�rence au script GroundedTest

    public float speed;                 // Vitesse de d�placement   
    public float jumpHeight = 6f;       // Hauteur d�sir�e pour le saut
    public float timeToJumpApex = 0.6f; // Temps pour atteindre le point le plus haut du saut

    private float gravity;          // Gravit� calcul�e
    private float jumpVelocity;     // V�locit� de saut calcul�e

    private float Move;             // Stocke la valeur de l'axe horizontal
    private bool isFacingRight;     // Stocke la direction du joueur
    private bool jump = false;      // Stocke l'�tat du saut

    //Stocke les limites du monde pour �viter que le joueur ne sorte de la zone de jeu
    private float WorldminBound;    
    private float WorldmaxBound;

    void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Calcul de la gravit� et de la v�locit� de saut bas�es sur la hauteur de saut et le temps pour atteindre l'apex
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        groundedTester = GetComponentInChildren<GroundedTest>(); // R�cup�re le script GroundedTest attach� � l'enfant du joueur

        WorldminBound = DataToStore.instance.LevelCompoCol2D.bounds.min.x;
        WorldmaxBound = DataToStore.instance.LevelCompoCol2D.bounds.max.x;
    }

    void Update()
    {
        Move = Input.GetAxis("Horizontal");

        DataToStore.instance.UpdatePlayerMovement(Move);
        DataToStore.instance.UpdateGroundCheckData(groundedTester.isGrounded);

        if (groundedTester.isGrounded) {        //Si notre personnage est au sol alors

            if (Input.GetKey(KeyCode.Space)) {  //Si notre joueur a la touche espace enfonc�e
                jump = true;                    // Saut
                DataToStore.instance.jumpingData();
            }
        }

        anim.SetBool("isRunning", Mathf.Abs(Move) > 0);         // Animation de course
        anim.SetBool("isJumping", !groundedTester.isGrounded);  // Animation de saut

        if ((!isFacingRight && Move > 0) || (isFacingRight && Move < 0))
        {
            Flip(); // Retourne le joueur si n�cessaire
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

        // Applique la gravit� calcul�e lorsque le joueur n'est pas au sol
        if (!groundedTester.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + gravity * Time.fixedDeltaTime); // Applique la gravit�
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
        rb.velocity = new Vector2(0, 0); // On arr�te le joueur
        anim.SetBool("isRunning", false); // On arr�te l'animation de course
    }

}
