using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSmallAngel : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    public void KillAngel()
    {
        Destroy(enemy);
    }
}
