using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteCSVLoader : MonoBehaviour
{
    public int steps;
    public static int stepCount;
    public static int layerCount;
    public static List<int> objLayers = new List<int>();
    public static List<int> linesLayers = new List<int>();
    public static List<int> linesWithArrowLayers = new List<int>();
    private string urlBase;
    private string urlCSV;
    public class LayerObject
    {
        public int index;
        public string name;
        public string contentType;
        public string material;
    }
    public static LayerObject[] myLayerObjects;
    [Serializable]
    public static class StoryLine
    {
        public static string[] layerNameArray;
        public static List<int>[] layerFilters;
    }
    private void Awake()
    {
        urlBase = RemoteResourceLoader.urlAddress;
        //urlCSV = string.Format("{0}/{1}", urlBase, "layerInfo.csv");
        urlCSV = urlBase;
        StartCoroutine(ReadCSV());
    }
    public IEnumerator ReadCSV()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(urlCSV))
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
            Debug.Log(www.downloadHandler.text);
            string stringData = www.downloadHandler.text;
            string[] data = stringData.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

            layerCount = data.Length / (steps + 4) - 1;
            myLayerObjects = new LayerObject[layerCount];
            StoryLine.layerFilters = new List<int>[steps];

            for (int i = 0; i < layerCount; i++)
            {
                myLayerObjects[i] = new LayerObject();
                myLayerObjects[i].index = i + 6; // custome layer starting from 6
                myLayerObjects[i].name = data[(steps + 4) * (i + 1) + 1];
                myLayerObjects[i].contentType = data[(steps + 4) * (i + 1) + 2];
                myLayerObjects[i].material = data[(steps + 4) * (i + 1) + 3];
                //StoryLine.layerNameArray[i] = myLayerObjects[i].name;

                if (myLayerObjects[i].contentType == "mesh")
                {
                    objLayers.Add(myLayerObjects[i].index);
                }
                else if (myLayerObjects[i].contentType == "lines")
                {
                    linesLayers.Add(myLayerObjects[i].index);
                }
                else if (myLayerObjects[i].contentType == "arrow")
                {
                    linesWithArrowLayers.Add(myLayerObjects[i].index);
                }
            }

            for (int j = 0; j < steps; j++)
            {
                StoryLine.layerFilters[j] = new List<int>();
                StoryLine.layerFilters[j].Add(0); // add default layer
                for (int i = 0; i < layerCount; i++)
                {
                    if (int.Parse(data[(steps + 4) * (i + 1) + 4 + j]) == 1)
                    {
                        StoryLine.layerFilters[j].Add(myLayerObjects[i].index);
                    }
                }
                //Debug.Log(string.Join(", ", StoryLine.layerFilters[j]));
            }
        }
    }
}

