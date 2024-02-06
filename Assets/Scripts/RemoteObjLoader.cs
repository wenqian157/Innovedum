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
        LoadObj();
    }
    private void LoadObj()
    {
        foreach (int layerIndex in RemoteCSVLoader.objLayers)
        {
            string layerName = RemoteCSVLoader.myLayerObjects[layerIndex - 6].name;
            GameObject mesh = Resources.Load<GameObject>($"obj/{layerName}");
            mesh = Instantiate(mesh, this.transform);
            mesh.layer = layerIndex;
            foreach (Transform child in mesh.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layerIndex;
            }
        }
    }
}
