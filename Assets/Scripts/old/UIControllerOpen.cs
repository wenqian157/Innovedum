using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControllerOpen : MonoBehaviour
{
    public GameObject detailButtons;
    public void OnUIReturn2Begin()
    {
        detailButtons.SetActive(false);
        Logs.Instance.announce.text = "select the project to load...";
    }
}
