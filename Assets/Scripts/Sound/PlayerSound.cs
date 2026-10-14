using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepsArray;
    [SerializeField] private float footstepsVolume = 1f;

    [SerializeField] private AudioSource footsteps;
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
        
        int randomIndex = Random.Range(0, footstepsArray.Length);
        AudioClip clip = footstepsArray[randomIndex];

        footsteps.PlayOneShot(clip, footstepsVolume);
    }
}