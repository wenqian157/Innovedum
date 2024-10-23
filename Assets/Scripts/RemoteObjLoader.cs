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
            string material = RemoteCSVLoader.myLayerObjects[layerIndex - 6].material;
            StartCoroutine(LoadObjAsync(layerIndex, layerName, material));
        }
    }
    IEnumerator LoadObjAsync(int layerIndex, string layerName, string material)
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
            ApplyMaterial(meshGO, material);
            foreach (Transform child in meshGO.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layerIndex;
            }
        }

        log.SetActive(false);
    }
    private void ApplyMaterial(GameObject gameObject, string material)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter is null)
        {
            return;
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer is null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        Material mat = Resources.Load<Material>(material);
        if (mat is null) return;

        meshRenderer.material = mat;
    }
}
