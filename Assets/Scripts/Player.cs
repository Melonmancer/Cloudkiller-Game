using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public PlayerController playerController;

    private Rigidbody playerRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay()
    {
        if(!playerController.GetGrounded())
        {
            if(playerRigidbody.GetPointVelocity(this.transform.position) == Vector3.zero)
            {
                //Debug.Log("Grounded!");
                playerController.SetGrounded(true);
            }
        }
    }

    void OnCollisionExit()
    {
        if(playerController.GetGrounded())
        {
            if(playerRigidbody.GetPointVelocity(this.transform.position) != Vector3.zero)
            {
                //Debug.Log("Falling!");
                playerController.SetGrounded(false);
            }            
        }
    }
}
