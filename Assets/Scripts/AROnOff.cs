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
            float arScale = LoadInfo.instance.projectScale * LoadInfo.instance.arScale;
            ResetTransform(model.transform, new Vector3(0, 0, 0), Quaternion.identity, 
                new Vector3(arScale, arScale, arScale));
            model.transform.Rotate(new Vector3(90, 0, 0));

            LineRenderer[] lines = model.GetComponentsInChildren<LineRenderer>();
            foreach (var line in lines)
            {
                if(line.gameObject.name == "line")
                {
                    line.startWidth = LoadInfo.instance.lineWidth * LoadInfo.instance.arScale;
                    line.endWidth = LoadInfo.instance.lineWidth * LoadInfo.instance.arScale;
                }
                else if (line.gameObject.name == "arrow")
                {
                    line.startWidth = LoadInfo.instance.arrowSize * LoadInfo.instance.arScale;
                    line.endWidth = 0;
                }
            }
        }
        else
        {
            model.transform.SetParent(modelParent.transform);
            ResetTransform(model.transform, new Vector3(0, 0, 0), Quaternion.identity, new Vector3(
                LoadInfo.instance.projectScale, LoadInfo.instance.projectScale, LoadInfo.instance.projectScale
                ));

            LineRenderer[] lines = model.GetComponentsInChildren<LineRenderer>();
            foreach (var line in lines)
            {
                if (line.gameObject.name == "line")
                {
                    line.startWidth = LoadInfo.instance.lineWidth;
                    line.endWidth = LoadInfo.instance.lineWidth;
                }
                else if (line.gameObject.name == "arrow")
                {
                    line.startWidth = LoadInfo.instance.arrowSize;
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
