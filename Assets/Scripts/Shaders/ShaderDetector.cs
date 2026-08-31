using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderDetector : MonoBehaviour
{
    [SerializeField] private Renderer coneRenderer;
    [SerializeField] private Material coneMaterial;

    void Start()
    {
        coneMaterial.SetFloat("isdetected", 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            coneMaterial.SetFloat("isdetected", 1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            coneMaterial.SetFloat("isdetected", 0f);
        }
    }
}