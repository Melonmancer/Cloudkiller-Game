using UnityEngine;

public class EyeballSound : MonoBehaviour
{
    [SerializeField] private AudioClip flapUp;
    [SerializeField] private AudioClip flapDown;
    [SerializeField] private float flappingVolume = 1f;
    [SerializeField] private AudioSource eyeballAudio;

    public void PlayFlappingUpSound()
    {
        eyeballAudio.PlayOneShot(flapUp, flappingVolume);
    }

    public void PlayFlappingDownSound()
    {
        eyeballAudio.PlayOneShot(flapDown, flappingVolume);
    }
}