using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    private int currentContent = 0;
    private int contentNum;
    private void Start()
    {
        contentNum = transform.childCount;
    }
    public void OnClickNextContent()
    {
        if (currentContent < contentNum - 1)
        {
            currentContent += 1;
            DisplayCurrent();
        }
    }
    public void OnClickPreviousContent()
    {
        if (currentContent > 0)
        {
            currentContent -= 1;
            DisplayCurrent();
        } 
    }
    private void DisplayCurrent()
    {
        for (int i = 0; i < contentNum; i++)
        {
            if (i == currentContent){
                transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
