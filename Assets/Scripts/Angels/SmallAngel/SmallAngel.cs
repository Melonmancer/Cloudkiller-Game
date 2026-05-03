using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SmallAngel : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;

    [SerializeField] private GameObject target;
    private PlayerController playerController;

    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    private float rotationSpeed = 0.15f;

    //How much "damage" the angel deals to the player's disguise
    [SerializeField] private float disguiseDamage;

    //Bubbles are cylinders stretched to represent the angel's chase distance on the floor. May remove this later!!
    [SerializeField] private GameObject bubble;

    //How far the angel can see/will stray from its spawn point
    [SerializeField] private float chaseDistance;

    [SerializeField] private float attackCooldown;
    private float attackTimer = 0f;
    private bool attackReady = true;

    //How long the angel stays alerted before forgetting it saw the player
    [SerializeField] private float maxWaitTime;
    private float timeWaiting = 0f;
    private bool isWaiting = false;

    private bool alerted = false;
    
    //How fast the angel detects the player whilst they are in its line of sight
    [SerializeField] private float spotSpeed;
    private float tickSpotting = 0f;
    private bool spottedPlayer = false;

    //Starting point for the angel - returns here when not chasing the player
    Vector3 home;

    //Used for vector calculations - holds data about target's location relative to the angel and its home
    private Vector3 directionToTarget = new Vector3();
    private Vector3 directionToTargetFromHome = new Vector3();

    //Makes the angel turn towards the player/its target
    bool turningToPlayer = false;
    private Vector3 turnDirection = new Vector3();

    //A layer mask used for the angel's line of sight - makes sure that only objects in the 'obstacle' layer block LOS
    private LayerMask lm;

    //A reference to the spawner that made the angel instance - ALL ANGELS SHOULD BE MADE FROM SPAWNERS! This lets them respawn!
    private AngelSpawner spawner = null;

    //Animation Controller
    [SerializeField] private GameObject angelMesh;
    private Animator animator;
    private float animationSpeed;

    //WIP - Might delete later - Allows a text display over the angel's head (Using this to demonstrate its L.O.S. behaviours at the moment)
    [SerializeField] private GameObject textObject;
    private TMP_Text text;

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
        
        home = this.transform.position;

        //Sizes the attached bubble to visually represent the angel's chase distance. May remove/rework this later!!
        if(bubble != null)
        {
            bubble.transform.localScale = new Vector3(chaseDistance * 2, 0, chaseDistance * 2);
        }
        
        //Sets speed of the NavMeshAgent
        agent.speed = speed;

        animator = angelMesh.GetComponent<Animator>();

        //Sets layer mask for the line of sight system - cannot see through anything in the obstacles layers
        lm = LayerMask.GetMask("Obstacle", "ObstacleNoSpotlight");

        //WIP - Gets text component
        text = textObject.GetComponent<TMP_Text>();
        
    }

    // Update is called once per frame
    void Update()
    { 
        //ON ANGEL AI:
        //The angel has a few different states it progresses through!
        //If it ever has line of sight to the player whilst the player is within its range, it gets ALERTED
        //Whilst ALERTED, it can SPOT the player if they are in line of sight for too long
        //If the player is spotted and disguised, the angel stares and drains their disguise
        //If the player is spotted and not disguised, the angel chases after them!
        //If the angel is alerted but the player breaks line of sight, the angel WAITS in place
        //If the angel WAITS for too long, it becomes un-alerted and returns to its home point

        //Gets the directional data to the target using vector math
        directionToTarget = (target.transform.position - this.transform.position);
        directionToTargetFromHome = (target.transform.position - home);

        //If the target is in range, uses a raycast to check if and for how long the target has been in line of sight, handling the alert and spotting behaviours
        if(directionToTargetFromHome.magnitude <= chaseDistance)
        {
            CastLineOfSight();   
        }
        
        //If the angel is alerted but the target is outside the angel's chase distance, they will wait instead of chasing further (as if they lost line of sight)
        if(alerted && directionToTargetFromHome.magnitude > chaseDistance && agent.remainingDistance <= 0.5f)
        {
            tickSpotting = 0f;
            isWaiting = true;
        }

        //For debugging: Draws a blue line to the angel's destination
        Debug.DrawRay(this.transform.position, (agent.destination - this.transform.position).normalized * agent.remainingDistance, Color.blue);

        //If the angel is waiting in place, ticks down until maxWaitTime is exceeded. Then, the angel returns to its home, un-alerts and un-spots the player (a full reset)
        if(isWaiting)
        {
            timeWaiting += (1f * Time.deltaTime);

            if(timeWaiting >= maxWaitTime)
            {
                agent.destination = home;
                timeWaiting = 0f;
                isWaiting = false;
                spottedPlayer = false;
                alerted = false;

                text.text = "";
            }
            else
            {
                //The angel spotted the player, but lost sight of them
                if(spottedPlayer)
                {
                    text.text = "...!";
                }
                //The player wasn't spotted, but the angel is still alerted
                else
                {
                    text.text = "...?";
                }
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

    //Handles all the line-of-sight AI, including the angel's alert and spot behaviours
    void CastLineOfSight()
    {
        //Line of sight is calculated using a raycast line
        //NOTE: All the programming for the angel's movement is done using NavMeshAgent - consult the Unity documentation
        RaycastHit hit;
        //If the raycast hits an obstacle between them and the player, the angel cannot see them
        if(Physics.Raycast(this.transform.position, directionToTarget, out hit, chaseDistance, lm) && hit.distance < directionToTarget.magnitude)
        {
            Debug.DrawRay(this.transform.position, directionToTarget.normalized * hit.distance, Color.yellow);

            //If the angel was spotting the player, the spotting progress resets - the player has hidden in time!
            tickSpotting = 0f;

            //If the angel is alerted and has reached its current destination but cannot see the player, it waits there
            if(alerted && agent.remainingDistance <= 0.5f)
            {
                isWaiting = true;
            }
        }
        //If the raycast doesn't hit an obstacle, there is line of sight - the angel starts spotting the player
        else
        {
            //If the player has been 'spotted' the angel locks on!
            if(spottedPlayer)
            {
                //NOTE TO SELF: Team has debated whether the angel should be fooled by the disguise whilst it is already chasing the player
                //Right now, it IS fooled - to change this, will probably need another bool for chaseInitiated - if this is true and player is disguised,
                //make it so the angel doesn't care about the disguise/drains it to 0 immediately

                //If player is diguised, the angel stops in place and starts draining their disguise
                if(playerController.GetIsDisguised())
                {
                    turningToPlayer = true;

                    text.text = "!?";

                    Debug.DrawRay(this.transform.position, directionToTarget, Color.white);

                    //Stops the angel in place
                    agent.destination = this.transform.position;

                    //Drains disguise
                    playerController.ChangeDisguiseHealth((disguiseDamage * -1) *  Time.deltaTime);
                }
                //If player is not disguised, chases after them
                if(!playerController.GetIsDisguised())
                {
                    text.text = "!!";

                    Debug.DrawRay(this.transform.position, directionToTarget, Color.red);

                    agent.destination = target.transform.position;                   
                } 
            }
            //If the angel can see the player but has not spotted them, spot value builds until the player is spotted - the angel stops moving whilst spotting
            else
            {
                turningToPlayer = true;

                text.text = "?";

                agent.destination = this.transform.position;

                Debug.DrawRay(this.transform.position, directionToTarget, Color.magenta);
                    

                //Ticks up spotting value
                tickSpotting += (1f * Time.deltaTime);

                if(tickSpotting >= spotSpeed)
                {
                    spottedPlayer = true;
                    tickSpotting = 0f;
                    //Debug.Log("Spotted player!");
                }
            }

            //If the angel can see the player (and the player is inside the chase distance), it stops waiting and becomes alerted if it was not already
            timeWaiting = 0f;
            isWaiting = false;
            alerted = true;     
        }
    }


    void FixedUpdate()
    {
        //Angel will turn to face the player whilst trying to spot them
        if(turningToPlayer)
        {
            //The direction the angel should turn to face towards its target
            turnDirection = target.transform.position - new Vector3(this.transform.position.x, 0, this.transform.position.z);
            
            //Interpolates between the direction the angel's mesh is currently facing and the direction it should be facing to turn it.
            this.transform.forward = Vector3.Slerp(this.transform.forward, turnDirection.normalized, rotationSpeed);

            //When the transform vector and the intended turn direction are near equal, the vector between them should be very very short
            //A short vector has a small magnitude, so we can use this to compare the two vectors without needing them to be exactly equal
            if((this.transform.forward - turnDirection.normalized).magnitude < 0.1f)
            {
                turningToPlayer = false;
                //Debug.Log("Finished turning!");
            }
        }
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
    
    //Causes the angel to take damage, destroying it if lethal damage is dealt
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
                playerController.DamagePlayer(damage);
            }
        }
    }

    //Sets all variables - this is used by spawners to fill in data for the spawned angel
    public void SetVariables(AngelSpawner spawnScript, GameObject t, float h, float d, float s, float cD, float aC, float mWT, float sS, float dD)
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
        disguiseDamage = dD;
    }
}
