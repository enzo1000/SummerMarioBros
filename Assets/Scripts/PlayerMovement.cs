using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb; // Référence au Rigidbody2D
    private Animator anim; // Référence à l'Animator

    public float speed; // Vitesse de déplacement   
    public float jumpHeight = 6f; // Hauteur désirée pour le saut
    public float timeToJumpApex = 0.6f; // Temps pour atteindre le point le plus haut du saut

    private float gravity; // Gravité calculée
    private float jumpVelocity; // Vélocité de saut calculée

    private float Move; // Stocke la valeur de l'axe horizontal
    private bool isFacingRight; // Stocke la direction du joueur
    private GroundedTest groundedTester; // Référence au script GroundedTest

    void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Calcul de la gravité et de la vélocité de saut basées sur la hauteur de saut et le temps pour atteindre l'apex
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        groundedTester = GetComponentInChildren<GroundedTest>(); // Récupère le script GroundedTest attaché à l'enfant du joueur
    }

    void Update()
    {
        Move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(Move * speed, rb.velocity.y); // Déplacement horizontal

        if (Input.GetKeyDown(KeyCode.Space) && groundedTester.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity); // Saut
        }

        anim.SetBool("isRunning", Mathf.Abs(Move) > 0); // Animation de course

        if ((!isFacingRight && Move > 0) || (isFacingRight && Move < 0))
        {
            Flip(); // Retourne le joueur si nécessaire
        }
    }

    void FixedUpdate()
    {
        // Applique la gravité calculée lorsque le joueur n'est pas au sol
        if (!groundedTester.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + gravity * Time.fixedDeltaTime); // Applique la gravité
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
