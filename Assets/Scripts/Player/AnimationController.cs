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
        //New run speed variable for use with animator. If animator detects horizontal or vertical input, 
        //begins transition from idle to run
        animationSpeed = new Vector3(horizontalInput, 0f, verticalInput).magnitude;

        if (animator != null)
        {
            animator.SetFloat("runSpeed", animationSpeed);
            animator.SetFloat("verticalVelocity", PlayerController.GetPlayerVelocity().y);
            animator.SetBool("grounded", PlayerController.GetGrounded());
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
    }

    public void HideWeapon()
    {
        daggerMesh.SetActive(false);
    }
}
