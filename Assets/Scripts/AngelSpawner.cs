using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelSpawner : MonoBehaviour
{
    //The angel prefab spawned by this spawner
    [SerializeField] private GameObject angelPrefab;

    //These details are passed onto the angels spawned
    [SerializeField] private GameObject angelTarget;
    [SerializeField] private float angelHealth;
    [SerializeField] private float angelDamage;
    [SerializeField] private float angelSpeed;
    [SerializeField] private float angelChaseDistance;
    [SerializeField] private float angelAttackCooldown;
    [SerializeField] private float angelMaxWaitTime;
    [SerializeField] private float angelSpotSpeed;


    //Cooldown timer for respawning a dead angel
    [SerializeField] private float respawnCooldown;
    private float respawnTimer = 0f;
    private bool angelIsDead;

    // Start is called before the first frame update
    void Start()
    {
        SpawnAngel();
    }

    // Update is called once per frame
    void Update()
    {
        //If angel is dead, ticks up the respawn timer
        if(angelIsDead)
        {
            respawnTimer += (1f * Time.deltaTime);
            if(respawnTimer >= respawnCooldown)
            {
                respawnTimer = 0f;
                angelIsDead = false;
                SpawnAngel();
            }
        }
    }

    //Creates a new angel at this spawner's position, passing in the variables
    void SpawnAngel()
    {
        GameObject newAngel = Instantiate(angelPrefab, this.transform);
        newAngel.transform.GetChild(0).GetComponent<SmallAngel>().SetVariables(this, angelTarget, angelHealth, angelDamage, angelSpeed, angelChaseDistance, angelAttackCooldown, angelMaxWaitTime, angelSpotSpeed);
    }

    //Alert sent by the spawned angel on death
    public void DeathAlert()
    {
        angelIsDead = true;
    }
}
