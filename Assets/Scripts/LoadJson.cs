using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadJson : MonoBehaviour
{
    public float lineWidth = 0.05f;
    public float arrowSize = 0.1f;
    [Serializable]
    public struct LineData
    {
        public List<List<List<float>>> lines;
    }
    private void Start()
    {
        LoadJsonByLayer();
    }
    private void LoadJsonByLayer()
    {
        Debug.Log($"found {LoadCSV.instance.lineLayers.Count} line LayerObject");
        foreach (var i in LoadCSV.instance.lineLayers)
        {
            string name = LoadCSV.instance.myLayerObjects[i - 6].name;
            string material = LoadCSV.instance.myLayerObjects[i - 6].material;
            StartCoroutine(LoadLineAsync(i, name, material));
            LoadingProgress.Instance.coroutineCount++;
        }
        Debug.Log($"found {LoadCSV.instance.arrowLayers.Count}arrow LayerObject");
        foreach (int i in LoadCSV.instance.arrowLayers)
        {
            string name = LoadCSV.instance.myLayerObjects[i - 6].name;
            string material = LoadCSV.instance.myLayerObjects[i - 6].material;
            StartCoroutine(LoadArrowAsync(i, name, material));
            LoadingProgress.Instance.coroutineCount++;
        }
    }
    IEnumerator LoadLineAsync(int index, string name, string material)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(LoadCSV.instance.urlBase + "/json/" + name + ".json"))
        {
            www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"{www.error}");
                yield break;
            }
            while (!www.isDone)
            {
                Debug.Log("loading line...");
                yield return new WaitForSeconds(0.2f);
            }

            string data = www.downloadHandler.text;
            LineData lineData = JsonConvert.DeserializeObject<LineData>(data);
            GameObject lineGO = new GameObject();
            lineGO.transform.SetParent(transform);
            lineGO.name = name;
            lineGO.layer = index;
            lineGO.transform.localScale = new Vector3(1, 1, 1);
            Color lineColor = colorDict[material];
            AddLines(lineGO.transform, lineData, lineColor);
            foreach (Transform child in lineGO.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = index;
            }
            LoadingProgress.Instance.coroutineCount--;
        }
    }
    IEnumerator LoadArrowAsync(int index, string name, string material)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(LoadCSV.instance.urlBase + "/json/" + name + ".json"))
        {
            www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"{www.error}");
                yield break;
            }
            while (!www.isDone)
            {
                Debug.Log("loading arrow...");
                yield return new WaitForSeconds(0.2f);
            }

            string data = www.downloadHandler.text;
            LineData lineData = JsonConvert.DeserializeObject<LineData>(data);
            GameObject lineWithArrow = new GameObject();
            lineWithArrow.transform.SetParent(transform);
            lineWithArrow.name = name;
            lineWithArrow.layer = index;
            lineWithArrow.transform.localScale = new Vector3(1, 1, 1);
            Color lineColor = colorDict[material];
            AddArrows(lineWithArrow.transform, lineData, lineColor);
            foreach (Transform child in lineWithArrow.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = index;
            }
        }
        LoadingProgress.Instance.coroutineCount--;
    }
    private void AddLines(Transform parent, LineData lineData, Color c)
    {
        List<Vector3[]> lines = DataToLine(lineData, true);
        for (int i = 0; i < lines.Count; i++)
        {
            var line = AddLineObject(parent);
            line.name = "line";
            AddLineRenderer(line, lines[i], lineWidth, c);
        }
    }
    private void AddArrows(Transform parent, LineData lineData, Color c)
    {
        List<Vector3[]> lines = DataToLine(lineData, true);
        for (int i = 0; i < lines.Count; i++)
        {
            var line = AddLineObject(parent);
            line.name = "line";
            AddLineRenderer(line, lines[i], lineWidth, c);

            // add arrow
            GameObject arrow = new GameObject();
            arrow.name = "arrow";
            arrow.transform.parent = parent;
            arrow.transform.localPosition = new Vector3(0, 0, 0);
            arrow.transform.localRotation = Quaternion.identity;
            arrow.transform.localScale = new Vector3(1, 1, 1);

            var arrowR = arrow.AddComponent<LineRenderer>();
            Vector3 vNew = lines[i][1] - (lines[i][1] - lines[i][0]).normalized * arrowSize;
            arrowR.SetPositions(new Vector3[] { vNew, lines[i][1] });
            arrowR.material = new Material(Shader.Find("Sprites/Default"));
            arrowR.startColor = c;
            arrowR.endColor = c;
            arrowR.startWidth = arrowSize;
            arrowR.endWidth = 0;
            arrowR.useWorldSpace = false;
        }
    }
    private GameObject AddLineObject(Transform parent)
    {
        GameObject line = new GameObject();
        line.transform.parent = parent;
        line.transform.localPosition = new Vector3(0, 0, 0);
        line.transform.localRotation = Quaternion.identity;
        line.transform.localScale = new Vector3(1, 1, 1);
        return line;
    }
    private void AddLineRenderer(GameObject lineObj, Vector3[] vertices, float f, Color c)
    {
        var lineR = lineObj.AddComponent<LineRenderer>();
        lineR.SetPositions(vertices);
        lineR.material = new Material(Shader.Find("Sprites/Default"));
        lineR.startColor = c;
        lineR.endColor = c;
        lineR.startWidth = f;
        lineR.endWidth = f;
        lineR.useWorldSpace = false;
    }
    private List<Vector3[]> DataToLine(LineData lineData, bool flipYZ)
    {
        List<Vector3[]> lines = new List<Vector3[]>();
        foreach (var item in lineData.lines)
        {
            var v1 = new Vector3(item[0][0], item[0][1], item[0][2]);
            var v2 = new Vector3(item[1][0], item[1][1], item[1][2]);
            var line = new Vector3[2] { v1, v2 };
            if (flipYZ)
            {
                FlipYZ(line);
            }
            lines.Add(line);
        }
        return lines;
    }
    private void FlipYZ(Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            vertices[i] = new Vector3(v.x, v.z, v.y);
        }
    }
    private static Dictionary<string, Color> colorDict = new Dictionary<string, Color>()
    {
        { "red", Color.red },
        { "blue", Color.blue },
        { "green", Color.green },
        { "white", Color.white },
        { "black", Color.black },
        { "yellow", Color.yellow },
        { "cyan", Color.cyan },
        { "magenta", Color.magenta },
        { "gray", Color.gray },
        { "grey", Color.grey },
        { "clear", Color.clear },
        { "orange", Color.yellow }
        // Add more if needed
    };
}


