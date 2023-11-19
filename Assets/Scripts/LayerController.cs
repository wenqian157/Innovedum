using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    public Camera arCam;
    public Camera cam;
    private List<string> layerList;
    private void Awake()
    {
        string[] layersArray = { "Default", "UI", "Concrete", "Reinforcement", "TransverseBendingMoment", "SlabStripTransverse", "StrutAndTie", "AeraLoad", "T-BeamLongidtudinal" };
        layerList = new List<string>(layersArray);

        cam.cullingMask = LayerMask.GetMask(layerList.ToArray());
        arCam.cullingMask = LayerMask.GetMask(layerList.ToArray());
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
        arCam.cullingMask = LayerMask.GetMask(layerList.ToArray());
    }
}
