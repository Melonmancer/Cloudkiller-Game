using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //speed manages how fast the player moves at base - manually modify this to speed up or slow down the player character
    //rotationSpeed manages how quickly the player object rotates to match the direction the camera is pointing
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float jumpHeight = 9f;
    private float rotationSpeed = 1.5f;
    

    private float horizontalInput;
    private float verticalInput;
    private float jumpInput;

    //Controls player's ability to jump slightly after leaving a platform
    private bool coyoteTime = false;
    private float coyoteWindow = 0f;
    [SerializeField] private float coyoteMax = 8f;

    private Vector3 direction;
    private Vector3 viewDirection;

    //Stores references to the player and camera objects in the same scene
    //NOTE: If you are making a new scene, you'll need to link these manually from the inspector
    public GameObject playerObject;
    public Camera playerCamera;

    private Rigidbody playerRigidbody;

    private bool grounded = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerRigidbody = playerObject.GetComponent<Rigidbody>();
    }

    //Update called each frame update, collects player inputs
    void Update()
    {
        GetPlayerInputs();
    }

    //FixedUpdate should be used for all rigidbody movement and collision work
    void FixedUpdate()
    {
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
    }

    //Resets input data
    void ResetPlayerInputs()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
        jumpInput = 0f;
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


}
