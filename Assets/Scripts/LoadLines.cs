using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

[ExecuteInEditMode]
public class LoadLines : MonoBehaviour
{
    public string fileName;
    public float lineWidth = 0.2f;
    private LinesFromRhino data;

    private void Start()
    {
        OnClickLoadLines();
    }
    public void OnClickLoadLines()
    {
        LoadFromJson();
        AddLineRenderers(true);  
    }
    public void DestroyChildren()
    {
        int index = transform.childCount;
        for (int i = 0; i < index; i++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    private void LoadFromJson()
    {
        string path = Application.dataPath + $"/Resources/" + fileName;

        try
        {
            Debug.Log("read file");
            string jsonString = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<LinesFromRhino>(jsonString);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            //throw new ArgumentException($"{path}IsNullOrEmpty");
        }
    }
    private void AddLineRenderers(bool flipYZ)
    {
        DestroyChildren();

        List<Vector3[]> lines = new List<Vector3[]>();
        foreach (var item in data.lines)
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
        for (int i = 0; i < lines.Count; i++)
        {
            var line = AddLineObject();
            line.name = "line";
            AddLineRenderer(line, lines[i], lineWidth, Color.blue);
        }
    }
    private void FlipYZ(Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            vertices[i] = new Vector3(v.x, v.z, v.y);
        }
    }
    private GameObject AddLineObject()
    {
        GameObject line = new GameObject();
        line.transform.parent = transform;
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
    [Serializable]
    struct LinesFromRhino
    {
        public List<List<List<float>>> lines;
    }
}
