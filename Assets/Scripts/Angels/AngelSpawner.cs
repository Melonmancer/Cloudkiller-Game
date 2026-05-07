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
    [SerializeField] private float angelDisguiseDamage;

    //Used only by Snitches
    [SerializeField] private float angelBigAngelAlertRange;


    //Cooldown timer for respawning a dead angel
    [SerializeField] private float respawnCooldown;
    private float respawnTimer = 0f;
    private bool angelIsDead;

    // Start is called before the first frame update
    void Start()
    {
        angelIsDead = true;
        respawnTimer = respawnCooldown;
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

                if(angelPrefab.transform.GetChild(0).GetComponent<SmallAngel>() != null)
                {
                    SpawnAngel();            
                }
                else if(angelPrefab.transform.GetChild(0).GetComponent<SnitchAngel>() != null)
                {
                    SpawnSnitchAngel();
                }
                else
                {
                    Debug.Log("Error! Spawner has no assigned angel prefab.");
                }
            }
        }
    }

    //Creates a new angel at this spawner's position, passing in the variables
    void SpawnAngel()
    {
        GameObject newAngel = Instantiate(angelPrefab, this.transform);
        newAngel.transform.GetChild(0).GetComponent<SmallAngel>().SetVariables(this, angelTarget, angelHealth, angelDamage, angelSpeed, angelChaseDistance, 
                angelAttackCooldown, angelMaxWaitTime, angelSpotSpeed, angelDisguiseDamage);
    }

    void SpawnSnitchAngel()
    {
        GameObject newAngel = Instantiate(angelPrefab, this.transform);
        newAngel.transform.GetChild(0).GetComponent<SnitchAngel>().SetVariables(this, angelTarget, angelHealth, angelSpeed, angelChaseDistance, 
                angelMaxWaitTime, angelSpotSpeed, angelDisguiseDamage, angelBigAngelAlertRange);        
    }

    //Alert sent by the spawned angel on death
    public void DeathAlert()
    {
        angelIsDead = true;
    }
}