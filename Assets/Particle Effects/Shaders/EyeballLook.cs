using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EyeballLook : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 5f;
    [SerializeField] private Material eyeMaterial;
    private Vector3 restDirection = default;
    private float value = 0f;

    void Start()
    {
        restDirection = transform.position + transform.forward;
    }

    void Update()
    {
        float currentDistance = Vector3.Distance(transform.position, player.position);

        if(currentDistance < distance)
        {
            value += Time.deltaTime;
            value = Mathf.Min(value, 1f);
        }
        else
        {
            value -= Time.deltaTime / 2f;
            value = Mathf.Max(value, 0f);
        }

        float pupilSize = math.remap(0, distance, 0, 1, 1 - value);
        eyeMaterial.SetFloat("focus", value);

        Vector3 lookAt = Vector3.Lerp(restDirection, player.position, value);
        transform.LookAt(lookAt, Vector3.up);



    }
}
