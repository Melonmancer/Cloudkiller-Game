using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private PlatformNode[] nodes;
    private PlatformNode targetNode;

    private Vector3 lastNodePosition;

    [SerializeField] private float platformSpeed;

    //The nodePath should be a gameObject containing objects at different positions, each with PlatformNode scripts that link to each other
    //View the sample in prefabs for an example of a node path, but you can make your own
    [SerializeField] private GameObject nodePath; 

    private float startTime;

    private bool isDelayed = false;
    private float delay;
    private float delayStep;


    // Start is called before the first frame update
    void Start()
    {
        nodes = nodePath.GetComponentsInChildren<PlatformNode>();
        targetNode = nodes[0];

        lastNodePosition = transform.position;

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDelayed)
        {
            delayStep += (1f * Time.deltaTime);
            if(delayStep >= delay)
            {
                isDelayed = false;
                startTime = Time.time;
            }
        }

        else
        {
            Vector3 dir = CalculateDirectionToTargetNode();
            //When the platform reaches the target node, the target's linked node becomes the new target node
            if(dir.magnitude <= 0.05f)
            {
                NextNode();
            }

            else
            {
                Vector3 pathBetweenNodes = new Vector3();
                pathBetweenNodes = lastNodePosition - targetNode.GetNodePosition();

                //From Unity API on linear interpolation

                //distanceInTime is the lapsed time between when it started along the path and right now
                float distanceInTime = (Time.time - startTime) * platformSpeed;
                //distanceAlongPath is the distance along the path the platform should be based on distanceInTime and the actual length of the path
                float distanceAlongPath = distanceInTime / pathBetweenNodes.magnitude;

                //Linearly interpolates between the starting node and the target node by the distance along the path the platform should be to set the actual position
                transform.position = Vector3.Lerp(lastNodePosition, targetNode.GetNodePosition(), distanceAlongPath);
            }
        }
    }

    Vector3 CalculateDirectionToTargetNode()
    {
        Vector3 directionToTarget = new Vector3();
        directionToTarget = transform.position - targetNode.GetNodePosition();

        return directionToTarget;
    }

    //Gets platform ready to go to the next node in the node path
    private void NextNode()
    {
        //If the node the platform just reached has a delay, activate it now
        if(targetNode.GetDelayBeforeMoving() > 0f)
        {
            isDelayed = true;
            delay = targetNode.GetDelayBeforeMoving();
            delayStep = 0f;
        }

        //If the node the platform just reached has a configured speed, override the platform's speed
        if(targetNode.GetSpeedToNextNode() > 0f)
        {
            platformSpeed = targetNode.GetSpeedToNextNode();
        }       

        //Sets the current node's linked node as the new target
        if(targetNode.GetNextNode() != null)
        {   
            lastNodePosition = targetNode.GetNodePosition();
            targetNode = targetNode.GetNextNode();

            startTime = Time.time;
        }
        else
        {
            Debug.Log("Error! No next node set on platform node: " + targetNode.gameObject);
        }
    }
    

    //When the player touches this platform, they become a child of the platform until they leave it, causing the platform to carry the player along
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.transform.parent.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            transform.DetachChildren();
        }
    }    
}
