using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrbSpawner : MonoBehaviour
{
    [SerializeField] private GameObject orbPrefab;

    [SerializeField] private float orbHealthRestored;

    //Cooldown timer for respawning a used orb
    [SerializeField] private float respawnCooldown;
    private float respawnTimer = 0f;
    private bool orbIsGone;


    // Start is called before the first frame update
    void Start()
    {
        SpawnOrb();
    }

    // Update is called once per frame
    void Update()
    {
        //If orb is gone, ticks up the respawn timer
        if(orbIsGone)
        {
            respawnTimer += (1f * Time.deltaTime);
            if(respawnTimer >= respawnCooldown)
            {
                respawnTimer = 0f;
                orbIsGone = false;
                SpawnOrb();
            }
        }        
    }

    //Creates a new angel at this spawner's position, passing in the variables
    void SpawnOrb()
    {
        GameObject newOrb = Instantiate(orbPrefab, this.transform);
        newOrb.GetComponent<HealthOrb>().SetVariables(this, orbHealthRestored);
    }

    //Alert sent by the spawned angel on death
    public void DeathAlert()
    {
        orbIsGone = true;
    }

}
