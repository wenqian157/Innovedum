using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Test : MonoBehaviour
{
    private float orthoSize = 5;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    void Update()
    {
        orthoSize += Input.GetAxis("Mouse ScrollWheel")* 100;
        Debug.Log(orthoSize);

        cam.orthographicSize = orthoSize;
    }
        
}
