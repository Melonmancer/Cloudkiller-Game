using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    private float health;

    [SerializeField] private GameObject respawnPoint;

    //speed manages how fast the player moves at base - manually modify this to speed up or slow down the player character
    //rotationSpeed manages how quickly the player object rotates to match the direction the camera is pointing
    //meshRotationSpeed affects how quickly the mesh rotates to face a new direction (purely aesthetic)
    [SerializeField] private float maxSpeed = 1f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float jumpHeight = 9f;
    [SerializeField] private float rotationSpeed = 0.3f;
    [SerializeField] private float meshRotationSpeed = 7.5f;

    private float speed = 0f;
    
    //Player inputs stored in these variables
    private float horizontalInput;
    private float verticalInput;
    private float jumpInput;
    private float fireInput;

    private bool disguiseInput;


    //Controls player's ability to jump slightly after leaving a platform
    private bool coyoteTime = false;
    private float coyoteWindow = 0f;
    [SerializeField] private float coyoteMax = 8f;

    //If player is touching the ground or not
    private bool grounded = true;

    private LayerMask lm;


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
    [SerializeField] private Camera playerCamera;

    private Rigidbody playerRigidbody;

    [SerializeField] private GameObject playerMesh;
    private bool spinningMesh = false;

    [SerializeField] private GameObject disguiseMesh;
    [SerializeField] private GameObject hairMesh;

    [SerializeField] private float disguiseHealthMax;
    [SerializeField] private float startingDisguiseHealth = 0;
    private float disguiseHealth = 0;
    private bool isDisguised = false;

    public ParticleSystem disguiseDepletedEffect;
    public ParticleSystem disguiseActiveEffect;
    public ParticleSystem disguiseInactiveEffect;

    private AnimationController AnimationController;

    [SerializeField] private float outOfBoundsFloor;

    private InGameUI ui;
   

    // Start is called before the first frame update
    void Start()
    {
        //Locks mouse cursor whilst game is active
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Gets the player's Rigidbody collider - there should always be one attached to the player object!
        playerRigidbody = GetComponent<Rigidbody>();

        health = maxHealth;

        disguiseHealth = startingDisguiseHealth;

        lm = LayerMask.GetMask("Obstacle", "ObstacleNoSpotlight");

        AnimationController = GetComponentInChildren<AnimationController>();

        ui = GameObject.FindWithTag("GameManager").GetComponent<InGameUI>();
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

        //'E' toggles disguise on or off, if player has charged it up
        if(disguiseInput)
        {
            ToggleDisguise();
        }

    }

    //FixedUpdate should be used for all rigidbody movement and collision work
    void FixedUpdate()
    {
        CheckIfFalling();

        //All axis-based player controls (movement and attacking) are executed in here.
        ProcessPlayerMovement();


        //If the player is supposed to be facing a certain direction, spin the mesh closer to this direction
        if(spinningMesh)
        {
            //Adjusts the mesh to face whichever way the camera is facing
            playerMesh.transform.forward = Vector3.Slerp(playerMesh.transform.forward, viewDirection.normalized, meshRotationSpeed);

            if(Vector3.Distance(playerMesh.transform.forward, viewDirection.normalized) <= 0.05f)
            {
                spinningMesh = false;
            }
        }  
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

        disguiseInput = Input.GetButtonDown("Disguise");
    }

    //Resets axis-based input data
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

        //Creates a new direction based on which keys the player is pressing
        var verticalRotation = Quaternion.Euler(0,  transform.rotation.eulerAngles.y, 0);
        direction = (verticalRotation * new Vector3(horizontalInput, 0f, verticalInput)).normalized;


        //While the player is moving they rotate to face the direction the camera is facing
        if(horizontalInput != 0.0f || verticalInput != 0.0f)
        {
            //Stop trying to spin the mesh if it is currently doing so (UpdatePlayerDirection sets it properly)
            spinningMesh = false;

            //Update the player object and meshes' facings
            UpdatePlayerDirection();
        }


        //Moves the player object in the new direction
        //playerObject.transform.Translate(direction * speed);
        var prevVerticalVelocity = playerRigidbody.velocity.y;

        if(direction == Vector3.zero)
        {
            speed = Mathf.Lerp(speed, 0f, Time.deltaTime * acceleration);
        }
        else
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
        }
        playerRigidbody.velocity = direction * speed; 
        playerRigidbody.velocity += prevVerticalVelocity * Vector3.up;

        if (jumpInput > 0)
        {
            if(grounded || coyoteTime)
            {
                grounded = false;
                coyoteTime = false;
                coyoteWindow = 0f;           
                //Debug.Log("Jumping!");
                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);
                playerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            }
        }


        //coyoteTime allows the player to jump immediately after leaving a platform - raise coyoteMax for a bigger window
        if(coyoteTime)
        {
            coyoteWindow += 1f;
            //Debug.Log(coyoteWindow);
        }
        if((coyoteTime && (coyoteWindow >= coyoteMax)) || grounded)
        {
            coyoteTime = false;
            coyoteWindow = 0f;
        }


        if(fireInput > 0f && canAttack)
        {
            PlayerAttack();
        }

        //If player is out of world, respawn
        if(transform.position.y <= outOfBoundsFloor)
        {
            RespawnPlayer();
        }

        ResetPlayerInputs();
    }



    //UpdatePlayerDirection interpolates the player object to face towards the direction the camera is facing
    private void UpdatePlayerDirection()
    {
        //Subtracting the camera's position along the X and Z axes from the player's position yields the direction the camera is facing (excluding the camera's tilt up or down)
        viewDirection = transform.position - new Vector3(playerCamera.transform.position.x, transform.position.y, playerCamera.transform.position.z);

        //Interpolates between the direction the player is currently facing and the direction the camera is facing, and sets the player object to face this new direction
        transform.forward = Vector3.Slerp(transform.forward, viewDirection.normalized, rotationSpeed);

        //Adjusts the mesh to face whichever way the player object is moving
        playerMesh.transform.forward = Vector3.Slerp(playerMesh.transform.forward, direction, rotationSpeed);
    }

    //Immediately sets player object to face the direction of the camera - used when attacking
    private void SnapPlayerDirection()
    {
        Vector3 oldFacing = playerMesh.transform.forward;

        //Snaps the player object to face whichever way the camera is facing
        viewDirection = transform.position - new Vector3(playerCamera.transform.position.x, transform.position.y, playerCamera.transform.position.z);
        transform.forward = viewDirection.normalized;

        playerMesh.transform.forward = oldFacing;
    }

    void CheckIfFalling()
    {
        RaycastHit hit;
        
        if(!Physics.Raycast(transform.position, Vector3.down, out hit, 0.99f, lm))
        {
            if(grounded)
            {
                coyoteTime = true;
            }
            grounded = false;
            //Debug.Log("Not grounded!");
        }
        else
        {
            grounded = true;
            //Debug.Log("Grounded!");
        }
    }

    //Creates an attack object - this should be linked to a prefab that has the attack script somewhere inside it.
    //Turns the player to face wherever the attack is coming from
    void PlayerAttack()
    {
        canAttack = false;
        attackTimer = 0f;

        //Makes player object face the attack direction
        SnapPlayerDirection();

        //Causes mesh to spin towards the attack direction
        spinningMesh = true;

        Instantiate(attack, this.transform);
        AnimationController.PlayAttackAnimation();
    }

    public void ChangeDisguiseHealth(float val)
    {
        disguiseHealth += val;
        if(disguiseHealth > disguiseHealthMax)
        {
            disguiseHealth = disguiseHealthMax;
            //Debug.Log("Disguise at max health!");
        }
        if(disguiseHealth <= 0)
        {
            //Debug.Log("Disguise depleted!");

            disguiseHealth = 0;
            isDisguised = false;
            disguiseMesh.SetActive(false);
            hairMesh.SetActive(true);

            // Spawn particle effect at player's position
            ParticleSystem effect = Instantiate(
                disguiseDepletedEffect,
                transform.position + Vector3.down * 0.9f,
                Quaternion.identity, 
                transform
            );

            effect.Play();

            Destroy(effect.gameObject, effect.main.duration);
        }


        //Debug.Log("Disguise health: " + disguiseHealth);
    }

    public bool GetIsDisguised()
    {
        return isDisguised;
    }

    //Add all other respawn work in here (clear stored disguise material, etc.)
    private void RespawnPlayer()
    {
        ui.FadeDeathScreen();

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

    //Toggles the player's disguise
    //Only activates the disguise if it has charge
    private void ToggleDisguise()
    {
        if(isDisguised)
        {
            isDisguised = false;
            disguiseMesh.SetActive(false);
            hairMesh.SetActive(true);
            //Debug.Log("Undisguised!");

            // Spawn particle effect at player's position
                ParticleSystem effect = Instantiate(
                disguiseInactiveEffect,
                transform.position + Vector3.down * 0.9f,
                Quaternion.identity,
                transform
                );

                effect.Play();

                Destroy(effect.gameObject, effect.main.duration);
        }
        else
        {
            if(disguiseHealth > 0)
            {
                isDisguised = true;
                disguiseMesh.SetActive(true);
                hairMesh.SetActive(false);
                //Debug.Log("Disguised!");

                // Spawn particle effect at player's position
                ParticleSystem effect = Instantiate(
                    disguiseActiveEffect,
                    transform.position + Vector3.down * 0.9f,
                    Quaternion.identity,
                    transform
                );

                effect.Play();

                Destroy(effect.gameObject, effect.main.duration);
            }
            else
            {
                Debug.Log("Cannot disguise! Disguise has ran out!");
            }
        }
    }


    public bool DamagePlayer(float damage)
    {
        health -= damage;
        //Debug.Log("DAMAGED PLAYER! " + health + " health remaining!");


        //If the player is at 0 hp, they need to be respawned. FINISH THIS!!
        if(health <= 0)
        {
            //Debug.Log("PLAYER IS DEAD!");
            RespawnPlayer();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ChangeHealth(float hp)
    {
        health += hp;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        //Debug.Log("Health changed! New health: " + health);
    }

    public float[] GetHealthValues()
    {
        float[] values = new float[2];
        values[0] = health;
        values[1] = maxHealth;

        return values;
    }

    public float GetDisguiseHealth()
    {
        return disguiseHealth;
    }

    public void SetRespawnPoint(GameObject newPoint)
    {
        respawnPoint = newPoint;
    }

    public Vector3 GetPlayerVelocity()
    {
        return playerRigidbody.velocity;
    }

    public bool GetGrounded()
    {
        return grounded;
    }
} 