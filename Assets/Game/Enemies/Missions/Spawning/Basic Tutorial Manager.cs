using GeneralUtility.GameEventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputHandler : MonoBehaviour
{
    [Header("Tutorial Events")]
    [SerializeField]
    private GameEvent tutorialContinueInput;

    public void TutorialContInput(InputAction.CallbackContext context)
    {
        if (context.started)
            tutorialContinueInput.Trigger();
    }
}

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

    private bool tutorialActive = false;

    private void Awake()
    {
        tutorialQueue = new List<Tutorials>();
    }

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
        // Ignore input if there is no tutorial active at the moment.
        if (!tutorialActive) 
        {
            return;
        }

        // if there are no more tutorials, close them.
        else if (tutorialQueue.Count == 0) 
        {
            tutorialPanel.SetActive(false);
            tutorialActive = false;
            return;
        }

        objectiveText.text = tutorialQueue[0].text;
        tutorialQueue.RemoveAt(0);
    }

    private void AddToQueue(TutorialSeries series) 
    {
        // if this series has already been triggered, do nothing
        if (series.IsTriggered()) 
        {
            return;
        }

        List<Tutorials> tutorials = series.tutorials;

        foreach (Tutorials t in tutorials)
        {
            tutorialQueue.Add(t);
        }

        series.WasTriggered();

        // if the tutorial isn't already on, start it.
        if (!tutorialActive) 
        {
            tutorialPanel.SetActive(true);
            tutorialActive = true;
        }
    }
}

[Serializable]
public class Tutorials
{
    [TextArea(1,10)]
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

    public bool IsTriggered() 
    {
        return triggered;
    }
}

