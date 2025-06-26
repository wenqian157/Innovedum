using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadMD : MonoBehaviour
{
    public static LoadMD instance;
    public TEXDraw texDraw;
    private string[] data;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(LoadMDAsync());
        LoadingProgress.Instance.coroutineCount++;
    }
    private IEnumerator LoadMDAsync()
    {
        yield return new WaitForSeconds(0.2f);
        using (UnityWebRequest www = UnityWebRequest.Get(LoadCSV.instance.urlBase + "/md" + "/formula.md"))
        {
            www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"{www.error}");
                yield break;
            }
            while (!www.isDone)
            {
                Debug.Log("loading md...");
                yield return new WaitForSeconds(0.1f);
            }
            string stringData = www.downloadHandler.text;
            data = stringData.Split(new string[] { "====" }, StringSplitOptions.None);
            UpdateMD(0);
            LoadingProgress.Instance.coroutineCount--;
        }
    }
    public void UpdateMD(int step)
    {
        string currentText = data[step].Substring(4);
        texDraw.text = currentText;
    }
}
