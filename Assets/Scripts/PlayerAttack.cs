using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //Lifespan is how long the weapon lasts before disappearing.
    [SerializeField] private float lifespan = 0.2f;
    [SerializeField] private float damage = 5f;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Ticks timer to destroy this attack after a time limit.
        timer += (1f * Time.deltaTime);
        if (timer >= lifespan)
        {
            Destroy(this.gameObject.transform.parent.gameObject);
        }
    }
    
    //If this hits a killable enemy (only small angels at the moment) it is damaged (function in SmallAngel script).
    void OnTriggerEnter(Collider col)
    {   
        //Doesn't deal damage if the attack only hits an enemies' trigger (their hurtbox)
        if(col.gameObject.tag == "KillableEnemy" && !col.isTrigger)
        {
            SmallAngel angel = col.gameObject.GetComponent<SmallAngel>();
            angel.DamageAngel(damage);
        }
    }
}
