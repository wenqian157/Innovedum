using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Logs : MonoBehaviour
{
    public static Logs Instance;

    public TextMeshProUGUI announce;
    public TextMeshProUGUI debug;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance == this) return;
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    private void Start()
    {
        announce.text = "default text";
    }
    private void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }
    private void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }
    private void LogMessage(string message, string stackTrace, LogType type)
    {
        if (debug.text.Length > 500)
        {
            debug.text = message + "\n";
        }
        else
        {
            debug.text += message + "\n";
        }
    }

}
