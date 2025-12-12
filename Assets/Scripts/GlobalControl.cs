using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    void Start()
    {
        StartCoroutine(LoadComplete());
    }
    IEnumerator LoadComplete()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (LoadingProgress.Instance.loadComplete)
            {
                SetGlobal();
                break;
            }
        }
    }
    private void SetGlobal()
    {
        textMeshPro.text = LoadInfo.instance.projectName;

        Vector3 scale = new Vector3(
            LoadInfo.instance.projectScale,
            LoadInfo.instance.projectScale,
            LoadInfo.instance.projectScale);
        transform.localScale = scale;

        StepControl.instance.SetStep(0);

        Logs log = Logs.Instance.gameObject.GetComponent<Logs>();
        log.debug.enabled = false;
    }
}
