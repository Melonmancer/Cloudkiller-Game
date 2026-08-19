using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeTester : MonoBehaviour
{


    [SerializeField] private VisionCone cone;
    [SerializeField] private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cone.CheckIfObjectInCone(target))
        {
            Debug.Log("I see you!");
        }


        //Angel will turn to face the player whilst trying to spot them - RIPPED FROM SMALL ANGEL CODE FOR TESTING PURPOSES


        //The direction the angel should turn to face towards its target
        Vector3 turnDirection = new Vector3(target.transform.position.x, 0, target.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
            
        //Interpolates between the direction the angel's mesh is currently facing and the direction it should be facing to turn it.
        transform.forward = Vector3.Slerp(transform.forward, turnDirection.normalized, 0.5f * Time.deltaTime);
    }
}
