using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public LayerMask groundLayer;
    private Animator anim;

    public float speed;
    public float gravity;
    public float jumpVelocity = 20;

    private float Move;
    private Vector2 velocity;
    
    private bool isFacingRight;
    public bool isGrounded = true;

    void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        velocity = rb.velocity;
    }

    void Update()
    {


        Move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(Move * speed, rb.velocity.y);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            velocity.y = jumpVelocity;
            rb.velocity = new Vector2(rb.velocity.x, velocity.y);
            isGrounded = false;

        }
        //Animation
        if (Move != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (!isFacingRight && Move > 0)
        {
            Flip();
        }
        else if (isFacingRight && Move < 0)
        {
            Flip();
        }
    }


    void FixedUpdate()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
            rb.velocity = new Vector2(rb.velocity.x, velocity.y);
        }
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
           isGrounded = true;
        }

    }

    

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

}