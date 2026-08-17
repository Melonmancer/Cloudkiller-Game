using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{

    [SerializeField] private float coneRange = 5f;
    [SerializeField] private float coneAngle = 90f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool CheckIfPlayerInCone()
    {
        if(false) //  <---- Check distance between player and this object's transform, make sure it's within the range
        {
            return false;
        }
        else
        {
            //Check if player in cone by taking this object's transform and getting the angle between it and the player, comparing to coneAngle as a max
            //THEN raycast to player and make sure no obstacles are in the way

            //Will need to import raycasting from angel code
        }
    }
}
