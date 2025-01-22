using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RemoteProjectLoader : MonoBehaviour
{
    public GameObject buttonParent;

    private string url= "https://raw.githubusercontent.com/wenqian157/webServer/refs/heads/main/StructureXRProjects/projectList.json";
    [Serializable]
    public class Project
    {
        public string name;
        public string url;
    }
    private Project[] projectList;
    void Start()
    {
        StartCoroutine(ReadProjectJson(url));
    }
    private IEnumerator ReadProjectJson(string url)
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
                Debug.Log("loading project list...");
                yield return new WaitForSeconds(0.2f);
            }
            string stringData = www.downloadHandler.text;
            projectList = JsonConvert.DeserializeObject<Project[]>(stringData);
            Debug.Log($"found {projectList.Length} projects");
            SpawnProjectMenu();
        }
    }
    private void SpawnProjectMenu()
    {
        for (int i = 0; i < projectList.Length; i++)
        {
            string name = projectList[i].name;
            string url = projectList[i].url;

            var projectButton = Resources.Load("Button") as GameObject;
            projectButton = Instantiate(projectButton, buttonParent.transform);

            projectButton.GetComponentInChildren<TextMeshProUGUI>().text = name;

            Button button = projectButton.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {

                OnUILoadScene(name, url);
            });
        }
    }
    private void OnUILoadScene(string name, string url)
    {
        if (url is null) return;
        Debug.Log(url);

        RemoteCSVLoader.urlBase = url;
        RemoteCSVLoader.projectID = 0; // tbd
        RemoteCSVLoader.projectName = name;
        RemoteCSVLoader.displayingScale = 1;
        RemoteCSVLoader.instance.OnUILoadScene();
    }
}
