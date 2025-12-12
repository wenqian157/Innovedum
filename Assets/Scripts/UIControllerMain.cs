using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControllerMain : MonoBehaviour
{
    public Camera arCam;
    public Camera cam;
    public GameObject stepGO;
    public GameObject layerGO;
    public AROnOff arOnOff;
    public GameObject stepTextGO;
    public GameObject navigationGO;
    private int currentState = 0;

    public void OnUISwitchState(int state)
    {
        currentState = state;
        if(currentState == 0) //by steps
        {
            stepGO.SetActive(true);
            stepTextGO.SetActive(true);
            layerGO.SetActive(false);
            arOnOff.OnClickOnOffAR(false);
            navigationGO.SetActive(true);

            int currentStep = StepControl.instance.currentStep;
            StepControl.instance.SetStep(currentStep);
        }

        else if(currentState == 1) //by layers
        {
            stepGO.SetActive(false);
            stepTextGO.SetActive(false);
            layerGO.SetActive(true);
            arOnOff.OnClickOnOffAR(false);
            navigationGO.SetActive(true);

            int currentStep = StepControl.instance.currentStep;
            List<int> currentFilter = LoadCSV.instance.myStoryObject.layerFilters[currentStep].ToList();
            LayerControl.instance.currentFilter = currentFilter;
            LayerControl.instance.UpdateLayerToggles();
        }

        else if (currentState == 2)  // ar mode
        {
            stepGO.SetActive(false);
            stepTextGO.SetActive(false);
            layerGO.SetActive(false);
            arOnOff.OnClickOnOffAR(true);
            navigationGO.SetActive(false);
            arCam.cullingMask = cam.cullingMask;
        }
    }
}
