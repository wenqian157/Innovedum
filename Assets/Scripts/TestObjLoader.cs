using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Dummiesman;
using System.Text;

public class TestObjLoader : MonoBehaviour
{
    private string url;
    void Start()
    {
        url = "https://raw.githubusercontent.com/wenqian157/Innovedum/main/OnlineResources/obj/mesh_concrete_beam.obj";
        //StartCoroutine(LoadObjAsync());
        StartCoroutine(LoadObjAsyncNewMethod());
    }
    IEnumerator LoadObjAsync()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
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
            var textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text));
            GameObject mesh = new OBJLoader().Load(textStream);
            mesh.name = "oldMethod";
            mesh.transform.SetParent(this.transform);
            Debug.Log(www.result);
            Debug.Log($"old method loading position: {mesh.transform.position}");
            Debug.Log($"old method loading scale: {mesh.transform.localScale}");
        }
    }
    IEnumerator LoadObjAsyncNewMethod()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
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

            GameObject meshGO = ObjReader.ObjToMeshObject(www.downloadHandler.text);

            meshGO.name = "newMethod";
            meshGO.transform.SetParent(this.transform);
            //Debug.Log(www.result);
            //Debug.Log($"new method loading position: {meshGO.transform.position}");
            //Debug.Log($"new method loading transform: {meshGO.transform.localScale}");

            Mesh mesh = meshGO.GetComponent<MeshFilter>().mesh;
            Debug.Log($"found {mesh.vertexCount} vertices");
            foreach (var item in mesh.vertices)
            {
                Debug.Log(item.x + " " + item.y+ " " + item.z);
            }
        }
    }
}
