using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAngelSpotlight : MonoBehaviour
{

    [SerializeField] private GameObject target;
    private PlayerController playerController;

    [SerializeField] private Vector3 angelPlane;

    [SerializeField] private GameObject bigAngel;

    [SerializeField] private BigAngel bigAngelScript;

    [SerializeField] private GameObject spotlightCenter;

    [SerializeField] private float patrolSpeed;

    [SerializeField] private float pursuitSpeed;

    private float currentSpeed;

    private Vector3 destination = new Vector3(0f, 0f, 0f);

    private bool patrolMode;

    private bool pursuitMode = false;

    private PatrolNode[] currentPatrolNodes;
    private PatrolNode targetPatrolNode; 

    [SerializeField] private float offGroundModifier = -250f;

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
        pursuitMode = false;
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

            spotlightCenter.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y + offGroundModifier), this.transform.position.z);
            
        }

        //Whilst patrolling, destination is set to a random point near the angel
        if(patrolMode && currentPatrolNodes != null)
        {
            Vector3 distanceToDestination = (destination - this.transform.position);

            if(distanceToDestination.magnitude <= 0.5f)
            {
                StartPatrolToNextNode();
            }

        }
        else if(pursuitMode)
        {
            AdjustDestinationToAngelPlane(target.transform.position);     
        }   

        MoveToDestination();
    }

    public void SetAngelPlane(Vector3 plane)
    {
        angelPlane = plane;
    }

    //Adjusts any given position to account for the offset of the angel's designated navmesh plane, setting the new destination
    void AdjustDestinationToAngelPlane(Vector3 d)
    {
        destination = new Vector3(d.x, angelPlane.y, d.z);   

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
        Vector3 directionToDestination = new Vector3();
        directionToDestination = (destination - this.transform.position);

        this.transform.Translate(directionToDestination.normalized * (currentSpeed * Time.deltaTime));

        this.transform.position = new Vector3(this.transform.position.x, angelPlane.y, this.transform.position.z);
    }

    public void SetPatrolMode()
    {
        pursuitMode = false;
        patrolMode = true;

        currentSpeed = patrolSpeed;
    }

    public void SetPursuitMode()
    {
        pursuitMode = true;
        patrolMode = false;

        currentSpeed = pursuitSpeed;
    }

    public void GiveNewPatrol(GameObject spotlightPatrolPath)
    {
        currentPatrolNodes = spotlightPatrolPath.GetComponentsInChildren<PatrolNode>();
        targetPatrolNode = currentPatrolNodes[0];
        AdjustDestinationToAngelPlane(targetPatrolNode.gameObject.transform.position);
    }

    private void StartPatrolToNextNode()
    {
        if(targetPatrolNode.GetNextNode() != null)
        {   
            targetPatrolNode = targetPatrolNode.GetNextNode();
            AdjustDestinationToAngelPlane(targetPatrolNode.gameObject.transform.position);
        }        
    }
}
