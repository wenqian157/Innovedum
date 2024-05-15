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
    public GameObject arGO;
    private int currentState = 0;
    private int tempState;

    private void Awake()
    {
        tempState = currentState;
    }
    public void OnUIBack2Menu()
    {
        Debug.Log("load open scene...");
        SceneManager.LoadScene("Open", LoadSceneMode.Single);
    }
    public void OnUISwitchState(int state)
    {
        currentState = state;
        if(currentState == 0)
        {
            stepGO.SetActive(true);
            layerGO.SetActive(false);
            arGO.SetActive(false);

            if(tempState != 0)
            {
                StoryController.currentState = 0;
                StoryController.instance.OnUISetStep(0);
                cam.cullingMask = StoryController.instance.IndexesToLayerMask(StoryController.currentLayerFilter);
                arCam.cullingMask = StoryController.instance.IndexesToLayerMask(StoryController.currentLayerFilter);
            }
            tempState = currentState;
        }
        else if(currentState == 1)
        {
            stepGO.SetActive(false);
            layerGO.SetActive(true);
            arGO.SetActive(false);

            if(tempState != 1)
            {
                LayerController.instance.TurnOnAllLayers();
            }
            
            tempState = currentState;
        }
        else if(currentState == 2)
        {
            stepGO.SetActive(false);
            layerGO.SetActive(false);
            arGO.SetActive(true);
        }
    }
}
