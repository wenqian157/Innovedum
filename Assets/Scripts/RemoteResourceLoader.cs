using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class RemoteResourceLoader : MonoBehaviour
{
    public TMP_Text URLAddressText;
    private string address;
    private void Awake()
    {
        address = "https://github.com/wenqian157/Innovedum/tree/main/OnlineResources";
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GetComponent<RemoteResourceLoader>());
    }
    public void OnClickLoadScene()
    {
        // todo verify url address
        //if ()
        //{

        //}
        address = URLAddressText.text;
        StartCoroutine(VerifyWebAddress(address));
    }
    public void OpenMainScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    private IEnumerator VerifyWebAddress(string url)
    {
        using(UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if(www.result != UnityWebRequest.Result.Success) { }
            Debug.LogError($"{www.error}");
            yield break;
        }

        //var wwwww = UnityWebRequest.Get(url);
        //Debug.Log(url);
        //yield return www.SendWebRequest();
        //if(www.result != UnityWebRequest.Result.Success)
        //{
        //    Debug.LogError($"{www.error}");
        //    yield break;
        //}
        //OpenMainScene();
    }
}
