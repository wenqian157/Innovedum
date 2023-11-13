using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    private Camera cam;
    private List<string> layerList;
    private void Awake()
    {
        cam = Camera.main;

        string[] layersArray = { "Default", "UI", "Concrete" }; //"Reinforcement", "Insulation", "TopLayer", "StructuralSystem"
        layerList = new List<string>(layersArray);

        cam.cullingMask = LayerMask.GetMask(layerList.ToArray());

    }
    public void OnClickDisplayLayer(string layer)
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
    }
}
