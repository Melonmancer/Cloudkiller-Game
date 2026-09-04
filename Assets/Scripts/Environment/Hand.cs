using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private Animator animator;
    private float randomOffset;
    [SerializeField] private GameObject handMesh;

    void Start()
    {
        randomOffset = Random.Range(0f, 1f);
        animator = handMesh.GetComponent<Animator>();
        animator.SetFloat("Offset", randomOffset);
    }
}
