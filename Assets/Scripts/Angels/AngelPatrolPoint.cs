using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelPatrolPoint : MonoBehaviour
{

    [SerializeField] GameObject linkedAngelSpawner;

    private GameObject linkedAngel = null;

    [SerializeField] GameObject nextPatrolPoint;

    private SmallAngel smallAngel = null;
    private SnitchAngel snitchAngel = null;
    
    [SerializeField] private float waitTime = 0f;
    private float waitStep = 0f;

    private bool goingNext = false;

    [SerializeField] private GameObject nextLookAtObject;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(linkedAngel != null)
        {
            Vector3 distanceFromPoint = (linkedAngel.transform.position - this.transform.position);
            //Debug.Log(distanceFromPoint.magnitude);

            if(distanceFromPoint.magnitude <= 1f)
            {
                if(waitTime > 0)
                {
                    if(waitStep >= waitTime)
                    {
                        waitStep = 0f;
                        goingNext = true;
                    }
                    else
                    {
                        waitStep += 1f * Time.deltaTime;
                    }
                }
                else
                {
                    goingNext = true;
                }
            }
            else
            {
                waitStep = 0f;
            }

            if(goingNext)
            {
                //Debug.Log("Angel reached patrol point");
                if(smallAngel != null) 
                {
                    smallAngel.PatrolAngel(nextPatrolPoint.transform.position, nextLookAtObject);
                }
                else if(snitchAngel != null) 
                {
                    snitchAngel.PatrolAngel(nextPatrolPoint.transform.position);
                }
                else
                {
                    Debug.Log("Error! Angel patrol point could not find an angel script to order around.");
                }

                goingNext = false;
            }
        }
        else
        {
            linkedAngel = linkedAngelSpawner.GetComponent<AngelSpawner>().GetCurrentAngel();
            //Debug.Log(linkedAngel);
            if(linkedAngel != null)
            {
                //Debug.Log("Found angel successfully!");
                smallAngel = linkedAngel.GetComponent<SmallAngel>();
                snitchAngel = linkedAngel.GetComponent<SnitchAngel>();
            }
        }
    }
}
