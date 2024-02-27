using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class RemoteJsonLoader : MonoBehaviour
{
    public float lineWidth = 0.05f;
    public float arrowSize = 0.1f;
    public Color lineColorA = Color.blue;
    public Color lineColorB = Color.yellow;
    private LinesFromRhino lineData;
    void Start()
    {
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        while (RemoteCSVLoader.StoryLine.layerFilters.Length == 0)
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("reading cvs file...");
        }
        RemoteLoadLine();
    }
    private void RemoteLoadLine()
    {
        Debug.Log($"found {RemoteCSVLoader.linesLayers.Count} LayerObject");
        foreach (int layerIndex in RemoteCSVLoader.linesLayers)
        {
            string layerName = RemoteCSVLoader.myLayerObjects[layerIndex - 6].name;
            LinesFromRhino lineData = LoadLineJson(layerIndex, layerName);

            GameObject line = new GameObject();
            line.transform.SetParent(transform);
            line.name = layerName;
            line.layer = layerIndex;
            AddLines(line.transform, lineData, lineColorA);
            foreach (Transform child in line.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layerIndex;
            }
        }

        Debug.Log($"found {RemoteCSVLoader.linesWithArrowLayers.Count} LayerObject");
        foreach (int layerIndex in RemoteCSVLoader.linesWithArrowLayers)
        {
            string layerName = RemoteCSVLoader.myLayerObjects[layerIndex - 6].name;
            LinesFromRhino lineData = LoadLineJson(layerIndex, layerName);

            GameObject lineWithArrow = new GameObject();
            lineWithArrow.transform.SetParent(transform);
            lineWithArrow.name = layerName;
            lineWithArrow.layer = layerIndex;
            AddLinesWithArrow(lineWithArrow.transform, lineData, lineColorB);
            foreach (Transform child in lineWithArrow.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = layerIndex;
            }
        }
    }
    private LinesFromRhino LoadLineJson(int layerIndex, string layerName)
    {
        var www = new WWW(RemoteCSVLoader.urlBase + "/json/" + layerName + ".json");
        while (!www.isDone) System.Threading.Thread.Sleep(1);
        try
        {
            string jsonString = www.text;
            lineData = JsonConvert.DeserializeObject<LinesFromRhino>(jsonString);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex, this);
        }
        return lineData;
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
    struct LinesFromRhino
    {
        public List<List<List<float>>> lines;
    }
}
