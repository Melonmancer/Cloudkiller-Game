using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel
{
    private float health;
    private float damage;
    private float speed;
    //private float followDistance = 5;
    

    public Angel()
    {
        health = 1;
        damage = 1;
        speed = 1;
    }

    public Angel(float h, float d, float s)
    {
        health = h;
        damage = d;
        speed = s;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(float h)
    {
        health = h;
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetDamage(float d)
    {
        damage = d;
    }

    public float GetDamage()
    {
        return damage;
    }


    public void SetSpeed(float s)
    {
        speed = s;
    }

    public float GetSpeed()
    {
        return speed;
    }


    //Destroys angel and returns true if lethal damage is dealt
    public bool DamageAngel(float dmg)
    {
        health = (health - dmg);
        if(health <= 0f)
        {
            Destroy();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Destroy()
    {
        Debug.Log("Angel destroyed!");
    }
}
