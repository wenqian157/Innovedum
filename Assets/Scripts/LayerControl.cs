using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LayerControl : MonoBehaviour
{
    public static LayerControl instance;
    public Camera cam;
    public GameObject layerToggleParent;
    [HideInInspector]
    public List<string> layerNameList;
    [HideInInspector]
    public List<int> layerList;
    private List<Toggle> allToggles;
    public List<int> currentFilter;
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
        yield return new WaitForSeconds(0.5f);
        layerNameList = new List<string>() {
            "Default", "TransparentFX", "Ignore Raycast", "None", "Water", "UI"
        };
        foreach (var item in LoadCSV.instance.myLayerObjects)
        {
            layerNameList.Add(item.displayingName);
        }
        layerList = Enumerable.Range(0, layerNameList.Count).ToList();
        currentFilter = layerList;
        CreateLayerToggles();
    }
    private LayerMask IndexesToLayerMaxk(List<int> indexes)
    {
        LayerMask mask = 1 << 0;
        foreach (var index in indexes)
        {
            if (index > 0) mask |= 1 << index;
        }
        return mask;
    }
    private void CreateLayerToggles()
    {
        for (int i = 0; i < layerNameList.Count; i++)
        {
            if (i >= 6) // do not create toggles for the first 6 default unity layers
            {
                string name = layerNameList[i];
                int layerIndex = i; // this has to be a local copy

                var layerToggle = Resources.Load("ToggleLayer") as GameObject;
                layerToggle = Instantiate(layerToggle, layerToggleParent.transform);

                layerToggle.GetComponentInChildren<Text>().text = name;

                Toggle mToggle = layerToggle.GetComponent<Toggle>();
                mToggle.onValueChanged.AddListener(delegate
                {
                    OnUIDisplayLayer(layerIndex);
                });

                allToggles.Add(mToggle);
            }
        }
    }
    private void OnUIDisplayLayer(int index)
    {
        if (!currentFilter.Contains(index))
        {
            currentFilter.Add(index);
        }
        else
        {
            currentFilter.Remove(index);
        }
        UpdateLayerMask();
    }
    public void UpdateLayerToggles()
    {
        List<int> copyList = currentFilter.ToList();
        TurnOffAllLayers();
        foreach (int i in copyList)
        {
            if (i != 0)
                allToggles[i - 6].isOn = true;
        }
    }
    public void UpdateLayerMask()
    {
        cam.cullingMask = IndexesToLayerMaxk(currentFilter);
    }
    public void TurnOffAllLayers()
    {
        foreach (var toggle in allToggles)
        {
            toggle.isOn = false;
        }
        currentFilter = Enumerable.Range(0, 5).ToList();
    }
}
