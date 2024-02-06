using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteObjLoader : MonoBehaviour
{
    private string urlBase;
    void Start()
    {
        urlBase = RemoteResourceLoader.urlAddress;
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        while (RemoteCSVLoader.StoryLine.layerFilters.Length == 0)
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("reading cvs file...");
        }
    }
}
