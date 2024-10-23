using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Test : MonoBehaviour
{
    int count = 0;
    int max = 10;

    void Start()
    {
        
    }

    void Update()
    {
        while (count < max)
        {
            count++;
            Thread.Sleep(500);
            Debug.Log(count);
        }
        Debug.Log(Time.deltaTime);
    }
}
