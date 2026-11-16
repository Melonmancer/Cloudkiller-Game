using UnityEngine;

public class PlayerStayAwake : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Rigidbody>().sleepThreshold = 0f;
    }
}
