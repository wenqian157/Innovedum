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

    public string projectUrl = "https://raw.githubusercontent.com/wenqian157/Innovedum/main/OnlineResources";
    public static string urlBase;
    private string urlCSVLayer;
    private string urlCSVStory;
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
        DontDestroyOnLoad(gameObject);
        urlBase = projectUrl;
    }
    public void OnClickLoadScene()
    {
        //urlBase = inputField.text;
        urlCSVLayer = urlBase + "/layerInfo.csv";
        urlCSVStory = urlBase + "/storyInfo.csv";
        StartCoroutine(ReadCSVLayer(urlCSVLayer));
    }
    public IEnumerator ReadCSVLayer(string url)
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
                Debug.Log("loading...");
                yield return new WaitForSeconds(0.2f);
            }
            //Debug.Log(www.downloadHandler.text); 
            string stringData = www.downloadHandler.text;
            string[] data = stringData.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

            layerCount = data.Length / 4 - 1;
            myLayerObjects = new LayerObject[layerCount];
            Debug.Log(layerCount);
            for (int i = 0; i < layerCount; i++)
            {
                myLayerObjects[i] = new LayerObject();
                myLayerObjects[i].index = i + 6; // custome layer starting from 6
                myLayerObjects[i].name = data[ 4 * (i + 1) + 1];
                myLayerObjects[i].contentType = data[4 * (i + 1) + 2];
                myLayerObjects[i].material = data[4 * (i + 1) + 3];
                

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
            StartCoroutine(ReadCSVStory(urlCSVStory));
        }
    }
    public IEnumerator ReadCSVStory(string url)
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
                Debug.Log("loading...");
                yield return new WaitForSeconds(0.2f);
            }
            string stringData = www.downloadHandler.text;
            string[] data = stringData.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

            storyCount = data.Length / (layerCount + 3);
            StoryLine.layerFilters = new List<int>[storyCount];
            
            for (int j = 0; j < storyCount; j++)
            {
                StoryLine.layerFilters[j] = new List<int>();
                StoryLine.layerFilters[j].Add(0); // add default layer
                for (int i = 0; i < layerCount; i++)
                {
                    if (int.Parse(data[layerCount * (i + 1) + j]) == 0)
                    {
                        StoryLine.layerFilters[j].Add(myLayerObjects[i].index);
                    }
                }
                Debug.Log(string.Join(", ", StoryLine.layerFilters[j]));
            }
            //StoryLine.layerNameArray[i] = myLayerObjects[i].name;
            //StoryLine.layerNameArray = new string[steps];
            OpenMainScene();
        }
    }
    public void OpenMainScene()
    {
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

