using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LoadText : MonoBehaviour
{
    [SerializeField]
    public struct Text3D
    {
        public float[] pt;
        public string text;
    }
    [SerializeField]
    public struct Text3DList
    {
        public List<Text3D> text3d;
    }
    private Color textColor;
    private float fontSize;
    private float fontRes ;
    private bool lookAtCam = false;
    private List<TEXDraw3D> allText3D = new List<TEXDraw3D>();
    void Start()
    {
        fontSize = LoadInfo.instance.fontSize;
        fontRes = LoadInfo.instance.fontRes;
        textColor = LoadInfo.instance.textColor;

        LoadTextByLayer();
        StartCoroutine(FaceCam());
    }
    private void LoadTextByLayer()
    {
        for (int i = 0; i < LoadCSV.instance.myLayerObjects.Length; i++)
        {
            string name = LoadCSV.instance.myLayerObjects[i].name;
            StartCoroutine(LoadTextAsync(i+6, name)); // customized layer starting from 6
            LoadingProgress.Instance.coroutineCount++;
        }
    }
    IEnumerator LoadTextAsync(int index, string name)
    {
        string url = LoadCSV.instance.urlBase + "/text/" + name + ".json";
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
                Debug.Log("loading text 3d...");
                yield return new WaitForSeconds(0.1f);
            }
            string stringData = www.downloadHandler.text;
            if (stringData == "404: Not Found")
            {
                LoadingProgress.Instance.coroutineCount--;
                yield break;
            }
            Text3DList text3DList = JsonConvert.DeserializeObject<Text3DList>(stringData);
            GameObject textParent = new GameObject();
            textParent.transform.SetParent(transform);
            textParent.transform.localPosition = new Vector3(0, 0, 0);
            textParent.transform.localRotation = Quaternion.identity;
            textParent.transform.localScale = new Vector3(1, 1, 1);
            textParent.name = "text_" + name;
            textParent.layer = index;
            foreach (Text3D text3D in text3DList.text3d)
            {
                AddText(textParent.transform, text3D);
            }
            foreach (Transform child in textParent.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = index;
            }
            LoadingProgress.Instance.coroutineCount--;
        }
    }
    private void AddText(Transform parent, Text3D text3d)
    {
        GameObject subParent = new GameObject();
        subParent.transform.SetParent(parent);
        subParent.transform.localRotation = Quaternion.identity;
        subParent.transform.localScale = new Vector3(1, 1, 1);
        subParent.transform.localPosition = new Vector3(
            text3d.pt[0],
            text3d.pt[2],
            text3d.pt[1]
            );
        subParent.name = text3d.text;

        GameObject textGO = new GameObject();
        textGO.transform.SetParent(subParent.transform);

        TEXDraw3D latex3D = textGO.AddComponent<TEXDraw3D>();
        latex3D.text = text3d.text;
        latex3D.color = textColor;
        latex3D.size = fontSize;
        latex3D.pixelsPerUnit = fontRes;

        RectTransform rectTransform = textGO.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localPosition = new Vector3(0, 0.3f, 0);
        }
    }
    IEnumerator FaceCam()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (LoadingProgress.Instance.loadComplete)
            {
                lookAtCam = true;
                allText3D = transform.GetComponentsInChildren<TEXDraw3D>().ToList();
                break;
            }
        }
    }
    private void Update()
    {
        if (lookAtCam && allText3D.Count > 0)
        {
            foreach (TEXDraw3D textMesh in allText3D)
            {
                textMesh.transform.LookAt(
                textMesh.transform.position +
                Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up
                );
            }
        }
    }   
}
