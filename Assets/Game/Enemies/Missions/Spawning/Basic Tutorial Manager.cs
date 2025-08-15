using GeneralUtility.GameEventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BasicTutorialManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI objectiveText;
    [SerializeField]
    private GameObject tutorialPanel;

    [SerializeField]
    private List<TutorialSeries> tutorialList;

    private List<Tutorials> tutorialQueue;

    private void addToQueue(TutorialSeries series) 
    {
        List<Tutorials> tutorials = series.tutorials;

        foreach (Tutorials t in tutorials)
        {
            tutorialQueue.Add(t);
        }
    }

}

[Serializable]
public class Tutorials
{
    public string text;
    
    // Highlight Focus targets if any
}

[Serializable]
public class TutorialSeries 
{
    public List<Tutorials> tutorials;

    public GameEvent targetEvent;
}

