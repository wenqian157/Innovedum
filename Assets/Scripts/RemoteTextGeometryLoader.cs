using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteTextGeometryLoader : MonoBehaviour
{
    private TextGeometryAll textGeometryAll;
    private bool lookAtCam = false;
    private List<TEXDraw3D> listOfText;
    [HideInInspector]
    public bool is3DText = false;

    void Start()
    {
        StartCoroutine(ReadCSVAsync());
        StartCoroutine(getAllText3D());
    }
    IEnumerator ReadCSVAsync()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(LoadText3DAsync());
        LoadingProgress.Instance.coroutineCount++;
    }
    IEnumerator LoadText3DAsync()
    {
        string url = RemoteCSVLoader.urlBase + "/json/step_text.json";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.certificateHandler = new BypassCertificate();
            www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                LoadingProgress.Instance.coroutineCount--;
                Debug.LogError($"{www.error}");
                yield break;
            }
            while (!www.isDone)
            {
                Debug.Log("loading text geometry...");
                yield return new WaitForSeconds(0.1f);
            }
            string stringData = www.downloadHandler.text;
            if(stringData == "404: Not Found")
            {
                LoadingProgress.Instance.coroutineCount--;
                yield break;
            }
            textGeometryAll = JsonConvert.DeserializeObject<TextGeometryAll>(stringData);
            GameObject textParent = new GameObject();
            textParent.transform.SetParent(transform);
            textParent.transform.localPosition = new Vector3(0, 0, 0);
            textParent.transform.localRotation = Quaternion.identity;
            textParent.transform.localScale = new Vector3(1, 1, 1);
            textParent.name = "textParent";
            is3DText = true;
            LoadingProgress.Instance.is3DText = true;

            foreach (var textGeometry in textGeometryAll.text3d)
            {
                CreateTextGeometrySubParent(textParent, textGeometry);
            }
        }
        LoadingProgress.Instance.coroutineCount--;
    }
    private void CreateTextGeometrySubParent(GameObject parent, TextGeometry textGeometry)
    {
        GameObject textSubParent = new GameObject();
        textSubParent.transform.SetParent(parent.transform);
        textSubParent.transform.localPosition = new Vector3(0, 0, 0);
        textSubParent.transform.localRotation = Quaternion.identity;
        textSubParent.transform.localScale = new Vector3(1, 1, 1);
        textSubParent.name = textGeometry.step.ToString();

        for (int i = 0; i < textGeometry.text.Count; i++)
        {
            AddLatexGeometry(textSubParent.transform, textGeometry.text[i], textGeometry.pt[i]);
        }
    }
    private void AddLatexGeometry(Transform parent, string text, float[] pt)
    {
        GameObject textGOLocation = new GameObject();

        textGOLocation.transform.parent = parent;
        textGOLocation.transform.localPosition = new Vector3(
            pt[0],
            pt[2],
            pt[1]
            );
        textGOLocation.transform.localScale = new Vector3(1, 1, 1);
        textGOLocation.transform.localRotation = Quaternion.identity;

        GameObject textGORect = new GameObject();
        textGORect.transform.SetParent(textGOLocation.transform);

        TEXDraw3D latex3D = textGORect.AddComponent<TEXDraw3D>();
        latex3D.text = text;
        latex3D.color = Color.red;
        latex3D.size = 0.15f;
        latex3D.pixelsPerUnit = 200;

        RectTransform rectTransform = textGORect.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localPosition = new Vector3(0, 0.3f, 0);
        }
    }
    IEnumerator getAllText3D()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (LoadingProgress.Instance.loadComplete)
            {
                lookAtCam = true;
                break;
            }
        }
    }
    private void Update()
    {
        if (is3DText && lookAtCam)
        {
            listOfText = transform.GetComponentsInChildren<TEXDraw3D>().ToList();
            foreach (TEXDraw3D textMesh in listOfText)
            {
                textMesh.transform.LookAt(
                textMesh.transform.position +
                Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up
                );
            }
        }
    }
    [SerializeField]
    public struct TextGeometry
    {
        public int step;
        public List<string> text;
        public List<float[]> pt;
    }
    [SerializeField]
    public struct TextGeometryAll
    {
        public List<TextGeometry> text3d;
    }
}
