using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController: MonoBehaviour
{
    [SerializeField] private GameObject daggerMesh;
    [SerializeField] private Animator animator;

    private float animationSpeed;
    private float horizontalInput;
    private float verticalInput;
    private float movementThreshold = 0.1f;
    private PlayerController PlayerController;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInputs();
        UpdateAnimations();
    }

    //Handles idle/run animations
    void UpdateAnimations()
    {   
        animationSpeed = new Vector3(horizontalInput, 0f, verticalInput).magnitude;
        float verticalVelocity = PlayerController.GetPlayerVelocity().y; 
        if (animator != null)
        {
            animator.SetFloat("runSpeed", animationSpeed);
            animator.SetFloat("verticalVelocity", verticalVelocity);
            animator.SetBool("grounded", PlayerController.GetGrounded());
        }
        if (animationSpeed > movementThreshold)
        {
            animator.SetLayerWeight(1, 1);
        }
        else if (verticalVelocity > movementThreshold)
        {
            animator.SetLayerWeight(1, 1);
        }
        else
        {
            animator.SetLayerWeight(1, 0);
        }
    }

    void GetPlayerInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    public void ShowWeapon()
    {
        daggerMesh.SetActive(true);
        //Debug.Log("ShowWeapon called");
    }

    public void HideWeapon()
    {
        daggerMesh.SetActive(false);
    }

    public void PlayAttackAnimation()
    {
        if(animator != null)
        {
            animator.SetTrigger("attack");
        }
    }
}
