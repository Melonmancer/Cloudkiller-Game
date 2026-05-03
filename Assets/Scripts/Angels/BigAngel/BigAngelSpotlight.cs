using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAngelSpotlight : MonoBehaviour
{

    [SerializeField] private GameObject target;
    private PlayerController playerController;

    [SerializeField] private GameObject angelPlane;

    [SerializeField] private GameObject bigAngel;

    [SerializeField] private BigAngel bigAngelScript;

    [SerializeField] private GameObject spotlightCenter;

    [SerializeField] private float leashRange;

    [SerializeField] private float patrolSpeed;

    [SerializeField] private float pursuitSpeed;

    private float currentSpeed;

    private Vector3 destination;

    private bool patrolMode;

    private bool reachedPatrolPoint = true;

    private bool pursuitMode = false;

    //private UnityEngine.AI.NavMeshAgent agent;
    
    //private bool spotlightOnFloor;

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
        //agent = GetComponent<UnityEngine.AI.NavMeshAgent>();   

        //Sets layer mask to grab only 'Obstacle' layer
        lm = LayerMask.GetMask("Obstacle");

        patrolMode = true;
        currentSpeed = patrolSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(this.transform.position, Vector3.down, out hit, Mathf.Infinity, lm))
        {
            Debug.DrawRay(this.transform.position, Vector3.down * hit.distance, Color.grey);

            //spotlightOnFloor = true;

            spotlightCenter.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y - hit.distance), this.transform.position.z);
        }
        else
        {
            //spotlightOnFloor = false;

            spotlightCenter.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y - 100f), this.transform.position.z);
        }

        //Whilst patrolling, destination is set to a random point near the angel
        if(patrolMode)
        {
            if(reachedPatrolPoint)
            {
                AdjustDestinationToAngelPlane(bigAngelScript.CreatePointOfInterest(leashRange));
                reachedPatrolPoint = false;
            }
            else
            {
                Vector3 distanceToDestination = new Vector3();
                distanceToDestination = (destination - this.transform.position);
                if(distanceToDestination.magnitude <= 0.5f)
                {
                    reachedPatrolPoint = true;
                }
            }
        }
        else
        {
            AdjustDestinationToAngelPlane(target.transform.position);     
        }   

        MoveToDestination();
    }



    //Adjusts any given position to account for the offset of the angel's designated navmesh plane, setting the new destination
    void AdjustDestinationToAngelPlane(Vector3 d)
    {
        destination = new Vector3(d.x, angelPlane.transform.position.y, d.z);   

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

    private void MoveToDestination()
    {
        Vector3 directionToAngel = new Vector3();
        directionToAngel = (bigAngel.transform.position - this.transform.position);

        Vector3 directionToDestination = new Vector3();
        directionToDestination = (destination - this.transform.position);

        this.transform.Translate(directionToDestination.normalized * (currentSpeed * Time.deltaTime));


        if(directionToAngel.magnitude > leashRange)
        {
            //Debug.Log("Too far! " + directionToAngel.magnitude);
            this.transform.Translate(directionToAngel.normalized * (currentSpeed * Time.deltaTime));

            //If spotlight is outside of leash range whilst patrolling, it probably cannot reach the current patrol point anymore - this makes a new one.
            if(patrolMode)
            {
                reachedPatrolPoint = true;
            }
        }


        this.transform.position = new Vector3(this.transform.position.x, angelPlane.transform.position.y, this.transform.position.z);
    }

    public void ToggleMode()
    {
        if(patrolMode)
        {
            patrolMode = false;
            pursuitMode = true;

            currentSpeed = pursuitSpeed;
        }
        else
        {
            pursuitMode = false;
            patrolMode = true;

            currentSpeed = patrolSpeed;
        }
    }
}
