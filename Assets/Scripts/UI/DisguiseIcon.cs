using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DisguiseIcon : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.isDisguised == true)
        {
            _animator.SetBool("IsDisguised", true);
        }
        else
        {
            _animator.SetBool("IsDisguised", false);
        }
    }
}
