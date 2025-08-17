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

    [SerializeField]
    private GameEvent nextTutorialEvent;
    private GameEventListener tutorialListener;

    private void Start()
    {
        // Inititial an event listener for cycling through tutorials.
        tutorialListener = gameObject.AddComponent<GameEventListener>();
        tutorialListener = gameObject.AddComponent<GameEventListener>();
        tutorialListener.Events.Add(nextTutorialEvent);
        tutorialListener.Response = new();
        tutorialListener.Response.AddListener(() => NextTutorial());

        // register
        nextTutorialEvent.RegisterListener(tutorialListener);

        // For each of the series of tutorials, create an event listener that will add the series to the queue.
        foreach (TutorialSeries series in tutorialList) 
        {
            // set up a new event listener for the new event
            GameEventListener listener = new GameEventListener();

            listener = gameObject.AddComponent<GameEventListener>();
            listener.Events.Add(series.targetEvent);
            listener.Response = new();
            listener.Response.AddListener(() => AddToQueue(series));

            // register
            series.targetEvent.RegisterListener(listener);
        }
    }

    private void NextTutorial() 
    {

    }

    private void AddToQueue(TutorialSeries series) 
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

    private bool triggered = false;

    public void WasTriggered() 
    {
        triggered = true;
    }
}

