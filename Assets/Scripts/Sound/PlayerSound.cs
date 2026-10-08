using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepsArray;
    [SerializeField] private float volume = 1f;

    [SerializeField] private AudioSource footsteps;
    [SerializeField] private Animator animator;
    private PlayerController PlayerController;

    void Start()
    {
        PlayerController = GetComponentInParent<PlayerController>();
    }

    public void PlayFootstepSound()
    {
        // Optional: Avoid playing sounds if the character isn't grounded
        if (PlayerController.GetGrounded() == false)
        {
            return;
        }
        
        int randomIndex = Random.Range(0, footstepsArray.Length);
        AudioClip clip = footstepsArray[randomIndex];

        footsteps.PlayOneShot(clip, volume);
    }
}