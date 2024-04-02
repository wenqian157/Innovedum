using Dummiesman;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ObjFromStream : MonoBehaviour {
	void Start () {
        string objstring = "https://people.sc.fsu.edu/~jburkardt/data/obj/lamp.obj";
        StartCoroutine(LoadObjAsync(objstring));
	}
    IEnumerator LoadObjAsync(string objstring)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(objstring))
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
            Debug.Log(textStream.Length);
            var loadedObj = new OBJLoader().Load(textStream);
        }
    }
}
