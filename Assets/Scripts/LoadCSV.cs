using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class LoadCSV : MonoBehaviour
{
    public static LoadCSV instance;
    [HideInInspector]
    public string urlBase;
    [HideInInspector]
    public int layerCount;
    [HideInInspector]
    public int stepCount;
    [HideInInspector]
    public List<int> meshLayers = new List<int>();
    [HideInInspector]
    public List<int> lineLayers = new List<int>();
    [HideInInspector]
    public List<int> arrowLayers = new List<int>();

    [Serializable]
    public class LayerObject
    {
        public int index;
        public string name;
        public string type;
        public string material;
        public string displayingName;
    }
    [Serializable]
    public class StoryObject
    {
        public string[] stepNameArray;
        public List<int>[] layerFilters;
        public string[] stepInfoArray;
    }
    [HideInInspector]
    public LayerObject[] myLayerObjects;
    [HideInInspector]
    public StoryObject myStoryObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance == this) return;
            Destroy(instance.gameObject);
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    public void Reset()
    {
        
    }
    public void Load()
    {
        StartCoroutine(LoadAsync());
    }
    public IEnumerator LoadAsync()
    {
        string urlCSV = urlBase + "/csv/info.csv";
        using (UnityWebRequest www = UnityWebRequest.Get(urlCSV))
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
                Debug.Log("loading csv...");
                Logs.Instance.announce.text = "loading csv...";
                yield return new WaitForSeconds(0.1f);
            }
            string stringData = www.downloadHandler.text;
            string[] data = stringData.Split(new string[] {"\n" }, StringSplitOptions.None);

            string[] datahead = data[0].Split(new string[] { "," }, StringSplitOptions.None); // first row is head
            stepCount = datahead.Length - 5; // first 5 columns are index, layername, displaying name, type, materials
            myStoryObject.stepNameArray = datahead.Skip(5).ToArray();
            List<List<int>> filtersTemp = new List<List<int>>();

            layerCount = data.Length - 1; // first row is title title row
            myLayerObjects = new LayerObject[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                myLayerObjects[i] = new LayerObject();
                string[] subdata = data[i + 1].Split(new string[] { "," }, StringSplitOptions.None);

                myLayerObjects[i].index = i + 6; // custome layer starting from 6
                myLayerObjects[i].name = subdata[1];
                myLayerObjects[i].displayingName = subdata[2];
                myLayerObjects[i].type = subdata[3];
                myLayerObjects[i].material = subdata[4];

                if(myLayerObjects[i].type == "mesh")
                {
                    meshLayers.Add(myLayerObjects[i].index);
                }
                else if (myLayerObjects[i].type == "line")
                {
                    lineLayers.Add(myLayerObjects[i].index);
                }
                else if (myLayerObjects[i].type == "arrow")
                {
                    arrowLayers.Add(myLayerObjects[i].index);
                }

                filtersTemp.Add(subdata.Skip(5).Select(int.Parse).ToList());
            }

            var transposed = Enumerable.Range(0, filtersTemp[0].Count).
                Select(col => filtersTemp.Select(row =>row[col]).ToList()).ToList();

            myStoryObject.layerFilters = new List<int>[transposed.Count()];
            for (int i = 0; i < myStoryObject.layerFilters.Count(); i++)
            {
                myStoryObject.layerFilters[i] = new List<int>();
                myStoryObject.layerFilters[i].Add(0); // add default layer
                for (int j = 0; j < transposed[i].Count; j++)
                {
                    if (transposed[i][j] == 1)
                    {
                        myStoryObject.layerFilters[i].Add(myLayerObjects[j].index);
                    }
                }
            }
            LoadMain();
        }
    }
    private void LoadMain()
    {
        Debug.Log("load main scene...");
        Logs.Instance.announce.text = "load main scene...";
        SceneManager.LoadScene("MainDev", LoadSceneMode.Single);
    }
}
