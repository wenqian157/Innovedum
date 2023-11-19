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

    public void OnClickOnOffAR()
    {
        arOn = !arOn;
        Debug.Log($"ARsession: {arOn}");
        mainCam.SetActive(!arOn);
        arMainCam.SetActive(arOn);
        if (arOn)
        {
            model.transform.SetParent(arModelParent.transform);
            ResetTransform(model.transform, new Vector3(0, 0, 0.05f), Quaternion.identity, new Vector3(0.01f, 0.01f, 0.01f));
            model.transform.Rotate(new Vector3(90, 0, 0));

            LineRenderer[] lines = model.GetComponentsInChildren<LineRenderer>();
            foreach (var line in lines)
            {
                line.startWidth = 0.001f;
                line.endWidth = 0.001f;
            }
        }
        else
        {
            model.transform.SetParent(modelParent.transform);
            ResetTransform(model.transform, new Vector3(0, 0, 0), Quaternion.identity, new Vector3(1, 1, 1));

            LineRenderer[] lines = model.GetComponentsInChildren<LineRenderer>();
            foreach (var line in lines)
            {
                line.startWidth = 0.05f;
                line.endWidth = 0.05f;
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
