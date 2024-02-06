using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class RemoteResourceLoader : MonoBehaviour
{
    public TMP_InputField inputField;
    public static string urlAddress;
    private void Awake()
    {
        urlAddress = "https://github.com/wenqian157/Innovedum/tree/main/OnlineResources";
        DontDestroyOnLoad(gameObject);
    }
    public void OnClickLoadScene()
    {
        urlAddress = inputField.text;
        StartCoroutine(VerifyWebAddress(urlAddress));
    }
    public void OpenMainScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    private IEnumerator VerifyWebAddress(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.certificateHandler = new BypassCertificate();
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"{www.error}");
                yield break;
            }
            Debug.Log("Found remote resources");
        }
        OpenMainScene();
    }
}
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
