using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AROnOff : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject arMainCam;
    public GameObject model;
    public GameObject arModelParent;
    public GameObject modelParent;

    private bool arOn = false;

    public void OnClickOnOffAR(bool state)
    {
        arOn = state;
        OnOffAR();
    }
    private void OnOffAR()
    {
        mainCam.SetActive(!arOn);
        arMainCam.SetActive(arOn);
        if (arOn)
        {
            model.transform.SetParent(arModelParent.transform);
            ResetTransform(model.transform, new Vector3(0, 0, 0.05f), Quaternion.identity, new Vector3(
                RemoteCSVLoader.displayingScale*0.05f, RemoteCSVLoader.displayingScale*0.05f, RemoteCSVLoader.displayingScale*0.05f
                ));
            model.transform.Rotate(new Vector3(90, 0, 0));

            LineRenderer[] lines = model.GetComponentsInChildren<LineRenderer>();
            foreach (var line in lines)
            {
                if(line.gameObject.name == "line")
                {
                    line.startWidth = 0.001f;
                    line.endWidth = 0.001f;
                }
                else if (line.gameObject.name == "arrow")
                {
                    line.startWidth = 0.005f;
                    line.endWidth = 0;
                }
            }
        }
        else
        {
            model.transform.SetParent(modelParent.transform);
            ResetTransform(model.transform, new Vector3(0, 0, 0), Quaternion.identity, new Vector3(
                RemoteCSVLoader.displayingScale, RemoteCSVLoader.displayingScale, RemoteCSVLoader.displayingScale
                ));

            LineRenderer[] lines = model.GetComponentsInChildren<LineRenderer>();
            foreach (var line in lines)
            {
                if (line.gameObject.name == "line")
                {
                    line.startWidth = 0.02f;
                    line.endWidth = 0.02f;
                }
                else if (line.gameObject.name == "arrow")
                {
                    line.startWidth = 0.12f;
                    line.endWidth = 0;
                }
            }
        }
    }
    private void ResetTransform(Transform trans, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        trans.localPosition = pos;
        trans.localRotation = rot;
        trans.localScale = scale;
    }
}
