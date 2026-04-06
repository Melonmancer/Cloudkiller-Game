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

    [SerializeField] private GameObject bubble;

    [SerializeField] private float chaseDistance;
    
    Vector3 home;

    Vector3 calc = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        angel = new Angel(health, damage, speed);
        //angel.DamageAngel(100);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        home = this.transform.position;

        bubble.transform.localScale = new Vector3(chaseDistance * 2, 0, chaseDistance * 2);

        agent.speed = angel.GetSpeed();
    }

    // Update is called once per frame
    void Update()
    { 
        calc = (target.transform.position - home);
        if(calc.magnitude <= chaseDistance)
        {
            agent.destination = target.transform.position; 
        }
        else
        {
            agent.destination = home;
        }
    }


}
