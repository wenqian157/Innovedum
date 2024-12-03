using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RemoteInfoLoader : MonoBehaviour
{
    public List<string> projectUrlList = new List<string>();
    public List<string> projectNameList = new List<string>();
    public static string urlBase;
    public static RemoteInfoLoader Instance;
    public int projectID;

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
    private void Start()
    {
        OnUIReadInfo(projectID);
    }
    public void OnUIReadInfo(int id)
    {
        urlBase = projectUrlList[id];
        if (urlBase is null) return;


        RemoteCSVLoader.urlBase = urlBase;
        RemoteCSVLoader.projectID = id;
        RemoteCSVLoader.projectName = projectNameList[id];

        switch (id)
        {
            case 0:
                RemoteCSVLoader.displayingScale = 1;
                break;
            case 1:
                RemoteCSVLoader.displayingScale = 0.5f;
                break;
        }
        RemoteCSVLoader.OnUILoadScene();
    }

}
