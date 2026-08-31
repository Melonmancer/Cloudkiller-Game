using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisguisePickup : MonoBehaviour
{
    [SerializeField] private float disguiseRestored;

    private DisguisePickupSpawner spawner;

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

            //Adds health
            pc.ChangeDisguiseHealth(disguiseRestored);

            spawner.DeathAlert();

            Destroy(this.gameObject);
        }
    }

    public void SetVariables(DisguisePickupSpawner s, float d)
    {
        spawner = s;
        disguiseRestored = d;
    }
}
