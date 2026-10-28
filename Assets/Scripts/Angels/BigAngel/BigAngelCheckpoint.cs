using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAngelCheckpoint : MonoBehaviour
{
    //Updates the seraphim's behaviour when the linked checkpoint collider is touched (can be set to check for the player or the seraphim)

    [SerializeField] private bool checkForSeraphim = true;
    [SerializeField] private bool checkForPlayer = false;

    [SerializeField] private GameObject linkedPatrolPath;
    [SerializeField] private GameObject linkedSpotlightPatrolPath;
    [SerializeField] private BigAngelSpawner linkedBigAngelSpawner;
    [SerializeField] private float defaultFloatHeight = -999f;

    [SerializeField] private bool disabledAfterTrigger = true;

    private bool disabled = false;

    private BigAngel seraphimController;

    // Start is called before the first frame update
    void Start()
    {
        if(defaultFloatHeight == -999f)
        {
            defaultFloatHeight = linkedPatrolPath.transform.position.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(seraphimController == null)
        {
            FindSeraphimInScene();
            disabled = false;
        }
    }

    void FindSeraphimInScene()
    {
        seraphimController = GameObject.FindWithTag("BigAngel").GetComponent<BigAngel>();
    }

    void OnTriggerStay(Collider col)
    {
        bool found = false;

        if(checkForSeraphim)
        {
            if(col.tag == "BigAngel")
            {
                found = true;
            }
        }
        if(checkForPlayer)
        {
            if(col.tag == "Player")
            {
                found = true;
            }
        }

        if(found && !disabled)
        {
            Debug.Log("Hit big angel checkpoint.");

            seraphimController.GiveNewPatrol(linkedPatrolPath, linkedSpotlightPatrolPath, defaultFloatHeight);
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("BigAngelSpawner");

            foreach(GameObject s in spawners)
            {
                s.GetComponent<BigAngelSpawner>().SetIsActive(false);
            }

            linkedBigAngelSpawner.SetIsActive(true);

            if(disabledAfterTrigger)
            {
                disabled = true;
                Debug.Log("Disabled checkpoint until respawn.");
            }
        }
    }

    public void SendDirectionsToBigAngel()
    {
        FindSeraphimInScene();
        seraphimController.GiveNewPatrol(linkedPatrolPath, linkedSpotlightPatrolPath, defaultFloatHeight);
    }
}
