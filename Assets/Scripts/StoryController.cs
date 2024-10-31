using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoryController : MonoBehaviour
{
    public static StoryController instance;
    public Camera cam;
    public TMPro.TextMeshProUGUI step;
    public TMPro.TextMeshProUGUI stepNameText;
    public RemoteTextLoader textLoader;
    private List<List<int>> layerFilters;
    [HideInInspector]
    public List<int> currentLayerFilter;
    private List<string> stepNames;
    public int currentState;
    private int tempState;
    private List<GameObject> text3DList;
    private void Awake()
    {
        instance = this;
        currentState = 0;
        tempState = 0;
    }
    private void Start()
    {
        StartCoroutine(ReadCSVAsync());
        StartCoroutine(IsLoadComplete());
    }
    IEnumerator ReadCSVAsync()
    {
        yield return new WaitForSeconds(1.0f);
 
        layerFilters = RemoteCSVLoader.StoryLine.layerFilters.ToList();
        stepNames = RemoteCSVLoader.StoryLine.stepNameArray.ToList();
        currentLayerFilter = layerFilters[0];
    }
    private LayerMask IndexesToLayerMask(List<int> indexes)
    {
        LayerMask mask = 1 << 0;
        foreach (int index in indexes)
        {
            if(index > 0)
            mask |= 1 << index;
        }
        return mask;
    }
    public void OnClickPreviousNext(int nextIndex)
    {
        tempState += nextIndex;
        if (tempState < 0 || tempState > RemoteCSVLoader.storyCount - 1)
        {
            tempState = currentState;
            return;
        }
        else
        {
            currentState = tempState;
            SetStep(currentState);
        }
    }
    public void SetStep(int index)
    {
        currentState = index;
        currentLayerFilter = layerFilters[currentState];

        step.text = currentState.ToString();
        stepNameText.text = stepNames[currentState];

        textLoader.UpdateText(currentState);

        UpdateLayerMask();

        if (LoadingProgress.Instance.is3DText)
        {
            Displaying3DText(currentState);
        }

    }
    public void UpdateLayerMask()
    {
        cam.cullingMask = IndexesToLayerMask(currentLayerFilter);
    }
    private void Displaying3DText(int step)
    {
        for (int i = 0; i < text3DList.Count; i++)
        {
            if(i == step)
            {
                text3DList[i].SetActive(true);
            }
            else
            {
                text3DList[i].SetActive(false);
            }
        }
    }
    private void Collect3DText()
    {
        text3DList = new List<GameObject>();
        GameObject textParent = GameObject.Find("ModelParent/RemoteLoader/textParent");
        if (textParent)
        {
            foreach (Transform transform in textParent.transform)
            {
                text3DList.Add(transform.gameObject);
            }
        }
    }
    IEnumerator IsLoadComplete()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (LoadingProgress.Instance.loadComplete)
            {
                if (LoadingProgress.Instance.is3DText)
                {
                    Collect3DText();
                }
                SetStep(0);
                break;
            }
        }
    }
}
