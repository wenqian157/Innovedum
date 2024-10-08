using System.Collections;
using System.Collections.Generic;
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
    //private int tempState;

    private void Awake()
    {
        //tempState = currentState;
    }
    public void OnUIBack2Menu()
    {
        Debug.Log("load open scene...");
        SceneManager.LoadScene("Open", LoadSceneMode.Single);
    }
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

            StoryController.instance.OnUISetStep(StoryController.instance.currentState);
        }

        else if(currentState == 1) //by layers
        {
            stepGO.SetActive(false);
            stepTextGO.SetActive(false);
            layerGO.SetActive(true);
            arOnOff.OnClickOnOffAR(false);
            navigationGO.SetActive(true);

            LayerController.instance.UpdateLayerToggles(
            StoryController.instance.currentLayerFilter);
        }

        else if(currentState == 2)  // ar mode
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
