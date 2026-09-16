using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    private GameObject[] angelSpawners;
    private GameObject[] pickupSpawners;
    private GameObject player;

    [SerializeField] private float resetDelay = 5f;
    private float resetStep = 0f;
    private bool resetting = false;

    // Start is called before the first frame update
    void Start()
    {
        angelSpawners = GameObject.FindGameObjectsWithTag("AngelSpawner");
        pickupSpawners = GameObject.FindGameObjectsWithTag("PickupSpawner");
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(resetting)
        {
            resetStep += 1f * Time.deltaTime;
            if(resetStep >= resetDelay)
            {
                resetStep = 0f;
                resetting = false;
                ResetAllEntities();
            }
        }
    }

    public void TriggerReset()
    {
        resetting = true;
    }

    private void ResetAllEntities()
    {
        foreach(GameObject angelSpawner in angelSpawners)
        {
            angelSpawner.GetComponent<AngelSpawner>().ResetSpawn();
        }
        foreach(GameObject pickupSpawner in pickupSpawners)
        {
            pickupSpawner.GetComponent<DisguisePickupSpawner>().ResetSpawn();
        }
        player.GetComponent<PlayerController>().RespawnPlayer();
    }
}
