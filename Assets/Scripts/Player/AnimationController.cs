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

    //How long an attack click stays queued before it's cleared, so a click the animator can't use yet doesn't fire a late attack
    [SerializeField] private float attackBufferTime = 0.2f;
    private float attackRequestTime;

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
        if (animator == null)
        {
            return;
        }

        //Clears an attack click that's been waiting too long
        if (animator.GetBool("attack") && Time.time - attackRequestTime > attackBufferTime)
        {
            animator.ResetTrigger("attack");
        }

        //Hides the dagger if no attack is playing, in case the HideWeapon event was skipped
        if (daggerMesh.activeSelf && !IsPlayingState(0, "attack") && !IsPlayingState(1, "attackRun"))
        {
            HideWeapon();
        }

        //Keeps the upper body attack layer on while an attack is playing, so stopping mid-swing doesn't cut it off
        if (IsPlayingState(1, "attackRun") || animator.IsInTransition(1))
        {
            animator.SetLayerWeight(1, 1);
        }
        else if (animationSpeed > movementThreshold)
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
            attackRequestTime = Time.time;
        }
    }

    //True if the layer is in the named state, or blending into it
    private bool IsPlayingState(int layer, string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
        {
            return true;
        }
        return animator.IsInTransition(layer) && animator.GetNextAnimatorStateInfo(layer).IsName(stateName);
    }

    public void PlayDeathAnimation()
    {
        if(animator != null)
        {
            animator.SetFloat("health", 0f);
        }
    }

    public void ResetDeathAnimation()
    {
        if(animator != null)
        {
            animator.SetFloat("health", 1f);
        }
    }
}
