using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Dummiesman;

public class RemoteObjLoader : MonoBehaviour
{
    private string urlBase;
    void Start()
    {
        urlBase = RemoteCSVLoader.urlBase;
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        while (RemoteCSVLoader.StoryLine.layerFilters.Length == 0)
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("reading cvs file...");
        }
        //LoadObj();
        RemoteLoadObj();
    }
    private void LoadObj()
    {
        Debug.Log($"found {RemoteCSVLoader.objLayers.Count} LayerObject");
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
    private void RemoteLoadObj()
    {
        Debug.Log($"found {RemoteCSVLoader.objLayers.Count} LayerObject");
        foreach (int layerIndex in RemoteCSVLoader.objLayers)
        {
            string layerName = RemoteCSVLoader.myLayerObjects[layerIndex - 6].name;
            var www = new WWW(RemoteCSVLoader.urlBase + "/obj/" + layerName + ".obj");
            while (!www.isDone) System.Threading.Thread.Sleep(1);

            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.text));
            GameObject mesh = new OBJLoader().Load(textStream);
            mesh.layer = layerIndex;
            foreach (Transform child in mesh.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layerIndex;
            }
        }
    }
}
