using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteTextGeometryLoader : MonoBehaviour
{
    private TextGeometry textGeometry;
    private string testUrl = "https://raw.githubusercontent.com/wenqian157/Innovedum/refs/heads/ar/OnlineResources/json/text_testing.json";
    private GameObject textGO;
    private bool lookAtCam = false;

    void Start()
    {
        StartCoroutine(LoadTextGeometryAsync(testUrl));
    }
    IEnumerator LoadTextGeometryAsync(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.certificateHandler = new BypassCertificate();
            www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"{www.error}");
                yield break;
            }
            while (!www.isDone)
            {
                Debug.Log("loading text geometry...");
                Logs.Instance.announce.text = "loading test geometry...";
                yield return new WaitForSeconds(0.2f);
            }
            string stringData = www.downloadHandler.text;
            textGeometry = JsonConvert.DeserializeObject<TextGeometry>(stringData);
            AddTextGeometry(this.transform, textGeometry);
            lookAtCam = true;
        }
    }
    private void AddTextGeometry(Transform parent, TextGeometry textGeometry)
    {
        textGO = new GameObject();
        textGO.name = "text";
        textGO.transform.parent = parent;
        textGO.transform.localPosition = new Vector3(
            textGeometry.pt[0],
            textGeometry.pt[2],
            textGeometry.pt[1]
            );
        textGO.transform.localScale = new Vector3(1, 1, 1);
        textGO.transform.localRotation = Quaternion.identity;

        TextMeshPro textMesh = textGO.AddComponent<TextMeshPro>();
        textMesh.text = textGeometry.text;
        textMesh.fontSize = 10;
    }
    private void Update()
    {
        if (lookAtCam)
        {
            textGO.transform.LookAt(
                textGO.transform.position +
                Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation* Vector3.up
                );
        }
    }
    [SerializeField]
    public struct TextGeometry
    {
        public string text;
        public float[] pt;
    }
}
