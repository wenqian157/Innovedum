using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RemoteInfoLoader : MonoBehaviour
{
    public List<string> projectUrlList = new List<string>();
    public static string urlBase;
    public static RemoteInfoLoader Instance;
    private string urlInfo;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance == this) return;
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }
    public void OnUIReadInfo(int id)
    {
        urlBase = projectUrlList[id];
        if (urlBase is null) return;
        urlInfo = urlBase + "/txt/info.txt";
        StartCoroutine(ReadInfo(urlInfo));

        RemoteCSVLoader.urlBase = urlBase;
        RemoteCSVLoader.projectID = id;

        switch (id)
        {
            case 0:
                RemoteCSVLoader.displayingScale = 1;
                break;
            case 1:
                RemoteCSVLoader.displayingScale = 0.5f;
                break;
        }
    }
    public static IEnumerator ReadInfo(string url)
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
                Debug.Log("loading project information...");
                Logs.Instance.announce.text = "loading project information...";
                yield return new WaitForSeconds(0.2f);
            }

            Debug.Log($"request is done: {www.downloadHandler.text}");
            Debug.Log(www.result);
            Debug.Log(www.downloadProgress);
            string stringData = www.downloadHandler.text;
            Logs.Instance.announce.text = stringData;
        }
    }
}
