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
    [SerializeField] private float angelVisionConeRange;
    [SerializeField] private float angelVisionConeAngle;

    //Used only by Snitches
    [SerializeField] private float angelBigAngelAlertRange;

    private Vector3 angelHomePoint;

    private GameObject currentAliveAngel;


    //Cooldown timer for respawning a dead angel
    //Setting this to 0 disables automatic respawn
    [SerializeField] private float respawnCooldown;
    private float respawnTimer = 0f;
    private bool angelIsDead = false;

    // Start is called before the first frame update
    void Start()
    {
        angelHomePoint = transform.GetChild(0).transform.position;
        
        MakeNewAngel();
    }

    // Update is called once per frame
    void Update()
    {
        //If angel is dead, ticks up the respawn timer
        if(angelIsDead && respawnCooldown != 0f)
        {
            respawnTimer += (1f * Time.deltaTime);
            if(respawnTimer >= respawnCooldown)
            {
                respawnTimer = 0f;
                angelIsDead = false;

                MakeNewAngel();
            }
        }
    }

    private void MakeNewAngel()
    {
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

    //Creates a new angel at this spawner's position, passing in the variables
    private void SpawnAngel()
    {
        GameObject newAngel = Instantiate(angelPrefab, this.transform);
        newAngel.transform.GetChild(0).GetComponent<SmallAngel>().SetVariables(this, angelTarget, angelHealth, angelDamage, angelSpeed, angelChaseDistance, 
                angelAttackCooldown, angelMaxWaitTime, angelSpotSpeed, angelDisguiseDamage, angelHomePoint, angelVisionConeRange, angelVisionConeAngle);
        currentAliveAngel = newAngel.transform.GetChild(0).gameObject;
    }

    private void SpawnSnitchAngel()
    {
        GameObject newAngel = Instantiate(angelPrefab, this.transform);
        newAngel.transform.GetChild(0).GetComponent<SnitchAngel>().SetVariables(this, angelTarget, angelHealth, angelSpeed, angelChaseDistance, 
                angelMaxWaitTime, angelSpotSpeed, angelDisguiseDamage, angelBigAngelAlertRange, angelHomePoint);    
        currentAliveAngel = newAngel.transform.GetChild(0).gameObject; 
    }

    //Alert sent by the spawned angel on death
    public void DeathAlert()
    {
        angelIsDead = true;
        currentAliveAngel = null;
    }

    public GameObject GetCurrentAngel()
    {
        return currentAliveAngel;
    }

    private void DestroyCurrentAngel()
    {
        Destroy(currentAliveAngel);
    }


    public void ResetSpawn()
    {
        DestroyCurrentAngel();
        MakeNewAngel();
    }
}