using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(100)]
public class PlayerHold : MonoBehaviour
{
    private PlayerController player;
    private Rigidbody body;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        bool noInput = Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && Input.GetAxis("Jump") == 0;
        bool hold = noInput && player.GetGrounded();
        body.useGravity = !hold;
        if (hold) 
        {
            body.velocity = Vector3.zero;
        }
    }
}