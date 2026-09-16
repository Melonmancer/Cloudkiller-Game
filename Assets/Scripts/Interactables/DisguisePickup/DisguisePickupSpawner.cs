using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisguisePickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private float disguiseRestored;

    //Cooldown timer for respawning a used orb
    [SerializeField] private float respawnCooldown;
    private float respawnTimer = 0f;
    private bool pickupGone = false;

    public ParticleSystem disguisePickupEffect;

    private GameObject currentPickup;


    // Start is called before the first frame update
    void Start()
    {
        SpawnOrb();
    }

    // Update is called once per frame
    void Update()
    {
        //If orb is gone, ticks up the respawn timer
        //If respawnCooldown is set to 0, does not automatically respawn
        if(pickupGone && respawnCooldown != 0f)
        {
            respawnTimer += (1f * Time.deltaTime);
            if(respawnTimer >= respawnCooldown)
            {
                respawnTimer = 0f;
                pickupGone = false;
                SpawnOrb();
            }
        }        
    }

    //Creates a new angel at this spawner's position, passing in the variables
    void SpawnOrb()
    {
        GameObject newPickup = Instantiate(prefab, this.transform);
        newPickup.GetComponent<DisguisePickup>().SetVariables(this, disguiseRestored);
        currentPickup = newPickup;
    }

    //Alert sent by the spawned angel on death
    public void DeathAlert()
    {
        pickupGone = true;

        // Spawn particle effect at apples position
        ParticleSystem effect = Instantiate(
            disguisePickupEffect,
            transform.position,
            Quaternion.identity
        );

        effect.Play();

        // Destroy effect after it finishes
        Destroy(effect.gameObject, effect.main.duration);
    }

    private void DestroyCurrentPickup()
    {
        Destroy(currentPickup);
    }

    public void ResetSpawn()
    {
        Destroy(currentPickup);
        SpawnOrb();
    }
}
