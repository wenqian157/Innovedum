using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Dummiesman;
using UnityEngine.Networking;

public class RemoteObjLoader : MonoBehaviour
{
    public GameObject log;
    void Start()
    {
        StartCoroutine(ReadCSVAsync());
    }
    public void OnClickLoadObj()
    {
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        yield return new WaitForSeconds(1.0f);
        RemoteLoadObj();
    }

    private void RemoteLoadObj()
    {
        Debug.Log($"found {RemoteCSVLoader.objLayers.Count} LayerObject");
        foreach (int layerIndex in RemoteCSVLoader.objLayers)
        {
            string layerName = RemoteCSVLoader.myLayerObjects[layerIndex - 6].name;
            StartCoroutine(LoadObjAsync(layerIndex, layerName));
        }
    }
    IEnumerator LoadObjAsync(int layerIndex, string layerName)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(RemoteCSVLoader.urlBase + "/obj/" + layerName + ".obj"))
        {
            www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"{www.error}");
                yield break;
            }
            while (!www.isDone)
            {
                Debug.Log("loading...");
                yield return new WaitForSeconds(0.2f);
            }
            //var textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
            //GameObject mesh = new OBJLoader().Load(textStream);

            GameObject meshGO = ObjReader.ObjToMeshObject(www.downloadHandler.text);

            meshGO.name = layerName;
            meshGO.transform.SetParent(this.transform);
            meshGO.layer = layerIndex;
            foreach (Transform child in meshGO.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layerIndex;
            }
        }

        log.SetActive(false);
    }
}
