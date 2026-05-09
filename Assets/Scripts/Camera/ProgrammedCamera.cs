using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammedCamera : MonoBehaviour
{

    [SerializeField] GameObject playerObject;
    [SerializeField] GameObject cam;

    //How far the camera tries to stay from the player
    [SerializeField] private float defaultCameraDistance = 6.5f;

    //Adjusts interpolation for smoother tracking
    [SerializeField] private float cameraSmoothing = 10f;

    //How responsive the camera is to mouse inputs
    [SerializeField] private float sensitivity = 5f;

    [SerializeField] private float minimumYTilt = -30f;
    [SerializeField] private float maximumYTilt = 40f;

    private LayerMask lm;

    float x = 0f;
    float y = 0f;

    void Start()
    {
        //Layer mask used to detect obstacles that would block the camera
        lm = LayerMask.GetMask("Obstacle", "ObstacleNoSpotlight");
    }

    void Update()
    {
        //Gets mouse input and adds it to the x and y axes
        float tiltAroundY = (Input.GetAxis("Mouse Y") * sensitivity) * -1f;
        float tiltAroundX = (Input.GetAxis("Mouse X") * sensitivity);

        x += tiltAroundX;
        y += tiltAroundY;

        //Prevents Y tilt from exceeding set values
        if(y < minimumYTilt)
        {
            y = minimumYTilt;
        }
        if(y > maximumYTilt)
        {
            y = maximumYTilt;
        }

        //Creates new target rotation based on x and y
        Quaternion target = Quaternion.Euler(y, x, 0);

        //Interpolates current rotation to target rotation
        Quaternion interpolatedTarget = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * cameraSmoothing);
        
        Vector3 eulers = interpolatedTarget.eulerAngles;

        //Factors out any Z rotation to keep the camera from tilting, then sets rotation
        transform.rotation = Quaternion.Euler(eulers.x, eulers.y, 0);

        CameraCollision();

    }

    //Sets camera focus to follow the player (camera is attached to and follows the focus)
    void FixedUpdate()
    {
        transform.position = playerObject.transform.position;
        transform.Translate(Vector3.up * 1.75f);
    }


    //Adjusts Z of camera to either keep it the set distance, or 
    //move closer to get in front of terrain
    void CameraCollision()
    {
        Vector3 directionToCamera = (cam.transform.position - transform.position);

        cam.transform.position = transform.position;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, directionToCamera, out hit, defaultCameraDistance, lm))
        {
            cam.transform.Translate(Vector3.back * (hit.distance - 0.1f));
        }
        else
        {
            cam.transform.Translate(Vector3.back * defaultCameraDistance);
        }
        //Debug.DrawRay(transform.position, Vector3.back * defaultCameraDistance, Color.yellow);
    }

}
