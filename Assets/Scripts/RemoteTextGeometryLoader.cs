using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteTextGeometryLoader : MonoBehaviour
{
    private TextGeometry textGeometry;
    private bool lookAtCam = true;
    private List<GameObject> textGOList;
    private List<TEXDraw3D> listOfText;
    private int latexCount = 0;
    private int tempCount = -1;
    private bool loadComplete = false;

    void Start()
    {
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        yield return new WaitForSeconds(1.0f);
        RemoteLoadAllText();
    }
    private void RemoteLoadAllText()
    {
        Debug.Log($"found {RemoteCSVLoader.objWithText.Count}line LayerObject");
        foreach (var layerIndex in RemoteCSVLoader.objWithText)
        {
            string layerName = RemoteCSVLoader.myLayerObjects[layerIndex - 6].name;
            StartCoroutine(LoadTextGeometryAsync(layerIndex, layerName));
        }
    }
    IEnumerator LoadTextGeometryAsync(int layerIndex, string layerName)
    {
        string url = RemoteCSVLoader.urlBase + "/json/" + layerName + "_text.json";
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
                yield return new WaitForSeconds(0.1f);
            }
            string stringData = www.downloadHandler.text;
            textGeometry = JsonConvert.DeserializeObject<TextGeometry>(stringData);

            // create one GO for all txt in a layer
            GameObject textGOLayerParent = new GameObject();
            textGOLayerParent.transform.SetParent(transform);
            textGOLayerParent.transform.localPosition = new Vector3(0, 0, 0);
            textGOLayerParent.transform.localRotation = Quaternion.identity;
            textGOLayerParent.transform.localScale = new Vector3(1, 1, 1);
            textGOLayerParent.name = layerName + "_text";
            textGOLayerParent.layer = layerIndex;

            for (int i = 0; i < textGeometry.text.Count; i++)
            {
                AddLatexGeometry(
                    textGOLayerParent.transform,
                    textGeometry.text[i],
                    textGeometry.pt[i],
                    layerIndex
                    );
                latexCount += 1;
            }
        }
        StoryController.instance.UpdateLayerMask();
    }
    private void AddTextGeometry(Transform parent, string text, float[] pt, int layerIndex)
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

        TextMeshPro textMesh = textGORect.AddComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.fontSize = 1.5f;
        textMesh.color = Color.red;

        RectTransform rectTransform = textGORect.GetComponent<RectTransform>();
        if(rectTransform != null)
        {
            rectTransform.localPosition = new Vector3(0, 0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(2, 0.4f);
        }

        List<Transform> allChildren = parent.gameObject.GetComponentsInChildren<Transform>().ToList();
        foreach (Transform trans in allChildren)
        {
            trans.gameObject.layer = layerIndex;
        }
    }
    private void AddLatexGeometry(Transform parent, string text, float[] pt, int layerIndex)
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

        List<Transform> allChildren = parent.gameObject.GetComponentsInChildren<Transform>().ToList();
        foreach (Transform trans in allChildren)
        {
            trans.gameObject.layer = layerIndex;
        }
    }
    private void Update()
    {
        while (tempCount != latexCount)
        {
            List<TEXDraw3D> tempList = transform.GetComponentsInChildren<TEXDraw3D>().ToList();
            tempCount = tempList.Count;
            if(tempCount == latexCount)
            {
                Debug.Log("text geometry load complete");
                loadComplete = true;
                listOfText = tempList;
                break;
            }
        }

        if (loadComplete)
        {
            if (lookAtCam)
            {
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
    }
    public void OnClickTextOnOff()
    {
        foreach (TEXDraw3D textMesh in listOfText)
        {
            textMesh.gameObject.SetActive(!textMesh.gameObject.activeSelf);
        }
    }
    [SerializeField]
    public struct TextGeometry
    {
        public List<string> text;
        public List<float[]> pt;
    }
}
