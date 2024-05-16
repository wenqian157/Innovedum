using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RemoteTextLoader : MonoBehaviour
{
    public TEXDraw texDraw;
    public string[] data;
    void Start()
    {
        StartCoroutine(ReadText());
    }
    public void OnUILoadText()
    {
        StartCoroutine(ReadText());
    }
    public IEnumerator ReadText()
    {
        yield return new WaitForSeconds(1.0f);
        using (UnityWebRequest www = UnityWebRequest.Get(RemoteCSVLoader.urlBase + "/md" + "/formula.md"))
        {
            www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"{www.error}");
                yield break;
            }
            while (!www.isDone)
            {
                Debug.Log("loading text...");
                yield return new WaitForSeconds(0.2f);
            }
            string stringData = www.downloadHandler.text;
            data = stringData.Split(new string[] {"===="}, StringSplitOptions.None);
            UpdateText(0);
        }
    }
    public void UpdateText(int state)
    {
        string currentText = data[state].Substring(4);
        texDraw.text = currentText;
    }
}
