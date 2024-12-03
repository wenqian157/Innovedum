using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RemoteInfoLoader : MonoBehaviour
{
    public List<string> projectUrlList = new List<string>(
        )
    { "https://raw.githubusercontent.com/wenqian157/webServer/main/torsion",
      "https://raw.githubusercontent.com/wenqian157/webServer/main/tBeam"};

    public int projectID;
    public static string urlBase;

    private void Start()
    {
        OnUIReadInfo(projectID);
    }
    public void OnUIReadInfo(int id)
    {
        RemoteCSVLoader.urlBase = projectUrlList[id];
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
        RemoteCSVLoader.OnUILoadScene();
    }
}
