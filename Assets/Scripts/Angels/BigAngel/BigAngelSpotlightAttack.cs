using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAngelSpotlightAttack : MonoBehaviour
{

    [SerializeField] private float disguiseDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player")
        {
            PlayerController pc = col.gameObject.GetComponent<PlayerController>();

            if(pc.GetIsDisguised())
            {
                pc.ChangeDisguiseHealth((disguiseDamage * -1f) * Time.deltaTime);
            }
            else
            {
                Debug.Log("SUFFER!!");
                pc.DamagePlayer(100);
            }
        }

    }
}
