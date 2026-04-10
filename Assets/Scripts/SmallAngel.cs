using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmallAngel : MonoBehaviour
{
    private Angel angel;
    private UnityEngine.AI.NavMeshAgent agent;

    [SerializeField] private GameObject target;

    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float speed;

    //Bubbles are cylinders stretched to represent the angel's chase distance on the floor. May remove this later!!
    [SerializeField] private GameObject bubble;

    [SerializeField] private float chaseDistance;

    [SerializeField] private float attackCooldown;
    private float attackTimer = 0f;
    private bool attackReady = true;
    
    //Starting point for the angel - returns here when not chasing the player
    Vector3 home;

    //Vector object used for vector calculations
    Vector3 calc = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        //NOTE TO SELF: This class may be deprecated later - consider structure.
        angel = new Angel(health, damage, speed);
        
        //All small angels should have a NavMeshAgent attached for moving and navigating!
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        home = this.transform.position;

        //Sizes attached bubble to make it represent the angel's chase distance
        //May remove this later!!
        if(bubble != null)
        {
            bubble.transform.localScale = new Vector3(chaseDistance * 2, 0, chaseDistance * 2);
        }

        agent.speed = angel.GetSpeed();
    }

    // Update is called once per frame
    void Update()
    { 
        //Gets the distance between the home point and the target's position using vector math
        //If the target is further away than the chase distance allows, the angel starts moving to its home point instead
        //NOTE: All the programming for the angel's movement is done using NavMeshAgent - consult the Unity documentation
        calc = (target.transform.position - home);

        if(calc.magnitude <= chaseDistance)
        {
            agent.destination = target.transform.position; 
        }
        else
        {
            agent.destination = home;
        }

        //Ticks down attack cooldown if it is on cooldown
        if(!attackReady)
        {
            attackTimer += (1f * Time.deltaTime);
            //Debug.Log(attackTimer);
            if(attackTimer >= attackCooldown)
            {
                attackReady = true;
                attackTimer = 0f;
                agent.isStopped = false;
            }
        }
    }

    public bool DamageAngel(float damage)
    {
        health -= damage;
        Debug.Log("Damaged! " + health + " health remaining!");


        //If the angel is at 0 hp, it is destroyed. Returns true or false based on if the angel was killed or not.
        if(health <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnTriggerStay(Collider col)
    {
        if(attackReady)
        {
            if(col.gameObject.tag == "Player")
            {
                Debug.Log("Hit player!");
                attackReady = false;
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                col.gameObject.GetComponent<PlayerController>().DamagePlayer(damage);
            }
        }
    }
}
