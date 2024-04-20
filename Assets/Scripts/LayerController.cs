using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerController : MonoBehaviour
{
    public Camera arCam;
    public Camera cam;
    public GameObject layerParent;
    private List<string> layerList;
    private void Start()
    {
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        yield return new WaitForSeconds(1.0f);
        layerList = new List<string>() { "Default" , "UI" };
        foreach (var item in RemoteCSVLoader.myLayerObjects)
        {
            layerList.Add(item.name);
        }
        cam.cullingMask = LayerMask.GetMask(layerList.ToArray());
        arCam.cullingMask = LayerMask.GetMask(layerList.ToArray());

        CreateLayerToggle();
    }
    private void CreateLayerToggle()
    {
        foreach (var name in layerList)
        {
            var layerToggle = Resources.Load("ToggleLayer") as GameObject;
            layerToggle = Instantiate(layerToggle, layerParent.transform);
            layerToggle.GetComponentInChildren<Text>().text = name;
        }
    }
    public void OnUIDisplayLayer(string layer)
    {
        if (!layerList.Contains(layer))
        {
            layerList.Add(layer);
        }
        else
        {
            layerList.Remove(layer);
        }
        cam.cullingMask = LayerMask.GetMask(layerList.ToArray());
        arCam.cullingMask = LayerMask.GetMask(layerList.ToArray());
    }
}
