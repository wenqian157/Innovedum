using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    public Camera arCam;
    public Camera cam;
    public TMPro.TextMeshProUGUI text;
    public TMPro.TextMeshProUGUI nameText;
    private List<string> layerList;
    private List<List<string>> layerFilters;
    private int currentState;
    private int tempState;
    private List<string> currentLayerList;
    private List<string> layerNames;
    private void Awake()
    {
        //string[] layersArray = { "Default", "UI", "Concrete", "Reinforcement", "TransverseBendingMoment", "SlabStripTransverse", "StrutAndTie", "AeraLoad", "T-BeamLongidtudinal",
        //"ReinforcementTransverse", "ReinforcementLongitudinal", "ReinforcementShear"};

        string[] layersArray =
        {
            "ConcreteWandscheibe", "ConcreteHohlkasten", "StrutAndTieWandscheibeStrut", "StrutAndTieWandscheibeTie", "StrutAndTieHohlkastenStrut", "StrutAndTieHohlkastenTie",
            "ReinforcementWandscheibeA", "ReinforcementWandscheibeB", "ReinforcementWandscheibeC", "ReinforcementWandscheibeD", "ReinforcementWandscheibeE", "ReinforcementWandscheibeF",
            "ReinforcementWandscheibeH", "ReinforcementWandscheibeBuegel4", "ReinforcementWandscheibeLaengsbewehrungVerteilt", "ReinforcementWandscheibeSteckbuegel", "ReinforcementHohlkastenLaengsbewehrungVerteilt",
            "ReinforcementHohlkastenBuegel", "ReinforcementHohlkastenGurt1-2_2-3", "ReinforcementHohlkastenGurt3-4_4-1", "ExternalForce", "InternalForceWandscheibe", "InternalForceHohlkasten",
            "SectionForcesWandscheibe", "SectionForcesHohlkasten", "Default"
        };

        layerList = new List<string>(layersArray);
        InitLayerFilter();
        InitLayerNames();

        currentState = 0;
        tempState = 0;
        currentLayerList = layerFilters[currentState];

        cam.cullingMask = LayerMask.GetMask(currentLayerList.ToArray());
        arCam.cullingMask = LayerMask.GetMask(currentLayerList.ToArray());
    }
    private void InitLayerFilter()
    {
        var digiMask = new List<List<int>> {
            new List<int> { 0, 1, 20, 25 },
            new List<int> { 1, 20, 21, 25 },
            new List<int> { 1, 20, 21, 23, 25 },
            new List<int> { 1, 2, 3, 20, 21, 25 },
            new List<int> { 1, 3, 25 },
            new List<int> { 1, 3, 6, 25 },
            new List<int> { 1, 3, 7, 25 },
            new List<int> { 1, 3, 8, 25 },
            new List<int> { 1, 3, 9, 25 },
            new List<int> { 1, 3, 10, 25 },
            new List<int> { 1, 3, 11, 14, 25 },
            new List<int> { 1, 3, 12, 25 },
            new List<int> { 1, 3, 13, 15, 25},
            new List<int> { 1, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 25 },
            new List<int> { 1, 2, 3, 20, 21, 25 },
            new List<int> { 2, 3, 20, 21, 25 },
            new List<int> { 22, 25 },
            new List<int> { 22, 23, 24, 25 },
            new List<int> { 4, 5, 22, 25 },
            new List<int> { 2, 3, 4, 5, 20, 25 },
            new List<int> { 4, 5, 22, 25 },
            new List<int> { 5, 25 },
            new List<int> { 5, 17, 25 },
            new List<int> { 5, 16, 25 },
            new List<int> { 5, 18, 25 },
            new List<int> { 5, 19, 25 },
            new List<int> { 16, 17, 18, 19, 25 },
            new List<int> { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 25 }
        };

        layerFilters = new List<List<string>>();
        for (int i = 0; i < 28; i++)
        {
            var layerfilter = new List<string>();
            for (int j = 0; j < 26; j++)
            {
                if (digiMask[i].Contains(j))
                {
                    layerfilter.Add(layerList[j]);
                }
            }
            layerFilters.Add(layerfilter);
        };
        //TODO this step could be automated
    }
    private void InitLayerNames()
    {
        layerNames = new List<string>
        {
            "Betongeometrie", "Kräfte auf Wandscheibe", "Schnittgrössen Wandscheibe", "Fachwerkmodell Wandscheibe", "Zugstreben Wandscheibe",
            "Bewehrung Strebe A", "Bewehrung Strebe B", "Bewehrung Strebe C", "Bewehrung Strebe D", "Bewehrung Strebe E",
            "Bewehrung Strebe F", "Bewehrung Strebe H", "Konstruktive Bewehrung Wandscheibe", "Bewehrung Wandscheibe", "Kräfte auf Wandscheibe",
            "Kräfte auf Wandscheibe", "Kräfte auf Hohlkasten", "Schnittgrössen Hohlkasten", "Fachwerkmodell Hohlkasten", "Fachwerkmodell komplett",
            "Fachwerkmodell Hohlkasten", "Zugstreben Hohlkasten", "Bügelbewehrung Hohlkasten", "Verteilte Längsbewehrung Hohlkasten", "Bewehrung Gurt 1-2/2-3 Hohlkasten",
            "Bewehrung Gurt 3-4/4-1 Hohlkasten", "Bewehrung Hohlkasten", "Bewehrung komplett", 
        };
    }
    public void OnClickDisplayLayer(string layer)
    {
        if (!layerList.Contains(layer))
        {
            layerList.Add(layer);
        }
        else
        {
            layerList.Remove(layer);
        }
        cam.cullingMask = LayerMask.GetMask(layerList.ToArray());
        arCam.cullingMask = LayerMask.GetMask(layerList.ToArray());
    }
    public void OnClickPreviousNext(int nextIndex)
    {
        tempState += nextIndex;
        if (tempState < 0 || tempState > 27)
        {
            tempState = currentState;
            return;
        }
        else
        {
            currentState = tempState;
            currentLayerList = layerFilters[currentState];

            cam.cullingMask = LayerMask.GetMask(currentLayerList.ToArray());
            arCam.cullingMask = LayerMask.GetMask(currentLayerList.ToArray());

            text.text = currentState.ToString();
            nameText.text = layerNames[currentState];
        }
    }
}
