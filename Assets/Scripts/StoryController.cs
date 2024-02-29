using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoryController : MonoBehaviour
{
    public Camera arCam;
    public Camera cam;
    public TMPro.TextMeshProUGUI step;
    public TMPro.TextMeshProUGUI stepNameText;
    private List<List<int>> layerFilters;
    private List<int> currentLayerFilter;
    private List<string> stepNames;
    private List<int> layerRange;
    private int currentState;
    private int tempState;
    private void Awake()
    {
        layerRange = Enumerable.Range(0, 30).ToList();
        currentState = 0;
        tempState = 0;
    }
    private void Start()
    {
        StartCoroutine(ReadCSVAsync());
    }
    IEnumerator ReadCSVAsync()
    {
        while (RemoteCSVLoader.StoryLine.layerFilters.Length == 0)
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("reading cvs file...");
        }
        layerFilters = RemoteCSVLoader.StoryLine.layerFilters.ToList();
        stepNames = RemoteCSVLoader.StoryLine.stepNameArray.ToList();

        cam.cullingMask = IndexesToLayerMask(layerFilters[0]);
        arCam.cullingMask = IndexesToLayerMask(layerFilters[0]);
    }
    private LayerMask IndexesToLayerMask(List<int> indexes)
    {
        LayerMask mask = 1 << 0;
        foreach (int index in indexes)
        {
            if(index > 0)
            mask |= 1 << index;
        }
        return mask;
    }
    public void OnClickPreviousNext(int nextIndex)
    {
        tempState += nextIndex;
        if (tempState < 0 || tempState > RemoteCSVLoader.storyCount - 1)
        {
            tempState = currentState;
            return;
        }
        else
        {
            currentState = tempState;
            currentLayerFilter = layerFilters[currentState];

            cam.cullingMask = IndexesToLayerMask(currentLayerFilter);
            arCam.cullingMask = IndexesToLayerMask(currentLayerFilter);

            step.text = currentState.ToString();
            stepNameText.text = stepNames[currentState];
        }
    }
    //public void OnClickDisplayLayer(int layerIndex)
    //{
    //    if (!layerRange.Contains(layerIndex))
    //    {
    //        layerList.Add(layer);
    //    }
    //    else
    //    {
    //        layerList.Remove(layer);
    //    }
    //    cam.cullingMask = LayerMask.GetMask(layerList.ToArray());
    //    arCam.cullingMask = LayerMask.GetMask(layerList.ToArray());
    //}
}
