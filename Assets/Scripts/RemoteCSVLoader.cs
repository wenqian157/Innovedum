using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RemoteCSVLoader : MonoBehaviour
{
    //public TMP_InputField inputField;

    public static int storyCount;
    public static int layerCount;
    public static List<int> objLayers = new List<int>();
    public static List<int> linesLayers = new List<int>();
    public static List<int> linesWithArrowLayers = new List<int>();

    public static string urlBase;
    private static string urlCSVLayer;
    private static string urlCSVStory;
    public static RemoteCSVLoader instance;
    
    public class LayerObject
    {
        public int index;
        public string name;
        public string contentType;
        public string material;
        public string displayName;
    }
    public static LayerObject[] myLayerObjects; 
    [Serializable]
    public static class StoryLine 
    {
        public static string[] stepNameArray;
        public static List<int>[] layerFilters;
        public static string[] stepInfoArray;
    }
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static void OnUILoadScene()
    {
        urlBase = RemoteInfoLoader.Instance.projectUrl;
        urlCSVLayer = urlBase + "/csv/layerInfo.csv";
        urlCSVStory = urlBase + "/csv/storyInfo.csv";
        
        instance.StartCoroutine(ReadCSVLayer(urlCSVLayer));
    }
    public static IEnumerator ReadCSVLayer(string url)
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
                Debug.Log("loading layer csv...");
                Logs.Instance.announce.text = "loading layer csv...";
                yield return new WaitForSeconds(0.2f);
            }
            string stringData = www.downloadHandler.text;

            string[] data = stringData.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

            layerCount = data.Length / 5 - 1; // 5 is the column numbers in the csv: index, name, type, material, display name
            myLayerObjects = new LayerObject[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                myLayerObjects[i] = new LayerObject();
                myLayerObjects[i].index = i + 6; // custome layer starting from 6
                myLayerObjects[i].name = data[ 5 * (i + 1) + 1];
                myLayerObjects[i].contentType = data[5 * (i + 1) + 2];
                // substring is because of a bug in unity
                //myLayerObjects[i].material = data[5 * (i + 1) + 3].Substring(0, data[5 * (i + 1) + 3].Length-1);
                myLayerObjects[i].material = data[5 * (i + 1) + 3];
                myLayerObjects[i].displayName = data[5 * (i + 1) + 4];

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
            instance.StartCoroutine(ReadCSVStory(urlCSVStory));
        }
    }
    public static IEnumerator ReadCSVStory(string url)
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
                Debug.Log("loading story csv...");
                Logs.Instance.announce.text = "loading story csv...";
                yield return new WaitForSeconds(0.2f);
            }
            string stringData = www.downloadHandler.text;
            string[] data = stringData.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

            Debug.Log("load story data: " + data.Length);

            storyCount = data.Length / (layerCount + 2);
            StoryLine.layerFilters = new List<int>[storyCount];
            
            for (int j = 0; j < storyCount; j++)
            {
                StoryLine.layerFilters[j] = new List<int>();
                StoryLine.layerFilters[j].Add(0); // add default layer
                for (int i = 0; i < layerCount; i++)
                {
                    if (int.Parse(data[storyCount * (i + 1) + j]) == 0)
                    {
                        StoryLine.layerFilters[j].Add(myLayerObjects[i].index);
                    }
                }
            }
            StoryLine.stepNameArray = new string[storyCount];
            for (int j = 0; j < storyCount; j++)
            {
                StoryLine.stepNameArray[j] = data[storyCount * (layerCount + 1) + j];
            }
        }
        OnUILoadMain();
    }
    private static void OnUILoadMain()
    {
        Debug.Log("load main scene...");
        Logs.Instance.announce.text = "load main scene...";
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

