using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingProgress : MonoBehaviour
{
    public static LoadingProgress Instance;
    [HideInInspector]
    public int coroutineCount;
    private bool loadStart;
    [HideInInspector]
    public bool loadComplete = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            coroutineCount = 0;
            loadStart = true;
        }
        else
        {
            if (Instance == this) return;
            Destroy(Instance.gameObject);
            Instance = this;
            coroutineCount = 0;
            loadStart = true;
        }
    }
    private void Update()
    {
        if (coroutineCount > 0 && loadStart)
        {
            loadStart = false;
        }
        if (coroutineCount == 0 && !loadStart)
        {
            loadComplete = true;
            Debug.Log("load complete ---------------------");
            GameObject log = GameObject.FindWithTag("Log");
            if (log is not null)
            {
                log.SetActive(false);
            }
            StoryController.instance.UpdateLayerMask();
            enabled = false;
        }
    }
}
