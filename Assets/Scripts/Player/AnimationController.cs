using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController: MonoBehaviour
{
    [SerializeField] private GameObject daggerMesh;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowWeapon()
    {
        daggerMesh.SetActive(true);
    }

    public void HideWeapon()
    {
        daggerMesh.SetActive(false);
    }
}
