using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayingScale : MonoBehaviour
{
    void Start()
    {
        Vector3 scale = new Vector3(
            RemoteCSVLoader.displayingScale,
            RemoteCSVLoader.displayingScale,
            RemoteCSVLoader.displayingScale
            );
        transform.localScale = scale;
    }
}
