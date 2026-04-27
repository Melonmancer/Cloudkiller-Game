using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAngelSpotlight : MonoBehaviour
{

    [SerializeField] private GameObject target;
    private PlayerController playerController;

    [SerializeField] private GameObject angelPlane;

    [SerializeField] private GameObject spotlightCenter;

    [SerializeField] private float leashRange;

    private GameObject parentAngel;

    private UnityEngine.AI.NavMeshAgent agent;
    
    private bool spotlightOnFloor;

    //A layer mask makes sure that only objects in the 'obstacle' layer count as the floor of the level
    private LayerMask lm;

    // Start is called before the first frame update
    void Start()
    {
        //If target is not set, tries to find the player in the scene and set it as target
        if(target == null)
        {
            FindPlayerTarget();
        }
        //If it already has the target set, gets its player component
        //NOTE: At the moment the target should always be the player, but we can rework this to make the angel target anything else, theoretically.
        else
        {
            playerController = target.GetComponent<PlayerController>();
        }

        //Spotlight moves using a navMeshAgent
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();   

        //Sets layer mask to grab only 'Obstacle' layer
        lm = LayerMask.GetMask("Obstacle");

        parentAngel = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(this.transform.position, Vector3.down, out hit, Mathf.Infinity, lm))
        {
            Debug.DrawRay(this.transform.position, Vector3.down * hit.distance, Color.grey);

            spotlightOnFloor = true;

            spotlightCenter.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y - hit.distance), this.transform.position.z);
        }
        else
        {
            spotlightOnFloor = false;

            spotlightCenter.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y - 100f), this.transform.position.z);
        }

        Vector3 distanceFromParent = parentAngel.transform.position - this.transform.position;
        if(distanceFromParent.magnitude > leashRange)
        {
            //Debug.Log("Too far from big angel! New position: " + distanceFromParent);

            agent.velocity = Vector3.zero;
            //this.transform.position = distanceFromParent;
        }

        if(spotlightOnFloor)
        {
            AdjustDestinationToAngelPlane(target.transform.position);     
        }
    }



    //Adjusts any given position to account for the offset of the angel's designated navmesh plane, setting the new destination
    void AdjustDestinationToAngelPlane(Vector3 destination)
    {
        agent.destination = new Vector3(destination.x, angelPlane.transform.position.y, destination.z);   
    }

    
    //Looks for an object in the scene with the player tag - if found, sets it as the target for the NavMeshAgent to use
    private bool FindPlayerTarget()
    {
        target = GameObject.FindWithTag("Player");
        if(target != null)
        {
            playerController = target.GetComponent<PlayerController>();
            return true;
        }
        else
        {
            return false;
        }
    }
}
