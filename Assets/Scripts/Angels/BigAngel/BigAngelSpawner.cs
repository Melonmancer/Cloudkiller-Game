using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAngelSpawner : MonoBehaviour
{

    //The angel prefab spawned by this spawner
    [SerializeField] private GameObject angelPrefab;

    [SerializeField] private BigAngelCheckpoint linkedCheckpoint;

    private GameObject currentAliveAngel;

    [SerializeField] public bool startsActive = false;

    private bool isActive = false;


    // Start is called before the first frame update
    void Start()
    {   
        if(startsActive)
        {
            MakeNewAngel();
            isActive = true;
        }
    }

    //Creates a new angel at this spawner's position, passing in the variables
    private void MakeNewAngel()
    {
        GameObject newAngel = Instantiate(angelPrefab, this.transform);

        currentAliveAngel = newAngel;

        linkedCheckpoint.SendDirectionsToBigAngel();
    }

    public GameObject GetCurrentAngel()
    {
        return currentAliveAngel;
    }

    private void DestroyCurrentAngel()
    {
        if(currentAliveAngel != null)
        {
            Destroy(currentAliveAngel);
        }
    }

    public void ResetSpawn()
    {
        DestroyCurrentAngel();
        if(isActive)
        {
            MakeNewAngel();
        }
    }

    public void SetIsActive(bool condition)
    {
        isActive = condition;
    }

    public bool GetIsActive()
    {
        return isActive;
    }
}
