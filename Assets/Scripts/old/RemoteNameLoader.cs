using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemoteNameLoader : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    void Start()
    {
        if(RemoteCSVLoader.projectName is null)
        {
            return;
        }
        textMeshPro.text = RemoteCSVLoader.projectName;
    }
}
