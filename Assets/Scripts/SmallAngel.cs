using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmallAngel : MonoBehaviour
{
    private Angel angel = new Angel();
    private UnityEngine.AI.NavMeshAgent agent;

    [SerializeField] private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        //angel.DamageAngel(100);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.transform.position; 
    }


}
