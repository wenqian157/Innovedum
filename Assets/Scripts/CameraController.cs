using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    private Vector3 originTarPos;
    private Quaternion originTarRot;
    private Vector3 originCamPos;
    private Quaternion originCamRot;
    private float orthoZoom = 15;
    private Camera cam;

    private enum camStates { perspective, top, front, left};
    private camStates currentCam;

    private void Start()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        GameObject go = new GameObject("Cam Target");
        go.transform.position = transform.position + (transform.forward * distance);
        target = go.transform;

        originTarPos = target.position;
        originTarRot = target.rotation;
        originCamPos = transform.position;
        originCamRot = transform.rotation;

        cam = this.GetComponent<Camera>();

        Init(); 
    }
    private void Init()
    {
        currentCam = camStates.perspective;
        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = -Vector3.Angle(Vector3.right, transform.right); // this is related to how we orient the 3d model initially
        yDeg = Vector3.Angle(Vector3.up, transform.up); 
    }
    void LateUpdate()
    {
        // pan!
        if (Input.GetMouseButton(2))
        {
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
        }
        // orbit!
        else if (Input.GetMouseButton(1))
        {
            if(currentCam == camStates.perspective)
            {
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                ////////OrbitAngle
                //Clamp the vertical axis for the orbit
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                // set camera rotation
                desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
                currentRotation = transform.rotation;

                rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
                transform.rotation = rotation;
            }
        }

        // calculate position based on the new currentDistance
        if(currentCam == camStates.top)
        {
            // zoom
            orthoZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000;
            orthoZoom = Mathf.Clamp(orthoZoom, 1, 20);
            cam.orthographicSize = orthoZoom;

            // pan
            position = target.position - rotation * Vector3.down *currentDistance;
        }
        else if (currentCam == camStates.left)
        {
            // zoom
            orthoZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000;
            orthoZoom = Mathf.Clamp(orthoZoom, 1, 20);
            cam.orthographicSize = orthoZoom;
            position = target.position - rotation * Vector3.right * currentDistance;
        }
        else if (currentCam == camStates.front)
        {
            // zoom
            orthoZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000;
            orthoZoom = Mathf.Clamp(orthoZoom, 1, 20);
            cam.orthographicSize = orthoZoom;
            position = target.position - rotation * Vector3.back * currentDistance;
        }
        else
        {
            // zoom!
            // affect the desired Zoom distance if we roll the scrollwheel
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
            //clamp the zoom min/max
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            // For smoothing of the zoom, lerp distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            //pan
            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        }
        
        transform.position = position;
    }
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
    public void ResetCamera()
    {
        target.SetPositionAndRotation(originTarPos, originTarRot);
        transform.SetPositionAndRotation(originCamPos, originCamRot);
        Init();

        cam.orthographic = false;
    }
    public void TopView()
    {
        currentCam = camStates.top;
        rotation = Quaternion.Euler(90, 0, 0);
        target.SetPositionAndRotation(new Vector3(0, 20 - 5, -6), Quaternion.Euler(90, 0, 0));
        transform.SetPositionAndRotation(new Vector3(0, 20, 0), Quaternion.Euler(90, 0, 0));
        
        cam.orthographic = true;
        cam.orthographicSize = 15;
        orthoZoom = 15;
    }
    public void LeftView()
    {
        currentCam = camStates.left;
        rotation = Quaternion.Euler(0, 90, 0);
        target.SetPositionAndRotation(new Vector3(-20 - 5, 0, -8), Quaternion.Euler(0, 90, 0));
        transform.SetPositionAndRotation(new Vector3(-20, 0, 0), Quaternion.Euler(0, 90, 0));

        cam.orthographic = true;
        cam.orthographicSize = 15;
        orthoZoom = 15;
    }
    public void FrontView()
    {
        currentCam = camStates.left;
        rotation = Quaternion.Euler(0, 90, 0);
        target.SetPositionAndRotation(new Vector3(0, 0, -20 - 5), Quaternion.Euler(0, 0, 0));
        transform.SetPositionAndRotation(new Vector3(0, 0, -20), Quaternion.Euler(0, 0, 0));

        cam.orthographic = true;
        cam.orthographicSize = 15;
        orthoZoom = 15;
    }
}
