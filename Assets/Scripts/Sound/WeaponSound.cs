using UnityEngine;

public class WeaponSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] attackHitArray;
    [SerializeField] private float attackHitVolume = 1f;

    [SerializeField] private AudioSource attackHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.CompareTag("KillableEnemy"))
            {
                PlayHitSound();
            }      
        }    
    }

    public void PlayHitSound()
    {
        int randomIndex = Random.Range(0, attackHitArray.Length);
        AudioClip clip = attackHitArray[randomIndex];

        attackHit.PlayOneShot(clip, attackHitVolume);
        Debug.Log("Played hit sound");
    }
}