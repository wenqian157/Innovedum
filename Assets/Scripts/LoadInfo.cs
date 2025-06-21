using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadInfo : MonoBehaviour
{
    public static LoadInfo instance;
    public string urlBase;
    public string projectName;
    public float projectScale;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance == this) return;
            Destroy(instance.gameObject);
            instance = this;
        }
    }
    private void Start()
    {
        LoadProjectInfo();
    }
    private void LoadProjectInfo()
    {
        if (urlBase is null) return;

        Debug.Log("loading info...");
        LoadCSV.instance.urlBase = urlBase;
        LoadCSV.instance.Load();
    }
}
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
