using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{

    [SerializeField] private float coneRange = 5f;
    [SerializeField] private float coneAngle = 90f;
    
    private bool expandedCone = false;

    //A layer mask used for the angel's line of sight - makes sure that only objects in the 'obstacle' layer block LOS
    private LayerMask lm;

    // Start is called before the first frame update
    void Start()
    {
        //Sets layer mask for the line of sight system - cannot see through anything in the obstacles layers
        lm = LayerMask.GetMask("Obstacle", "ObstacleNoSpotlight");
    }

    //Checks if an object is inside the vision cone and returns 'true' if it is. Must input the object you are checking for.
    public bool CheckIfObjectInCone(GameObject obj)
    {
        if((obj.transform.position - transform.position).magnitude <= coneRange) //  <---- Check distance between object and this cone's position, make sure it's within the range
        {
            //Check if player in cone by taking this object's transform and getting the angle between it and the player, comparing to coneAngle as a max
            float angle = Vector3.SignedAngle((obj.transform.position - transform.position), transform.forward, Vector3.up);

            //Debug.Log(angle);

            if((angle <= (coneAngle / 2) && angle >= ((coneAngle / 2) * -1f)) || expandedCone)
            {
                //THEN raycast to player and make sure no obstacles are in the way
                RaycastHit hit;

                //If the raycast hits an obstacle between this and the object, it must be obscured
                bool checkHit = Physics.Raycast(transform.position, (obj.transform.position - transform.position), out hit, coneRange, lm);
                if(checkHit && hit.distance < (obj.transform.position - transform.position).magnitude)
                {
                    Debug.DrawRay(transform.position, (obj.transform.position - transform.position).normalized * hit.distance, Color.yellow);

                    return false;
                }
                else
                {
                    Debug.DrawRay(transform.position, (obj.transform.position - transform.position).normalized * coneRange, Color.red);

                    Debug.Log("Hit player!");
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        //If object is not close enough to be within the cone, immediately return false
        else
        {
            return false;
        }
    }

    //When the angel has spotted the player, they need 360 degree vision to chase effectively
    //enabling expanded cone removes the angle check, so the angel will chase as long as the player is not obscured and is in the cone range
    public void EnableExpandedCone()
    {
        expandedCone = true;
    }

    public void DisableExpandedCone()
    {
        expandedCone = false;
    }
}
