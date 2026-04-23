using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAngel : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;

    [SerializeField] private GameObject angelPlane;

    [SerializeField] private GameObject target;
    private PlayerController playerController;

    [SerializeField] private GameObject proxyCenter;

    [SerializeField] private GameObject meshObject;


    //The target distance this should float from the nearest surface below it
    [SerializeField] private float floatingHeight;
    //The maximum height this can go along the world's y axis
    [SerializeField] private float maxWorldHeight;
    [SerializeField] private float heightChangeSpeed;
    //The default height the angel floats at if it is not over a solid surface - based on starting transform
    private float defaultFloatHeight;
    //The minimum height the angel should float from the nearest surface below it - calculated from angel's size to prevent clipping
    private float minFloatingHeight;
    private Vector3 newWorldPosition = new Vector3();


    //A layer mask used for the angel's line of sight - makes sure that only objects in the 'obstacle' layer block LOS
    private LayerMask lm;
    private LayerMask lm2;

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
        
        //All small angels should have a NavMeshAgent attached for moving and navigating!
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();   


        //Sets layer mask for the line of sight system - cannot see through anything in the obstacles layer
        lm = LayerMask.GetMask("Obstacle");
        //Another layer mask for the big angel's base - uses this to determine how high up the angel should be moved so it doesn't clip into surfaces
        lm2 = LayerMask.GetMask("BigAngelBase");

        defaultFloatHeight = proxyCenter.transform.position.y;


        RaycastHit hit;
        Physics.Raycast(proxyCenter.transform.position, Vector3.down, out hit, Mathf.Infinity, lm2);

        //Minimum floating distance is the distance between the angel's proxyCenter and base object in world units - the center should be this far from the surface below to avoid clipping!
        minFloatingHeight = hit.distance;
        //Debug.Log(hit.distance);

    }

    // Update is called once per frame
    void Update()
    {
        AdjustDestinationToAngelPlane(target.transform.position);     
    }

    void FixedUpdate()
    {
        FloatOverTerrain();
    }

    //Causes the angel's main body to float over terrain below it at the set floating height.
    void FloatOverTerrain()
    {
        RaycastHit hit;
        float newHeight;

        //Casts a ray straight down from the objects actual center (far above) colliding with the first obstacle in the way.
        if(Physics.Raycast(this.transform.position, Vector3.down, out hit, Mathf.Infinity, lm))
        {
            Debug.DrawRay(this.transform.position, Vector3.down * hit.distance, Color.gray);

            //New target height is the distance the collision occured from the angel's base object + the set floating height of the angel
            newHeight = (hit.distance * -1f) + floatingHeight;

            //New target position in world space is the base object's position + the new height
            newWorldPosition = new Vector3 (proxyCenter.transform.position.x, this.transform.position.y + newHeight, proxyCenter.transform.position.z);
        }
        //If there was no collision (i.e. the angel is not over solid ground), default to the starting height of the proxy center
        else
        {
            newHeight = -999f;
            newWorldPosition = new Vector3 (proxyCenter.transform.position.x, defaultFloatHeight, proxyCenter.transform.position.z);
        }

        //Interpolation between current proxy coordinates and intended coordinates
        Vector3 newPos = Vector3.Slerp(proxyCenter.transform.position, newWorldPosition, heightChangeSpeed);

        //If interpolation would not raise the angel above the minimum floating height, it is set to the minimum
        //NOTE: minFloatingHeight is calculated on start by getting the distance between the angel's proxy center and base in world units. This will always put the
        //base of the angel above the detected solid surface - if you want the angel to move through an indoors section, make sure the ceiling does not have the obstacle tag!
        if((newPos.y < (this.transform.position.y + (newHeight - floatingHeight + minFloatingHeight))) && newHeight != -999f)
        {
            newPos = new Vector3(newPos.x, (this.transform.position.y + (newHeight - floatingHeight + minFloatingHeight)), newPos.z);
            //Debug.Log("Raising minimum! Current: " + newPos.y);
        }
        
        //Caps new position to the given world height cap (as high as the angel can go) if it would otherwise be exceeded.
        if(newPos.y > maxWorldHeight)
        {
            newPos.y = maxWorldHeight;
        }

        //Adjusts proxy center's y position, floating the angel.
        //NOTE: Consider adjusting only the mesh object if we want the proxy to not bounce up and down whilst doing calculations
        proxyCenter.transform.position = new Vector3(proxyCenter.transform.position.x, newPos.y, proxyCenter.transform.position.z);
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
