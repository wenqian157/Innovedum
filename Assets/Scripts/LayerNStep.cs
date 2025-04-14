using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LayerNStep : MonoBehaviour
{
    public static LayerNStep instance;

    public string urlBase;
    [HideInInspector]
    public string urlLayer;
    [HideInInspector]
    public string urlStep;
    [HideInInspector]
    public LayerObjects layerObjects;
    [HideInInspector]
    public StepObjects stepObjects;

    [Serializable]
    public class LayerObject
    {
        public string name;
        public List<int> content;
        public List<int> color;
        public string material;
    }
    [Serializable]
    public class LayerObjects
    {
        public List<LayerObject> layerObjects;
    }
    [Serializable]
    public class StepObjects
    {
        public List<List<int>> stepContents;
    }

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
    public void OnUILoadLayerNStep()
    {
        if (urlBase is null) return;
        urlLayer = urlBase + "layers.json";
        urlStep = urlBase + "Steps.json";
        StartCoroutine(ReadLayerJson(urlLayer));
    }
    public void Reset()
    {
        layerObjects = new LayerObjects();
        stepObjects = new StepObjects();
    }
    public IEnumerator ReadLayerJson(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"{www.error}");
                yield break;
            }
            while (!www.isDone)
            {
                Debug.Log("load layers...");
                yield return new WaitForSeconds(0.1f);
            }

            string stringData = www.downloadHandler.text;
            layerObjects = JsonConvert.DeserializeObject<LayerObjects>(stringData);

            StartCoroutine(ReadStepJson(urlStep));
        }
    }
    public IEnumerator ReadStepJson(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"{www.error}");
                yield break;
            }
            while (!www.isDone)
            {
                Debug.Log("load steps...");
                yield return new WaitForSeconds(0.1f);
            }
            string stringData = www.downloadHandler.text;
            stepObjects = JsonConvert.DeserializeObject<StepObjects>(stringData);

            Debug.Log("load main scene...");
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
    }
}
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

