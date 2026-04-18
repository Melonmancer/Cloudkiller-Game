using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmallAngel : MonoBehaviour
{
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

    [SerializeField] private float maxWaitTime;
    private float timeWaiting = 0f;
    bool isWaiting = false;
    
    [SerializeField] private float spotSpeed;
    private float tickSpotting = 0f;
    private bool spottedPlayer = false;

    //Starting point for the angel - returns here when not chasing the player
    Vector3 home;

    //Vector object used for vector calculations
    private Vector3 directionToTarget = new Vector3();
    private Vector3 directionToTargetFromHome = new Vector3();

    private LayerMask lm;

    private AngelSpawner spawner = null;

    //Animation Controller
    [SerializeField] private GameObject angelObject;
    private Animator animator;
    private float animationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        //If target is not set, tries to find the player in the scene and set it as target
        if(target == null)
        {
            FindPlayerTarget();
        }
        
        //All small angels should have a NavMeshAgent attached for moving and navigating!
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        home = this.transform.position;

        //Sizes attached bubble to make it represent the angel's chase distance
        //May remove this later!!
        if(bubble != null)
        {
            bubble.transform.localScale = new Vector3(chaseDistance * 2, 0, chaseDistance * 2);
        }

        agent.speed = speed;

        animator = angelObject.GetComponent<Animator>();

        lm = LayerMask.GetMask("Obstacle");
        
    }

    // Update is called once per frame
    void Update()
    { 
        //Gets the directional data using vector math
        directionToTarget = (target.transform.position - this.transform.position);
        directionToTargetFromHome = (target.transform.position - home);

        //If the target is further away than the chase distance allows, the angel starts moving to its home point instead
        //NOTE: All the programming for the angel's movement is done using NavMeshAgent - consult the Unity documentation
        if(directionToTargetFromHome.magnitude <= chaseDistance)
        {   
            //Line of sight is calculated using a raycast line
            RaycastHit hit;
            //If the raycast hits an obstacle between them and the player, the angel's destination does not update - they stay at home, or move to the player's last known position and wait there
            if(Physics.Raycast(this.transform.position, directionToTarget, out hit, chaseDistance, lm) && hit.distance < directionToTarget.magnitude)
            {
                Debug.DrawRay(this.transform.position, directionToTarget.normalized * hit.distance, Color.yellow);

                //Resets spotting progress - the player is hidden!
                tickSpotting = 0f;

                if(agent.destination != home && agent.remainingDistance <= 0.5f)
                {
                    isWaiting = true;
                }
            }
            //If the raycast doesn't hit an obstacle, there is line of sight - the angel starts spotting the player
            else
            {
                //If the player has been 'spotted' the angel locks on and starts moving to their destination
                if(spottedPlayer)
                {
                    Debug.DrawRay(this.transform.position, directionToTarget, Color.red);

                    agent.destination = target.transform.position; 
                }
                //Otherwise, spot value builds until the player is spotted
                else
                {
                    Debug.DrawRay(this.transform.position, directionToTarget, Color.magenta);

                    tickSpotting += (1f * Time.deltaTime);

                    if(tickSpotting >= spotSpeed)
                    {
                        spottedPlayer = true;
                        tickSpotting = 0f;
                        //Debug.Log("Spotted player!");
                    }
                }

                //If the angel can see the player (and the player is inside the chase distance), any waiting progress is reset
                timeWaiting = 0f;
                isWaiting = false;
            }
        }
        
        //If the angel can see the player but they are outside the angel's chase distance, they will wait at the player's last known position inside their chase distance
        if(agent.destination != home && directionToTargetFromHome.magnitude >= chaseDistance && agent.remainingDistance <= 0.5f)
        {
            isWaiting = true;
        }


        //Debug: Draws a blue line to the angel's destination
        Debug.DrawRay(this.transform.position, (agent.destination - this.transform.position).normalized * agent.remainingDistance, Color.blue);


        //If the angel is waiting, ticks down until maxWaitTime is exceeded. Then, the angel returns to its home and un-spots the player
        if(isWaiting)
        {
            timeWaiting += (1f * Time.deltaTime);

            if(timeWaiting >= maxWaitTime)
            {
                agent.destination = home;
                timeWaiting = 0f;
                isWaiting = false;
                spottedPlayer = false;
            }
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
        UpdateAnimations();
    }

    //Handles idle/run animations
    void UpdateAnimations()
    {
        animationSpeed = agent.velocity.magnitude;
        if (agent != null && animator != null)
        {
            animator.SetFloat("flySpeed", animationSpeed);
        }
        //Debug.Log("Agent speed: " + animationSpeed);
    }

    //Looks for an object in the scene with the player tag - if found, sets it as the target for the navmeshagent to use
    private bool FindPlayerTarget()
    {
        target = GameObject.FindWithTag("Player");
        if(target != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool DamageAngel(float damage)
    {
        health -= damage;
        Debug.Log("Damaged! " + health + " health remaining!");


        //If the angel is at 0 hp, it is destroyed. Returns true or false based on if the angel was killed or not.
        if(health <= 0)
        {
            //Sends alert to spawner so it creates a new angel
            spawner.DeathAlert();

            //Destroys the whole angel prefab (the angel prefab should be an empty object containing the actual angel object and other relevant objects i.e. the bubble)
            Destroy(this.gameObject.transform.parent.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    //Runs whilst this angel's trigger collider is colliding with something
    public void OnTriggerStay(Collider col)
    {
        if(attackReady)
        {
            //Checks if collision is with the player object
            if(col.gameObject.tag == "Player")
            {
                Debug.Log("Hit player!");
                attackReady = false;
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                //Damages the player
                col.gameObject.GetComponent<PlayerController>().DamagePlayer(damage);
            }
        }
    }

    //Sets all variables - this is used by spawners to fill in data for the spawned angel
    public void SetVariables(AngelSpawner spawnScript, GameObject t, float h, float d, float s, float cD, float aC, float mWT, float sS)
    {
        spawner = spawnScript;
        target = t;
        health = h;
        damage = d;
        speed = s;
        chaseDistance = cD;
        attackCooldown = aC;
        maxWaitTime = mWT;
        spotSpeed = sS;
    }
}
