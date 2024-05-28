using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteJsonLoader : MonoBehaviour
{
    public float lineWidth = 0.05f;
    public float arrowSize = 0.1f;
    private LinesFromRhino lineData;
    private List<LinesFromRhino> lineDatas;
    void Start()
    {
        StartCoroutine(ReadCSVAsync());
    }
    public void OnClickLoadJson()
    {
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        yield return new WaitForSeconds(1.0f);
        RemoteLoadLine();
    }
    private void RemoteLoadLine()
    {
        Debug.Log($"found {RemoteCSVLoader.linesLayers.Count}line LayerObject");
        foreach (int layerIndex in RemoteCSVLoader.linesLayers)
        {
            string layerName = RemoteCSVLoader.myLayerObjects[layerIndex - 6].name;
            string layerMaterial = RemoteCSVLoader.myLayerObjects[layerIndex - 6].material;
            StartCoroutine(LoadLineJsonAsync(layerIndex, layerName, layerMaterial));
        }

        Debug.Log($"found {RemoteCSVLoader.linesWithArrowLayers.Count}arrow LayerObject");
        foreach (int layerIndex in RemoteCSVLoader.linesWithArrowLayers)
        {
            string layerName = RemoteCSVLoader.myLayerObjects[layerIndex - 6].name;
            string layerMaterial = RemoteCSVLoader.myLayerObjects[layerIndex - 6].material;
            StartCoroutine(LoadArrowJsonAsync(layerIndex, layerName, layerMaterial));
        }
    }

    IEnumerator LoadLineJsonAsync(int layerIndex, string layerName, string layerMaterial)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(RemoteCSVLoader.urlBase + "/json/" + layerName + ".json"))
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

            string stringData = www.downloadHandler.text;
            lineData = JsonConvert.DeserializeObject<LinesFromRhino>(stringData);
            GameObject line = new GameObject();
            line.transform.SetParent(transform);
            line.name = layerName;
            line.layer = layerIndex;
            Color lineColor = GetColorFromName(layerMaterial);
            AddLines(line.transform, lineData, lineColor);
            foreach (Transform child in line.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layerIndex;
            }
        }
    }
    IEnumerator LoadArrowJsonAsync(int layerIndex, string layerName, string layerMaterial)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(RemoteCSVLoader.urlBase + "/json/" + layerName + ".json"))
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

            string stringData = www.downloadHandler.text;
            lineData = JsonConvert.DeserializeObject<LinesFromRhino>(stringData);
            GameObject lineWithArrow = new GameObject();
            lineWithArrow.transform.SetParent(transform);
            lineWithArrow.name = layerName;
            lineWithArrow.layer = layerIndex;
            Color lineColor = GetColorFromName(layerMaterial);
            AddLinesWithArrow(lineWithArrow.transform, lineData, lineColor);
            foreach (Transform child in lineWithArrow.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layerIndex;
            }
            }
    }
    private void AddLines(Transform parent, LinesFromRhino lineData, Color c)
    {
        List<Vector3[]> lines = ReadLines(lineData, true);
        for (int i = 0; i < lines.Count; i++)
        {
            var line = AddLineObject(parent);
            line.name = "line";
            AddLineRenderer(line, lines[i], lineWidth, c);
        }
    }
    private void AddLinesWithArrow(Transform parent, LinesFromRhino lineData, Color c)
    {
        List<Vector3[]> lines = ReadLines(lineData, true);
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
            Vector3 vNew = (lines[i][0] - lines[i][1]).normalized * arrowSize * 2 + lines[i][0];
            arrowR.SetPositions(new Vector3[] { vNew, lines[i][0] });
            arrowR.material = new Material(Shader.Find("Sprites/Default"));
            arrowR.startColor = c;
            arrowR.endColor = c;
            arrowR.startWidth = 0;
            arrowR.endWidth = arrowSize;
            arrowR.useWorldSpace = false;
        }
    }
    private List<Vector3[]> ReadLines(LinesFromRhino lineData, bool flipYZ)
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
    private GameObject AddLineObject(Transform parent)
    {
        GameObject line = new GameObject();
        line.transform.parent = parent;
        line.transform.localPosition = new Vector3(0, 0, 0);
        line.transform.localRotation = Quaternion.identity;
        line.transform.localScale = new Vector3(1, 1, 1);
        return line;
    }
    private void FlipYZ(Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            vertices[i] = new Vector3(v.x, v.z, v.y);
        }
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
    [Serializable]
    public struct LinesFromRhino
    {
        public List<List<List<float>>> lines;
    }
    private Color GetColorFromName(string colorName)
    {
        Color resultColor = new Color();
        switch (colorName)
        {
            default:
                resultColor = Color.white;
                break;
            case "blue":
                resultColor = Color.blue;
                break;
            case "green":
                resultColor = Color.green;
                break;
            case "red":
                resultColor = Color.red;
                break;
            case "yellow":
                resultColor = Color.yellow;
                break;
            case "cyan":
                resultColor = Color.cyan;
                break;
            case "magenta":
                resultColor = Color.magenta;
                break;
        }
        return resultColor;
    }
}
