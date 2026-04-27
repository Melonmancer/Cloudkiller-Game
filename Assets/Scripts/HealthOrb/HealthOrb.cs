using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    [SerializeField] private float healthRestored;

    private HealthOrbSpawner spawner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Runs whilst this orb's trigger collider is colliding with something
    public void OnTriggerStay(Collider col)
    {
        //Checks if collision is with the player object
        if(col.gameObject.tag == "Player")
        {
            PlayerController pc = col.gameObject.GetComponent<PlayerController>();

            float[] values = pc.GetHealthValues();
            //Only gives health and consumes orb is health is below max health
            if(values[0] < values[1])
            {
                //Adds health
                pc.ChangeHealth(healthRestored);

                spawner.DeathAlert();

                Destroy(this.gameObject);
            }
        }
    }

    public void SetVariables(HealthOrbSpawner s, float h)
    {
        spawner = s;
        healthRestored = h;
    }
}
