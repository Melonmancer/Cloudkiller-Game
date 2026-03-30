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


    void Update()
    {

    }

    //NOTE: FixedUpdate to solve for some collision issues, if too janky switch back to Update and add * 'Time.deltaTime to all movements
    //Update is called once per frame
    void FixedUpdate()
    {
        ProcessPlayerMovement();        
    }



    //ProcessPlayerMovement checks if the player is inputting anything, and updates the player object's position and facing
    private void ProcessPlayerMovement()
    {


        //Horizontal checks for A, D, Left and Right arrow keys
        float horizontal = Input.GetAxis("Horizontal");
        //Vertical checks for W, S, Up and Down arrow keys
        float vertical = Input.GetAxis("Vertical");

         //While the player is moving they rotate to face the direction the camera is facing
        if(horizontal != 0.0f || vertical != 0.0f)
        {
            UpdatePlayerDirection();
        }

        //Creates a new direction based on which keys the player is pressing
        var verticalRotation = Quaternion.Euler(0,  playerObject.transform.rotation.eulerAngles.y, 0);
        direction = (verticalRotation * new Vector3(horizontal, 0f, vertical)).normalized;

        
        
        //Moves the player object in the new direction
        //playerObject.transform.Translate(direction * speed);
        var prevVerticalVelocity = playerRigidbody.velocity.y;
        playerRigidbody.velocity = direction * speed; 
        playerRigidbody.velocity += prevVerticalVelocity * Vector3.up;

        if (Input.GetAxis("Jump") > 0 && grounded)
        {
            SetGrounded(false);
            //Debug.Log("Jumping!");
            playerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

       
    }



    //UpdatePlayerDirection interpolates the player object to face towards the direction the camera is facing
    private void UpdatePlayerDirection()
    {
        //Subtracting the camera's position along the X and Z axes from the player's position yields the direction the camera is facing (excluding the camera's tilt up or down)
        viewDirection = playerObject.transform.position - new Vector3(playerCamera.transform.position.x, playerObject.transform.position.y, playerCamera.transform.position.z);

        //Interpolates between the direction the player is currently facing and the direction the camera is facing, and sets the player to face this new direction
        playerObject.transform.forward = Vector3.Slerp(playerObject.transform.forward, viewDirection.normalized, rotationSpeed * 0.2f);
    }


    public void SetGrounded(bool boolean)
    {
        grounded = boolean;
    }

    public bool GetGrounded()
    {
        return grounded;
    }
}