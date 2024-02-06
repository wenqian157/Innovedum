using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadModel : MonoBehaviour
{
    //private void Start()
    //{
    //    StartCoroutine(ReadCSVAsync());
    //}
    //IEnumerator ReadCSVAsync()
    //{
    //    while(CSVReader.StoryLine.layerFilters.Length == 0)
    //    {
    //        yield return new WaitForSeconds(1.0f);
    //        Debug.Log("reading cvs file...");
    //    }
    //    LoadObj();
    //    LoadLines();
    //    LoadArrows();
    //}
    //private void LoadObj()
    //{
    //    foreach (int layerIndex in CSVReader.objLayers)
    //    {
    //        string layerName = CSVReader.myLayerObjects[layerIndex - 6].name;
    //        GameObject mesh = Resources.Load<GameObject>($"obj/{layerName}");
    //        mesh = Instantiate(mesh, this.transform);
    //        mesh.layer = layerIndex;
    //        foreach (Transform child in mesh.GetComponentsInChildren<Transform>())
    //        {
    //            child.gameObject.layer = layerIndex;
    //        }
    //    }
    //}
    //private void LoadLines()
    //{
    //    foreach (int layerIndex in CSVReader.linesLayers)
    //    {
    //        string layerName = CSVReader.myLayerObjects[layerIndex - 6].name;
    //        GameObject lines = new GameObject();
    //        lines = Instantiate(lines, this.transform);
    //        lines.name = layerName;
    //        lines.layer = layerIndex;
    //        LoadLines loadLines = lines.AddComponent<LoadLines>();
    //        loadLines.fileName = $"json/{layerName}.json";
    //        object[] paras = new object[2] { lines, layerIndex };
    //        StartCoroutine("ChangeChildNameAsync", paras);
    //    }
    //}
    //private void LoadArrows()
    //{
    //    foreach (int layerIndex in CSVReader.linesWithArrowLayers)
    //    {
    //        string layerName = CSVReader.myLayerObjects[layerIndex - 6].name;
    //        GameObject arrows = new GameObject();
    //        arrows = Instantiate(arrows, this.transform);
    //        arrows.name = layerName;
    //        arrows.layer = layerIndex;
    //        LoadLines loadLines = arrows.AddComponent<LoadLines>();
    //        loadLines.fileName = $"json/{layerName}.json";
    //        loadLines.arrow = true;
    //        object[] paras = new object[2] { arrows, layerIndex };
    //        StartCoroutine("ChangeChildNameAsync", paras);
    //    }
    //}
    //IEnumerator ChangeChildNameAsync(object[] paras)
    //{
    //    GameObject parent = (GameObject)paras[0];
    //    int layerIndex = (int)paras[1];
    //    while (parent.GetComponent<LoadLines>().finish == false)
    //    {
    //        yield return new WaitForSeconds(1f);
    //    }

    //    foreach (Transform child in parent.GetComponentsInChildren<Transform>())
    //    {
    //        child.gameObject.layer = layerIndex;
    //    }
    //}
}
