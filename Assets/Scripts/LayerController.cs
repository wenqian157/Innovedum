using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LayerController : MonoBehaviour
{
    public static LayerController instance;
    public Camera arCam;
    public Camera cam;
    public GameObject layerParent;
    public static List<string> layerNameList;
    public static List<int> layerList;
    private List<int> currentLayerList;
    private static List<Toggle> allToggles;
    private void Awake()
    {
        instance = this;
        allToggles = new List<Toggle>();
    }
    private void Start()
    {
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        yield return new WaitForSeconds(1.0f);
        layerNameList = new List<string>() {
            "Default", "TransparentFX", "Ignore Raycast", "None", "Water", "UI"
        };
        foreach (var item in RemoteCSVLoader.myLayerObjects)
        {
            layerNameList.Add(item.name);
        }
        layerList = Enumerable.Range(0, layerNameList.Count+6).ToList();
        currentLayerList = layerList;

        CreateLayerToggle();
    }
    private void CreateLayerToggle()
    {
        for (int i = 0; i < layerNameList.Count; i++)
        {
            if(i >=6) // do not create toggles for the first 6 default unity layers
            {
                string name = layerNameList[i];
                int layerIndex = i; // this has to be a local copy

                var layerToggle = Resources.Load("ToggleLayer") as GameObject;
                layerToggle = Instantiate(layerToggle, layerParent.transform);

                layerToggle.GetComponentInChildren<Text>().text = name;

                Toggle mToggle = layerToggle.GetComponent<Toggle>();
                mToggle.onValueChanged.AddListener(delegate {
                    OnUIDisplayLayer(layerIndex);
                });

                allToggles.Add(mToggle);
            } 
        }
    }
    void OnUIDisplayLayer(int index)
    {
        if (!currentLayerList.Contains(index))
        {
            currentLayerList.Add(index);
        }
        else
        {
            currentLayerList.Remove(index);
        }
        cam.cullingMask = IndexesToLayerMask(currentLayerList);
        arCam.cullingMask = IndexesToLayerMask(currentLayerList);

        //cam.cullingMask = LayerMask.GetMask(currentLayerList.ToArray());
        //arCam.cullingMask = LayerMask.GetMask(currentLayerList.ToArray());
    }
    public void TurnOnAllLayers()
    {
        foreach (var toggle in allToggles)
        {
            toggle.isOn = true;
        }
        cam.cullingMask = IndexesToLayerMask(layerList);
        arCam.cullingMask = IndexesToLayerMask(layerList);
    }
    public LayerMask IndexesToLayerMask(List<int> indexes)
    {
        LayerMask mask = 1 << 0;
        foreach (int index in indexes)
        {
            if (index > 0)
                mask |= 1 << index;
        }
        return mask;
    }
}
