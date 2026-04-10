using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    private float health;

    [SerializeField] private GameObject respawnPoint;

    //speed manages how fast the player moves at base - manually modify this to speed up or slow down the player character
    //rotationSpeed manages how quickly the player object rotates to match the direction the camera is pointing
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float jumpHeight = 9f;
    private float rotationSpeed = 1.5f;
    
    //Player inputs stored in these variables
    private float horizontalInput;
    private float verticalInput;
    private float jumpInput;
    private float fireInput;


    //Controls player's ability to jump slightly after leaving a platform
    private bool coyoteTime = false;
    private float coyoteWindow = 0f;
    [SerializeField] private float coyoteMax = 8f;

    //If player is touching the ground or not
    private bool grounded = true;



    //Controls attack input cooldown - should be roughly same speed as the attack object's lifespan!
    [SerializeField] private float attackCooldown = 1f;
    private float attackTimer = 0f;
    private bool canAttack = true;

    //This object is spawned when the player performs the attack input
    [SerializeField] private GameObject attack;

    //The direction the player is moving in
    private Vector3 direction;

    //The direction the camera is facing
    private Vector3 viewDirection;

    //Stores references to the player and camera objects in the same scene
    //NOTE: These should be set inside of the player prefab!
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Camera playerCamera;

    private Rigidbody playerRigidbody;


    // Start is called before the first frame update
    void Start()
    {
        //Locks mouse cursor whilst game is active
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Gets the player's Rigidbody collider - there should always be one attached to the player object!
        playerRigidbody = playerObject.GetComponent<Rigidbody>();

        health = maxHealth;
    }

    //Update called each frame update, collects player inputs
    void Update()
    {
        GetPlayerInputs();

        //If the player's attack is on cooldown, this ticks it.
        if(!canAttack)
        {
            attackTimer += (1f * Time.deltaTime);
            if(attackTimer >= attackCooldown)
            {
                canAttack = true;
                //Debug.Log("Attack off cooldown!");
            }
        }
    }

    //FixedUpdate should be used for all rigidbody movement and collision work
    void FixedUpdate()
    {
        //All player controls are executed in here.
        ProcessPlayerMovement();        
    }



    //Collects input data from the player
    void GetPlayerInputs()
    {
        //Horizontal checks for A, D, Left and Right arrow keys
        horizontalInput = Input.GetAxis("Horizontal");
        //Vertical checks for W, S, Up and Down arrow keys
        verticalInput = Input.GetAxis("Vertical");

        jumpInput = Input.GetAxis("Jump");

        fireInput = Input.GetAxis("Fire1");
    }

    //Resets input data
    void ResetPlayerInputs()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
        jumpInput = 0f;
        fireInput = 0f;
    }



    //ProcessPlayerMovement checks if the player is inputting anything, and updates the player object's position and facing
    private void ProcessPlayerMovement()
    {

         //While the player is moving they rotate to face the direction the camera is facing
        if(horizontalInput != 0.0f || verticalInput != 0.0f)
        {
            UpdatePlayerDirection();
        }

        //Creates a new direction based on which keys the player is pressing
        var verticalRotation = Quaternion.Euler(0,  playerObject.transform.rotation.eulerAngles.y, 0);
        direction = (verticalRotation * new Vector3(horizontalInput, 0f, verticalInput)).normalized;

        //Moves the player object in the new direction
        //playerObject.transform.Translate(direction * speed);
        var prevVerticalVelocity = playerRigidbody.velocity.y;
        playerRigidbody.velocity = direction * speed; 
        playerRigidbody.velocity += prevVerticalVelocity * Vector3.up;


        if (jumpInput > 0 && (grounded || coyoteTime))
        {
            grounded = false;
            coyoteTime = false;
            coyoteWindow = 0f;           
            //Debug.Log("Jumping!");
            playerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }


        //coyoteTime allows the player to jump immediately after leaving a platform - raise coyoteMax for a bigger window
        if(coyoteTime)
        {
            coyoteWindow += 1f;
            //Debug.Log(coyoteWindow);
        }
        if(coyoteTime && (coyoteWindow >= coyoteMax))
        {
            coyoteTime = false;
            coyoteWindow = 0f;
        }


        if (fireInput > 0 && canAttack)
        {
            PlayerAttack();
        }

        ResetPlayerInputs();
    }



    //UpdatePlayerDirection interpolates the player object to face towards the direction the camera is facing
    private void UpdatePlayerDirection()
    {
        //Subtracting the camera's position along the X and Z axes from the player's position yields the direction the camera is facing (excluding the camera's tilt up or down)
        viewDirection = playerObject.transform.position - new Vector3(playerCamera.transform.position.x, playerObject.transform.position.y, playerCamera.transform.position.z);

        //Interpolates between the direction the player is currently facing and the direction the camera is facing, and sets the player to face this new direction
        playerObject.transform.forward = Vector3.Slerp(playerObject.transform.forward, viewDirection.normalized, rotationSpeed * 0.2f);
    }



    //Called by the collider component
    //If the player has no vertical velocity (ie. not falling or rising) and collides with something, reset their jump
    void OnCollisionStay()
    {
        if(!grounded)
        {
            if(playerRigidbody.GetPointVelocity(this.transform.position).y == 0f)
            {
                //Debug.Log("Grounded!");
                grounded = true;
            }
        }
    }

    //Called by the collider component
    //Sets the player's falling state if they are moving and leave a collision
    void OnCollisionExit()
    {
        if(grounded)
        {
            if(playerRigidbody.GetPointVelocity(this.transform.position) != Vector3.zero)
            {
                //Debug.Log("Falling!");
                grounded = false;
                coyoteTime = true;
            }            
        }
    }

    //Creates an attack object - this should be linked to a prefab that has the attack script somewhere inside it.
    void PlayerAttack()
    {
        canAttack = false;
        attackTimer = 0f;
        Instantiate(attack, this.transform);
    }

    //Add all other respawn work in here (clear stored disguise material, etc.)
    private void RespawnPlayer()
    {
        Debug.Log("Respawning player!");

        if(respawnPoint != null)
        {
            this.gameObject.transform.position = respawnPoint.transform.position;        
            playerRigidbody.velocity = Vector3.zero;    
        }
        else
        {
            Debug.Log("ERROR! No respawn point set for player controller!");
        }

        health = maxHealth;
    }


    public bool DamagePlayer(float damage)
    {
        health -= damage;
        Debug.Log("DAMAGED PLAYER! " + health + " health remaining!");


        //If the player is at 0 hp, they need to be respawned. FINISH THIS!!
        if(health <= 0)
        {
            Debug.Log("PLAYER IS DEAD!");
            RespawnPlayer();
            return true;
        }
        else
        {
            return false;
        }
    }

}
