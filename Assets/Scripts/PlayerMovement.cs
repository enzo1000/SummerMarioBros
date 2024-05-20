using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb; // R�f�rence au Rigidbody2D
    private Animator anim; // R�f�rence � l'Animator
    private GameObject playerSpawn; // R�f�rence au point de spawn du joueur
    private GameObject pauseMenu;   // R�f�rence au menu pause

    public float speed; // Vitesse de d�placement   
    public float jumpHeight = 6f; // Hauteur d�sir�e pour le saut
    public float timeToJumpApex = 0.6f; // Temps pour atteindre le point le plus haut du saut

    private float gravity; // Gravit� calcul�e
    private float jumpVelocity; // V�locit� de saut calcul�e

    private float Move; // Stocke la valeur de l'axe horizontal
    private bool isFacingRight; // Stocke la direction du joueur
    private GroundedTest groundedTester; // R�f�rence au script GroundedTest
    private bool jump = false; // Stocke l'�tat du saut
    private bool isMovable = true; // Stocke l'�tat du mouvement
    private bool firstMovementCheck = false; //V�rifie quand le joueur bouge pour la premi�re fois
    private int timer; //Timer du niveau
    private string levelName = "Level01"; //Nom du niveau

    void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        InitDataToStoreField();

        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn"); // R�cup�re le GameObject avec le tag "PlayerSpawn"
        //On passe par un parent actif pour atteindre un enfant inactif (FindGameObjectWithTag ne peut pas le faire)
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").transform.GetChild(0).gameObject; // R�cup�re le GameObject avec le tag "PauseMenu"

        // Calcul de la gravit� et de la v�locit� de saut bas�es sur la hauteur de saut et le temps pour atteindre l'apex
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        groundedTester = GetComponentInChildren<GroundedTest>(); // R�cup�re le script GroundedTest attach� � l'enfant du joueur
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

        DirectionCheckTimer();

        if (groundedTester.isGrounded) {  //Si notre personnage est au sol alors
            UpdatePlayerSpawn();          // Met � jour le point de spawn du joueur

            if (Input.GetKey(KeyCode.Space)) {  //Si notre joueur a la touche espace enfonc�e
                jump = true;                    // Saut
            }
        }

        anim.SetBool("isRunning", Mathf.Abs(Move) > 0); // Animation de course
        anim.SetBool("isJumping", !groundedTester.isGrounded); // Animation de saut

        if ((!isFacingRight && Move > 0) || (isFacingRight && Move < 0))
        {
            Flip(); // Retourne le joueur si n�cessaire
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(Move * speed, rb.velocity.y); // D�placement horizontal

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

    public void ShowPauseMenu()
    {
        //A minima, d�sactiver les mouvements du joueur
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

    //Fonction un peu foure tout pour les timers reli� � la direction du joueur / mouvements du joueur
    void DirectionCheckTimer()
    {
        if (Move != 0)
        {
            //Temps passe sans bouger avant le premier mouvement
            if (firstMovementCheck == false)
            {
                firstMovementCheck = true;
                
                DataToStore.instance.playerTimeInfo[levelName + "FirstDeplacementTimer"] = DataToStore.instance.playerTimeInfo[levelName + "Timer"];
            }

            //Si le joueur bouge et que le timer de pause est diff�rent de 0, on stocke le temps que le joueur a pass� sans bouger
            //pr�cision, on pourrait certainement ne pas avoir � r�aliser ce if mais pour des raisons de clart�, on pr�f�rera le laisser
            if (DataToStore.instance.playerTimeInfo[levelName + "StartPause"] != 0)
            {
                float timePaused = DataToStore.instance.playerTimeInfo[levelName + "Timer"] - DataToStore.instance.playerTimeInfo[levelName + "StartPause"];
                if (timePaused > DataToStore.instance.playerTimeInfo[levelName + "MaxPause"])
                {
                    DataToStore.instance.playerTimeInfo[levelName + "MaxPause"] = timePaused;
                }
                DataToStore.instance.playerTimeInfo[levelName + "StartPause"] = 0.0f;
            }
        }
        else
        {
            //Si le joueur s'arr�te de bouger, on stocke le temps que le joueur passe sans bouger
            if (DataToStore.instance.playerTimeInfo[levelName + "StartPause"] == 0.0f)
            {
                DataToStore.instance.playerTimeInfo[levelName + "StartPause"] = DataToStore.instance.playerTimeInfo[levelName + "Timer"];
            }
        }
    }

    //Initialise les champs importants pour le stockage des donn�es du joueur (appele dans Start())
    private void InitDataToStoreField()
    {
        //Temps auquel le joueur fait son premier deplacement
        DataToStore.instance.playerTimeInfo.Add(levelName + "FirstDeplacementTimer", 0.0f);
        //Temps auquel le joueur s'arrete de bouger
        DataToStore.instance.playerTimeInfo.Add(levelName + "StartPause", 0.0f);
        //Temps de la plus grande pause sans se deplacer du joueur
        DataToStore.instance.playerTimeInfo.Add(levelName + "MaxPause", 0.0f);
    }
}
