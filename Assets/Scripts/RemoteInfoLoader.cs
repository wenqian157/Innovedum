using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RemoteInfoLoader : MonoBehaviour
{
    public string projectUrl = "https://gitlab.ethz.ch/ibk-kfm-public/xr-apps/-/raw/main/examples/2_torsion/OnlineResources";
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
        urlBase = projectUrl;
    }
    public void OnUIReadInfo()
    {
        urlInfo = urlBase + "/txt/info.txt";
        StartCoroutine(ReadInfo(urlInfo));
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

            string stringData = www.downloadHandler.text;
            Logs.Instance.announce.text = stringData;
        }
    }
}
