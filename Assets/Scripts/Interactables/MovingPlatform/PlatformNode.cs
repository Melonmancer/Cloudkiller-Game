using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformNode : MonoBehaviour
{
    [SerializeField] private PlatformNode nextNode;
    [SerializeField] private float speedToNextNode;
    [SerializeField] private float delayBeforeMoving;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetNodePosition()
    {
        return transform.position;
    }

    public PlatformNode GetNextNode()
    {
        return nextNode;
    }

    public float GetSpeedToNextNode()
    {
        return speedToNextNode;
    }

    public float GetDelayBeforeMoving()
    {
        return delayBeforeMoving;
    }
}
