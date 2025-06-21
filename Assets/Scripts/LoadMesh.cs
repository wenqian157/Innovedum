using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadMesh : MonoBehaviour
{
    private void Start()
    {
        LoadMeshByLayer();
    }
    private void LoadMeshByLayer()
    {
        Debug.Log($"found {LoadCSV.instance.meshLayers.Count} mesh object");
        Logs.Instance.announce.text = $"found {LoadCSV.instance.meshLayers.Count} mesh object";
        foreach (int i in LoadCSV.instance.meshLayers)
        {
            string name = LoadCSV.instance.myLayerObjects[i-6].name;
            string material = LoadCSV.instance.myLayerObjects[i-6].material;
            StartCoroutine(LoadAsync(i, name, material));
            LoadingProgress.Instance.coroutineCount++;
        }
    }
    IEnumerator LoadAsync(int layerIndex, string layerName, string material)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(LoadInfo.instance.urlBase + "/obj/" + layerName + ".obj"))
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
                yield return new WaitForSeconds(0.1f);
            }

            GameObject meshGO = ObjReader.ObjToMeshObject(www.downloadHandler.text);
            meshGO.name = layerName;
            meshGO.transform.SetParent(transform);
            meshGO.layer = layerIndex;
            meshGO.transform.localScale = new Vector3(1, 1, 1);
            ApplyMaterial(meshGO, material);
            foreach (Transform child in meshGO.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layerIndex;
            }
        }
        LoadingProgress.Instance.coroutineCount--;
    }
    private void ApplyMaterial(GameObject obj, string material)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter is null)
        {
            return;
        }

        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        if (meshRenderer is null)
        {
            meshRenderer = obj.AddComponent<MeshRenderer>();
        }

        Material mat = Resources.Load<Material>(material);
        if (mat is null) return;

        meshRenderer.material = mat;
    }
}
