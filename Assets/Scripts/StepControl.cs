using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StepControl : MonoBehaviour
{
    public static StepControl instance;
    public TMPro.TextMeshProUGUI stepCount;
    public TMPro.TextMeshProUGUI stepName;
    [HideInInspector]
    public int currentStep;
    private void Awake()
    {
        instance = this;
        currentStep = 0;
    }
    public void ChangeStep(int change)
    {
        int tempStep = currentStep + change;
        if(tempStep < 0 || tempStep >= LoadCSV.instance.myStoryObject.layerFilters.Length)
        {
            return;
        }
        else
        {
            currentStep = tempStep;
            SetStep(currentStep);
        }
    }
    public void SetStep(int step)
    {
        stepCount.text = step.ToString();
        stepName.text = LoadCSV.instance.myStoryObject.stepNameArray[step];
        LoadMD.instance.UpdateMD(step);
        SetLayerMaskByStep(step);
    }
    private void SetLayerMaskByStep(int step)
    {
        // copy the filter from loadcsv
        List<int> currentLayerFilter = LoadCSV.instance.myStoryObject.layerFilters[step].ToList();
        LayerControl.instance.currentFilter = currentLayerFilter;
        LayerControl.instance.UpdateLayerMask();
    }
}
