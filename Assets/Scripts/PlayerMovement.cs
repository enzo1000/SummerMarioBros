using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform groundCheckL, groundCheckR;

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;
    private bool isJumping = false;
    private bool isGrounded;

    //FixedUpdate is called once per physics update (and not on frames)
    void FixedUpdate()
    {
        //Créer une boite de collision entre les deux points groundCheckL et groundCheckR
        isGrounded = Physics2D.OverlapArea(groundCheckL.position, groundCheckR.position);

        float horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        //Check if the player is grounded and if the player press the jump button (make him jump)
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            isJumping = true;
        }

        SpriteFlip(horizontalMovement);
        MovePlayer(horizontalMovement);
    }

    //Flip the sprite of the player depending on the direction he is moving
    // and also change the animation from idle to walk
    private void SpriteFlip(float horizontalMovement)
    {
        //Send the absolute value of the horizontal movement to the animator to change the animation (idle -> walk)
        animator.SetFloat("Speed", Mathf.Abs(horizontalMovement));
        
        //If the player is moving left, flip the sprite to the left
        if (horizontalMovement < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
        //Else, if the player is moving right, flip the sprite to the right
        else if (horizontalMovement > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
    }

    //Move the player on the x axis (smoothDamp) and make him jump (AddForce)
    private void MovePlayer(float horizontalMovement)
    {
        //On se déplace sur l'axe x, l'axe y reste inchangé (no Z axis on a 2D vector)
        Vector3 targetVelocity = new Vector2(horizontalMovement, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.05f);

        if(isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }
    }



}
