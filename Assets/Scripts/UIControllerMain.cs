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

    public void OnUIBack2Menu()
    {
        Debug.Log("load open scene...");
        Logs.Instance.announce.text = "select the project to load...";
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

            cam.cullingMask = LayerMask.GetMask(LayerController.layerList.ToArray());
            arCam.cullingMask = LayerMask.GetMask(LayerController.layerList.ToArray());
        }
        else if(currentState == 1)
        {
            stepGO.SetActive(false);
            layerGO.SetActive(true);
            arGO.SetActive(false);

            cam.cullingMask = StoryController.IndexesToLayerMask(StoryController.currentLayerFilter);
            arCam.cullingMask = StoryController.IndexesToLayerMask(StoryController.currentLayerFilter);
        }
        else if(currentState == 2)
        {
            stepGO.SetActive(false);
            layerGO.SetActive(false);
            arGO.SetActive(true);
        }
    }
}