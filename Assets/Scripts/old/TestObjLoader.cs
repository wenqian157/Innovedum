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
        url = "https://raw.githubusercontent.com/wenqian157/webServer/main/innovedum/obj/mesh_concrete_beam.obj";
        StartCoroutine(LoadObjAsyncNewMethod());
    }
    IEnumerator LoadObjAsyncNewMethod()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
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
            Logs.Instance.announce.text = www.downloadHandler.text;

            meshGO.transform.SetParent(this.transform);

            Mesh mesh = meshGO.GetComponent<MeshFilter>().mesh;
            Debug.Log($"found {mesh.vertexCount} vertices");
            int tempi = 0;
            foreach (var item in mesh.vertices)
            {
                if(tempi <3)
                {
                    Debug.Log($"unity mesh vertex: {item.x}, {item.y}, {item.z}");
                }
                tempi += 1;
            }
        }
    }
}
