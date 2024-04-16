using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject detialButtons;
    public void OnUILoadMain()
    {
        Debug.Log("load main scene...");
        Logs.Instance.announce.text = "load main scene...";
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
