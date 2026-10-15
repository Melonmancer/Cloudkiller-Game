using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioClip landing;
    [SerializeField] private AudioClip melee;
    [SerializeField] private AudioClip melee2;
    [SerializeField] private float footstepsVolume = 1f;
    [SerializeField] private float landingVolume = 1f;
    [SerializeField] private float meleeVolume = 1f;

    [SerializeField] private AudioSource playerAudio;
    private PlayerController PlayerController;

    void Start()
    {
        PlayerController = GetComponentInParent<PlayerController>();
    }

    public void PlayFootstepSound()
    {
        if (PlayerController.GetGrounded() == false)
        {
            return;
        }
        
        int randomIndex = Random.Range(0, footsteps.Length);
        AudioClip clip = footsteps[randomIndex];

        playerAudio.PlayOneShot(clip, footstepsVolume);
    }

    public void PlayLandingSound()
    {
        playerAudio.PlayOneShot(landing, landingVolume);
    }

    public void PlayMeleeSound()
    {
        playerAudio.PlayOneShot(melee, meleeVolume);
    }

    public void PlayMelee2Sound()
    {
        playerAudio.PlayOneShot(melee2, meleeVolume);
    }
}