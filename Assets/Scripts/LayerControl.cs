using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LayerControl : MonoBehaviour
{
    public static LayerControl instance;
    public Camera cam;
    public List<int> currentLayerList;
    public static List<string> layerNameList;
    public static List<int> layerList;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        yield return new WaitForSeconds(0.5f);
        layerNameList = new List<string>() {
            "Default", "TransparentFX", "Ignore Raycast", "None", "Water", "UI"
        };
        foreach (var item in LoadCSV.instance.myLayerObjects)
        {
            layerNameList.Add(item.displayingName);
        }
        layerList = Enumerable.Range(0, layerNameList.Count).ToList();
    }
    public LayerMask IndexesToLayerMaxk(List<int> indexes)
    {
        LayerMask mask = 1 << 0;
        foreach (var index in indexes)
        {
            if (index > 0) mask |= 1 << index;
        }
        return mask;
    }
    public void UpdateLayerMask()
    {
        cam.cullingMask = IndexesToLayerMaxk(currentLayerList);
    }
}
