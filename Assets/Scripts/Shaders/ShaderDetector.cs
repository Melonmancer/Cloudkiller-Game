using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderDetector : MonoBehaviour
{
    [SerializeField] private Renderer coneRenderer;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material whiteMaterial;

    void Start()
    {
        coneRenderer.material = whiteMaterial;
    }

    //private void OnTriggerEnter(Collider other)
    public void SetActivated()
    {
        //if (other.CompareTag("Player"))
        coneRenderer.material = redMaterial;
    }

    //private void OnTriggerExit(Collider other)
    public void SetDeactivated()
    {
        //if (other.CompareTag("Player"))
        coneRenderer.material = whiteMaterial;
    }

    public void HideShader()
    {
        coneRenderer.enabled = false;
    }

    public void ShowShader()
    {
        coneRenderer.enabled = true;
    }

}